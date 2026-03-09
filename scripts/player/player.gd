extends Area2D
## Player controller. Dual-input: WASD+mouse or gamepad twin-stick.
## Multi-layer sprite rendering with color-based combat.
##
## Original constants (at 60fps):
##   Movement: 4 px/frame = 240 px/s
##   Fire rate: 0.15s
##   Bullet speed: 10 px/frame = 600 px/s
##   Death duration: 4.0s
##   Spawn invincibility: ~2.5s (10 blinks × 0.25s)

const _ColorState := preload("res://scripts/color_state.gd")

const MOVE_SPEED := 240.0
const FIRE_COOLDOWN := 0.15
const COLLISION_RADIUS := 30.0
const DEATH_DURATION := 4.0
const BLINK_COUNT := 10
const BLINK_INTERVAL := 0.25
const CANNON_ROTATION_OFFSET := PI / 2.0
const LIGHT_ROTATION_OFFSET := 3.0 * PI / 4.0
const TREAD_ANIM_INTERVAL := 0.1
const MAX_BULLETS := 10

enum PlayerState { SPAWNING, GETTING_READY, ALIVE, DYING, DEAD }

var state := PlayerState.SPAWNING
var color_state_index: int = 0
var light_polarity: int = _ColorState.Polarity.POSITIVE
var aim_direction := Vector2.RIGHT
var move_direction := Vector2.ZERO
var player_index: int = 0

var _fire_timer: float = 0.0
var _death_timer: float = 0.0
var _blink_timer: float = 0.0
var _blink_count: int = 0
var _spawn_scale: float = 5.0
var _tread_timer: float = 0.0
var _tread_frame: int = 0
var _bullets: Array[Area2D] = []

@onready var base_sprite: Sprite2D = $BaseSprite
@onready var cannon_sprite: Sprite2D = $CannonSprite
@onready var light_sprite: Sprite2D = $LightSprite
@onready var tread_sprite: Sprite2D = $TreadSprite
@onready var collision_shape: CollisionShape2D = $CollisionShape2D

var _bullet_scene: PackedScene = preload("res://scenes/game/bullet.tscn")
var _tread_textures: Array[Texture2D] = [
	preload("res://assets/sprites/player/tread01.png"),
	preload("res://assets/sprites/player/tread02.png"),
]


func _ready() -> void:
	# Collision: layer 1 (player), detect layer 2 (enemies)
	collision_layer = 1
	collision_mask = 2

	var shape := CircleShape2D.new()
	shape.radius = COLLISION_RADIUS
	collision_shape.shape = shape

	add_to_group("player")
	area_entered.connect(_on_area_entered)

	# Pre-pool bullets
	for i in range(MAX_BULLETS):
		var bullet: Area2D = _bullet_scene.instantiate()
		_bullets.append(bullet)

	_start_spawn()
	_update_colors()


func _process(delta: float) -> void:
	match state:
		PlayerState.SPAWNING:
			_process_spawning(delta)
		PlayerState.GETTING_READY:
			_process_getting_ready(delta)
		PlayerState.ALIVE:
			_process_alive(delta)
		PlayerState.DYING:
			_process_dying(delta)


func _process_spawning(delta: float) -> void:
	_spawn_scale -= 6.0 * delta  # ~50 frames at 60fps → about 0.83s
	rotation += 0.6 * delta  # Slow spin during spawn
	scale = Vector2.ONE * _spawn_scale
	if _spawn_scale <= 1.0:
		scale = Vector2.ONE
		rotation = 0.0
		state = PlayerState.GETTING_READY
		_blink_count = 0
		_blink_timer = 0.0


func _process_getting_ready(delta: float) -> void:
	_handle_movement(delta)
	_handle_aim()
	_blink_timer += delta
	if _blink_timer >= BLINK_INTERVAL:
		_blink_timer -= BLINK_INTERVAL
		_blink_count += 1
		visible = !visible
	if _blink_count >= BLINK_COUNT * 2:
		visible = true
		state = PlayerState.ALIVE


func _process_alive(delta: float) -> void:
	_handle_movement(delta)
	_handle_aim()
	_handle_fire(delta)


func _process_dying(delta: float) -> void:
	_death_timer += delta
	# Fade out after halfway through death
	if _death_timer > DEATH_DURATION / 2.0:
		var fade := 1.0 - ((_death_timer - DEATH_DURATION / 2.0) / (DEATH_DURATION / 2.0))
		modulate.a = clampf(fade, 0.0, 1.0)
	if _death_timer >= DEATH_DURATION:
		state = PlayerState.DEAD
		visible = false
		GameState.lose_life(player_index)
		if GameState.lives[player_index] > 0:
			_start_spawn()


