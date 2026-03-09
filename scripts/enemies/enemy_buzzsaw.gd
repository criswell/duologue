extends "res://scripts/enemies/enemy_base.gd"
## Buzzsaw enemy. Rotating blades that chase the player.
## Original: 85x87 sprite, HP 1-5, point value 25, attracted to player.

const CHASE_SPEED := 120.0
const BLADE_SPIN_SPEED := 8.0

@onready var base_sprite: Sprite2D = $BaseSprite
@onready var blades_sprite: Sprite2D = $BladesSprite
@onready var shine_sprite: Sprite2D = $ShineSprite


func _enemy_ready() -> void:
	collision_radius = 35.0
	point_value = 25
	point_bonus_per_hp = 3


func _on_activate() -> void:
	_update_colors()


func _enemy_process(delta: float) -> void:
	_move_toward_player(delta, CHASE_SPEED)

	# Spin blades
	blades_sprite.rotation += BLADE_SPIN_SPEED * delta

	# Face movement direction
	if move_velocity.length_squared() > 1.0:
		base_sprite.rotation = move_velocity.angle()

	# Shine alpha based on remaining HP
	shine_sprite.modulate.a = float(hit_points) / float(max_hit_points)


func _update_colors() -> void:
	base_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.MEDIUM
	)
	blades_sprite.modulate = _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.LIGHT
	)


func _process_dying(delta: float) -> void:
	blades_sprite.rotation += BLADE_SPIN_SPEED * 2.0 * delta
	scale = Vector2.ONE * (1.0 + _death_timer * 2.0)
