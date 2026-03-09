extends "res://scripts/enemies/enemy_base.gd"
## AnnMoeba enemy. Central bubble with 5 orbiting globules.
## Original: 90x87, globules orbit using phi angle, spawns over 1.8s.

const ORBIT_SPEED := 2.5
const ORBIT_RADIUS := 40.0
const CHASE_SPEED := 70.0
const GLOBULE_COUNT := 5
const THROB_SPEED := 2.0
const THROB_AMOUNT := 0.1

var _phi: float = 0.0
var _globule_sprites: Array[Sprite2D] = []

@onready var base_sprite: Sprite2D = $BaseSprite
@onready var highlight_sprite: Sprite2D = $HighlightSprite


func _enemy_ready() -> void:
	collision_radius = 35.0
	point_value = 25
	point_bonus_per_hp = 3

	# Create orbiting globule sprites
	var globule_tex: Texture2D = load("res://assets/sprites/enemies/gloop/glooplet.png")
	for i in range(GLOBULE_COUNT):
		var spr := Sprite2D.new()
		spr.texture = globule_tex
		spr.scale = Vector2.ONE * 0.3
		add_child(spr)
		_globule_sprites.append(spr)


func _on_activate() -> void:
	_phi = randf() * TAU
	scale = Vector2.ONE
	rotation = 0.0
	_update_colors()


func _enemy_process(delta: float) -> void:
	_move_toward_player(delta, CHASE_SPEED, 2.0)

	# Orbit globules
	_phi += ORBIT_SPEED * delta
	for i in range(_globule_sprites.size()):
		var angle := _phi + (TAU / GLOBULE_COUNT) * i
		var offset := Vector2(cos(angle), sin(angle)) * ORBIT_RADIUS
		_globule_sprites[i].position = offset

	# Throb scale
	var throb := 1.0 + THROB_AMOUNT * sin(_phi * THROB_SPEED)
	base_sprite.scale = Vector2.ONE * throb


func _update_colors() -> void:
	var light_color := _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.LIGHT
	)
	var med_color := _ColorState.get_color(
		color_state_index, color_polarity, _ColorState.Variant.MEDIUM
	)
	base_sprite.modulate = light_color
	for spr in _globule_sprites:
		spr.modulate = med_color


func _process_dying(delta: float) -> void:
	# Globules scatter outward
	for i in range(_globule_sprites.size()):
		var angle := _phi + (TAU / GLOBULE_COUNT) * i
		var dist := ORBIT_RADIUS + _death_timer * 200.0
		_globule_sprites[i].position = Vector2(cos(angle), sin(angle)) * dist
	scale = Vector2.ONE * maxf(1.0 - _death_timer / _death_duration, 0.1)
