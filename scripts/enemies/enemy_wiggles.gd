extends "res://scripts/enemies/enemy_base.gd"
## Wiggles enemy. Multi-frame walking creature that chases the player.
## Original: 82x90, 8 walk frames, 7 death frames, state machine.

const WALK_SPEED := 80.0
const RUN_SPEED := 160.0
const WALK_FRAME_TIME := 0.15
const RUN_FRAME_TIME := 0.05
const DETECT_RANGE := 300.0

enum WigglesState { WALKING, RUNNING, DYING }

var _state := WigglesState.WALKING
var _frame: int = 0
var _frame_timer: float = 0.0
var _wander_dir := Vector2.RIGHT

@onready var base_sprite: Sprite2D = $BaseSprite
@onready var outline_sprite: Sprite2D = $OutlineSprite

var _base_textures: Array[Texture2D] = []
var _outline_textures: Array[Texture2D] = []


func _enemy_ready() -> void:
	collision_radius = 35.0
	point_value = 50
	point_bonus_per_hp = 2
	_death_duration = 1.0

	# Load animation frames
	for i in range(1, 9):
		var idx := "%02d" % i
		_base_textures.append(load("res://assets/sprites/enemies/wiggles/%s-base.png" % idx))
		_outline_textures.append(load("res://assets/sprites/enemies/wiggles/%s-outline.png" % idx))


func _on_activate() -> void:
	_state = WigglesState.WALKING
	_frame = 0
	_frame_timer = 0.0
	_wander_dir = Vector2.from_angle(randf() * TAU)
	scale = Vector2.ONE
	rotation = 0.0
	_update_colors()
	_set_frame(0)


func _enemy_process(delta: float) -> void:
	var player := _get_player()
	var dist_to_player := INF
	if player:
		dist_to_player = global_position.distance_to(player.global_position)

	# State transitions
	if dist_to_player < DETECT_RANGE and _state == WigglesState.WALKING:
		_state = WigglesState.RUNNING

	var speed: float
	var frame_time: float

	match _state:
		WigglesState.WALKING:
			speed = WALK_SPEED
			frame_time = WALK_FRAME_TIME
			# Random wander, occasionally change direction
			if randf() < 0.01:
				_wander_dir = _wander_dir.rotated(randf_range(-1.0, 1.0))
			move_velocity = move_velocity.lerp(_wander_dir * speed, 2.0 * delta)
		WigglesState.RUNNING:
			speed = RUN_SPEED
			frame_time = RUN_FRAME_TIME
			if player:
				var dir := (player.global_position - global_position).normalized()
				move_velocity = move_velocity.lerp(dir * speed, 5.0 * delta)

	position += move_velocity * delta

	# Animate frames
	_frame_timer += delta
	if _frame_timer >= frame_time:
		_frame_timer -= frame_time
		_frame = (_frame + 1) % _base_textures.size()
		_set_frame(_frame)

	# Face movement direction
	if move_velocity.length_squared() > 1.0:
		var target_rot := move_velocity.angle()
		base_sprite.rotation = lerp_angle(base_sprite.rotation, target_rot, 8.0 * delta)
		outline_sprite.rotation = base_sprite.rotation


func _set_frame(idx: int) -> void:
	if idx < _base_textures.size():
		base_sprite.texture = _base_textures[idx]
		outline_sprite.texture = _outline_textures[idx]


func _update_colors() -> void:
	base_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.LIGHT
	)
	outline_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.DARK
	)


func _process_dying(delta: float) -> void:
	rotation += 5.0 * delta
	scale = Vector2.ONE * maxf(1.0 - _death_timer / _death_duration, 0.1)
