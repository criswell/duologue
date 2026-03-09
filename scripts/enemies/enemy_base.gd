extends Area2D
## Base class for all enemies. Handles collision, color, damage, and scoring.
## Subclasses override _enemy_process() and _enemy_ready() for unique behavior.

const _ColorState := preload("res://scripts/color_state.gd")

const OFFSCREEN_MARGIN := 50.0
const REPEL_FORCE := 5.0

var color_state_index: int = 0
var color_polarity: int = _ColorState.Polarity.POSITIVE
var hit_points: int = 1
var max_hit_points: int = 1
var point_value: int = 25
var point_bonus_per_hp: int = 3
var active: bool = false
var collision_radius: float = 30.0
var move_velocity := Vector2.ZERO

var _spawn_timer: float = 0.0
var _spawn_delay: float = 0.0
var _is_spawning: bool = false
var _is_dying: bool = false
var _death_timer: float = 0.0
var _death_duration: float = 0.7

@onready var collision_shape: CollisionShape2D = $CollisionShape2D


func _ready() -> void:
	# Layer 2 (enemies), detect layer 1 (player) and layer 3 (bullets)
	collision_layer = 2
	collision_mask = 1 | 4

	var shape := CircleShape2D.new()
	shape.radius = collision_radius
	collision_shape.shape = shape

	area_entered.connect(_on_area_entered)
	set_process(false)
	visible = false

	_enemy_ready()


func activate(pos: Vector2, color_idx: int, polarity: int, hp: int = 1, spawn_delay: float = 0.0) -> void:
	global_position = pos
	color_state_index = color_idx
	color_polarity = polarity
	hit_points = hp
	max_hit_points = hp
	active = true
	visible = true
	_is_dying = false
	_death_timer = 0.0
	modulate.a = 1.0

	if spawn_delay > 0.0:
		_is_spawning = true
		_spawn_timer = 0.0
		_spawn_delay = spawn_delay
		collision_shape.set_deferred("disabled", true)
	else:
		_is_spawning = false
		collision_shape.set_deferred("disabled", false)

	_update_colors()
	set_process(true)
	_on_activate()


func deactivate() -> void:
	active = false
	visible = false
	set_process(false)
	collision_shape.set_deferred("disabled", true)


func _process(delta: float) -> void:
	if _is_spawning:
		_spawn_timer += delta
		# Fade in during spawn
		modulate.a = clampf(_spawn_timer / _spawn_delay, 0.0, 1.0)
		if _spawn_timer >= _spawn_delay:
			_is_spawning = false
			modulate.a = 1.0
			collision_shape.set_deferred("disabled", false)
		return

	if _is_dying:
		_death_timer += delta
		modulate.a = clampf(1.0 - _death_timer / _death_duration, 0.0, 1.0)
		_process_dying(delta)
		if _death_timer >= _death_duration:
			deactivate()
		return

	_enemy_process(delta)

	# Keep on screen with soft boundary
	_apply_screen_bounds()


func take_hit(bullet_color: Color) -> void:
	if _is_dying or _is_spawning or not active:
		return

	hit_points -= 1
	_trigger_fx("bullet_hit", global_position, bullet_color)
	if hit_points <= 0:
		_die()
	else:
		_on_hit()


func _die() -> void:
	_is_dying = true
	_death_timer = 0.0
	collision_shape.set_deferred("disabled", true)

	var points := point_value + point_bonus_per_hp * max_hit_points
	GameState.add_score(0, points)
	IntensityManager.register_kill()

	var death_color := _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.LIGHT
	)
	_trigger_fx("enemy_death", global_position, death_color)

	_on_death()


func _trigger_fx(method: String, pos: Vector2, color: Color) -> void:
	var fx_nodes := get_tree().get_nodes_in_group("fx_manager")
	if fx_nodes.size() > 0:
		fx_nodes[0].call(method, pos, color)


func _apply_screen_bounds() -> void:
	var margin := collision_radius
	var vp := GameState.VIEWPORT_SIZE
	# Soft bounce off edges
	if position.x < margin:
		move_velocity.x = absf(move_velocity.x)
	elif position.x > vp.x - margin:
		move_velocity.x = -absf(move_velocity.x)
	if position.y < margin:
		move_velocity.y = absf(move_velocity.y)
	elif position.y > vp.y - margin:
		move_velocity.y = -absf(move_velocity.y)

	position = position.clamp(
		Vector2(margin, margin),
		vp - Vector2(margin, margin)
	)


func _get_player() -> Area2D:
	# Find the player in the scene
	var players := get_tree().get_nodes_in_group("player")
	if players.size() > 0:
		return players[0]
	return null


func _move_toward_player(delta: float, speed: float, accel: float = 5.0) -> void:
	var player := _get_player()
	if not player:
		return
	var dir := (player.global_position - global_position).normalized()
	move_velocity = move_velocity.lerp(dir * speed, accel * delta)
	position += move_velocity * delta


func _update_colors() -> void:
	# Override in subclass for multi-sprite enemies
	modulate = _ColorState.get_color(color_state_index, color_polarity, _ColorState.Variant.LIGHT)


func _on_area_entered(area: Area2D) -> void:
	if not active or _is_dying or _is_spawning:
		return

	# Hit by bullet
	if area.has_method("deactivate") and area.get("bullet_color") != null:
		take_hit(area.bullet_color)
		area.deactivate()
		return

	# Collided with player
	if area.has_method("take_damage"):
		area.take_damage()


# --- Virtual methods for subclasses ---

func _enemy_ready() -> void:
	pass

func _enemy_process(_delta: float) -> void:
	pass

func _on_activate() -> void:
	pass

func _on_hit() -> void:
	pass

func _on_death() -> void:
	pass

func _process_dying(_delta: float) -> void:
	pass
