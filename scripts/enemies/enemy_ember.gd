extends "res://scripts/enemies/enemy_base.gd"
## Ember enemy. Simple single-sprite enemy that drifts toward the player.
## Slow but steady, low threat individually.

const DRIFT_SPEED := 60.0
const WOBBLE_SPEED := 3.0
const WOBBLE_AMOUNT := 0.15

var _wobble_phase: float = 0.0

@onready var base_sprite: Sprite2D = $BaseSprite


func _enemy_ready() -> void:
	collision_radius = 28.0
	point_value = 15
	point_bonus_per_hp = 2


func _on_activate() -> void:
	_wobble_phase = randf() * TAU
	scale = Vector2.ONE
	rotation = 0.0
	_update_colors()


func _enemy_process(delta: float) -> void:
	_move_toward_player(delta, DRIFT_SPEED, 2.0)

	# Wobble rotation
	_wobble_phase += WOBBLE_SPEED * delta
	base_sprite.rotation += WOBBLE_AMOUNT * sin(_wobble_phase) * delta


func _update_colors() -> void:
	base_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.MEDIUM
	)


func _process_dying(_delta: float) -> void:
	scale = Vector2.ONE * maxf(1.0 - _death_timer / _death_duration, 0.1)