func _handle_movement(delta: float) -> void:
	var input := Vector2.ZERO

	if GameState.input_mode == GameState.InputMode.KEYBOARD_MOUSE:
		input = Input.get_vector("move_left", "move_right", "move_up", "move_down")
	else:
		input = Input.get_vector("move_left", "move_right", "move_up", "move_down")

	if input.length_squared() > 0.01:
		move_direction = input.normalized()
		# Animate treads while moving
		_tread_timer += delta
		if _tread_timer >= TREAD_ANIM_INTERVAL:
			_tread_timer -= TREAD_ANIM_INTERVAL
			_tread_frame = (_tread_frame + 1) % _tread_textures.size()
			tread_sprite.texture = _tread_textures[_tread_frame]

	position += input * MOVE_SPEED * delta

	# Clamp to screen boundaries
	var half := Vector2(32, 31)  # Half of 64x62 sprite
	position = position.clamp(half, GameState.VIEWPORT_SIZE - half)


func _handle_aim() -> void:
	if GameState.input_mode == GameState.InputMode.KEYBOARD_MOUSE:
		# Aim toward mouse position
		var mouse_pos := get_global_mouse_position()
		var dir := mouse_pos - global_position
		if dir.length_squared() > 4.0:
			aim_direction = dir.normalized()
	else:
		# Gamepad right stick
		var stick := Vector2(
			Input.get_axis("aim_left", "aim_right"),
			Input.get_axis("aim_up", "aim_down")
		)
		if stick.length_squared() > 0.04:
			aim_direction = stick.normalized()

	# Update sprite rotations
	if move_direction.length_squared() > 0.01:
		base_sprite.rotation = move_direction.angle()
	tread_sprite.rotation = base_sprite.rotation + PI / 2.0
	cannon_sprite.rotation = aim_direction.angle() + CANNON_ROTATION_OFFSET
	light_sprite.rotation = base_sprite.rotation + LIGHT_ROTATION_OFFSET


func _handle_fire(delta: float) -> void:
	_fire_timer -= delta

	var wants_to_fire := false
	if GameState.input_mode == GameState.InputMode.KEYBOARD_MOUSE:
		wants_to_fire = Input.is_action_pressed("shoot")
	else:
		# Gamepad: any right stick input fires (original behavior)
		var stick := Vector2(
			Input.get_axis("aim_left", "aim_right"),
			Input.get_axis("aim_up", "aim_down")
		)
		wants_to_fire = stick.length_squared() > 0.04

	if wants_to_fire and _fire_timer <= 0.0:
		_fire_timer = FIRE_COOLDOWN
		_fire_bullet()


func _fire_bullet() -> void:
	# Find an inactive bullet
	for bullet in _bullets:
		if not bullet.active:
			# Add to scene if not already
			if not bullet.is_inside_tree():
				get_parent().add_child(bullet)
			var cannon_polarity := _ColorState.opposite_polarity(light_polarity)
			var color := _ColorState.get_color(
				color_state_index, cannon_polarity, _ColorState.Variant.LIGHT
			)
			var spawn_pos := global_position + aim_direction * COLLISION_RADIUS
			bullet.fire(spawn_pos, aim_direction, color)
			return


func _update_colors() -> void:
	# Base body: light color of current polarity
	base_sprite.modulate = _ColorState.get_color(
		color_state_index, light_polarity, _ColorState.Variant.LIGHT
	)
	# Cannon: dark color of OPPOSITE polarity
	var cannon_polarity := _ColorState.opposite_polarity(light_polarity)
	cannon_sprite.modulate = _ColorState.get_color(
		color_state_index, cannon_polarity, _ColorState.Variant.DARK
	)
	# Light: medium color of current polarity
	light_sprite.modulate = _ColorState.get_color(
		color_state_index, light_polarity, _ColorState.Variant.MEDIUM
	)


func _start_spawn() -> void:
	state = PlayerState.SPAWNING
	_spawn_scale = 5.0
	_death_timer = 0.0
	modulate.a = 1.0
	visible = true
	position = GameState.VIEWPORT_SIZE / 2.0
	collision_shape.set_deferred("disabled", false)


func take_damage() -> void:
	if state != PlayerState.ALIVE:
		return
	state = PlayerState.DYING
	_death_timer = 0.0
	# Disable collision during death
	collision_shape.set_deferred("disabled", true)
	# Trigger death effects
	var fx_nodes := get_tree().get_nodes_in_group("fx_manager")
	if fx_nodes.size() > 0:
		var color := _ColorState.get_color(
			color_state_index, light_polarity, _ColorState.Variant.LIGHT
		)
		fx_nodes[0].player_death(global_position, color)


func swap_polarity() -> void:
	light_polarity = _ColorState.opposite_polarity(light_polarity)
	_update_colors()


func _exit_tree() -> void:
	# Free pooled bullets not in the scene tree to avoid RID leaks.
	# Bullets that ARE in the tree will be freed by the tree itself.
	for bullet in _bullets:
		if is_instance_valid(bullet) and not bullet.is_inside_tree():
			bullet.free()
	_bullets.clear()


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("color_swap"):
		swap_polarity()


func _on_area_entered(_area: Area2D) -> void:
	# Phase 4: enemy collision → take_damage()
	pass
