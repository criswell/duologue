extends "res://scripts/enemies/enemy_base.gd"
## Gloop enemy. Blobby follower that chases the player with random scaling.
## Original: 83x83, scale 0.5-1.0, highlight pulses, death rotation.

const CHASE_SPEED := 90.0
const HIGHLIGHT_PULSE_SPEED := 4.0

var _gloop_scale: float = 1.0
var _highlight_phase: float = 0.0
var _death_rotation_speed: float = 0.0

@onready var base_sprite: Sprite2D = $BaseSprite
@onready var highlight_sprite: Sprite2D = $HighlightSprite


func _enemy_ready() -> void:
	collision_radius = 30.0
	point_value = 25
	point_bonus_per_hp = 3


func _on_activate() -> void:
	_gloop_scale = randf_range(0.5, 1.0)
	scale = Vector2.ONE * _gloop_scale
	collision_radius = 30.0 * _gloop_scale
	_highlight_phase = randf() * TAU
	_death_rotation_speed = randf_range(5.0, 10.0) * (1.0 if randf() > 0.5 else -1.0)
	_update_colors()


func _enemy_process(delta: float) -> void:
	_move_toward_player(delta, CHASE_SPEED)

	# Pulse highlight
	_highlight_phase += HIGHLIGHT_PULSE_SPEED * delta
	var alpha := 0.08 + 0.88 * (0.5 + 0.5 * sin(_highlight_phase))
	highlight_sprite.modulate.a = alpha


func _update_colors() -> void:
	base_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.LIGHT
	)
	highlight_sprite.modulate = Color.WHITE


func _process_dying(delta: float) -> void:
	rotation += _death_rotation_speed * delta
	scale = Vector2.ONE * _gloop_scale * (1.0 - _death_timer / _death_duration)
