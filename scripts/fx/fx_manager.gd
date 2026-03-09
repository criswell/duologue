extends Node2D
## Central effects manager. Creates and manages particle systems for
## enemy deaths, bullet hits, player explosions, and the throb vignette.

const ParticleSystemScript := preload("res://scripts/fx/particle_system.gd")

# Particle systems (children of this node)
var _enemy_explode: Node2D = null
var _bullet_hit: Node2D = null
var _player_explode: Node2D = null
var _player_smoke: Node2D = null
var _player_ring: Node2D = null

# Throb vignette
var _throb_corners: Array[Sprite2D] = []
var _throb_texture: Texture2D = null
var _throb_alpha: float = 0.0
var _throb_color := Color.WHITE


func _ready() -> void:
	add_to_group("fx_manager")
	# Enemy explosion particles
	_enemy_explode = _create_particle_system({
		"texture": preload("res://assets/sprites/effects/bullet-hit-01.png"),
		"min_speed": 10.0, "max_speed": 25.0,
		"min_lifetime": 0.5, "max_lifetime": 1.0,
		"min_scale": 1.0, "max_scale": 2.0,
		"deceleration": 1.5,
		"additive": true,
	})

	# Bullet hit sparks
	_bullet_hit = _create_particle_system({
		"texture": preload("res://assets/sprites/effects/bullet-hit-01.png"),
		"min_speed": 40.0, "max_speed": 50.0,
		"min_lifetime": 0.3, "max_lifetime": 0.6,
		"min_scale": 0.3, "max_scale": 0.8,
		"deceleration": 2.0,
		"additive": true,
	})

	# Player explosion
	_player_explode = _create_particle_system({
		"texture": preload("res://assets/sprites/player/player-explode.png"),
		"min_speed": 40.0, "max_speed": 80.0,
		"min_lifetime": 0.5, "max_lifetime": 1.0,
		"min_scale": 0.3, "max_scale": 1.0,
		"deceleration": 1.2,
		"additive": true,
	})

	# Player smoke
	_player_smoke = _create_particle_system({
		"texture": preload("res://assets/sprites/player/player-smoke.png"),
		"min_speed": 20.0, "max_speed": 60.0,
		"min_lifetime": 1.0, "max_lifetime": 2.5,
		"min_scale": 1.0, "max_scale": 2.0,
		"deceleration": 0.5,
		"additive": false,
	})

	# Player ring
	_player_ring = _create_particle_system({
		"texture": preload("res://assets/sprites/effects/explode-ring.png"),
		"min_speed": 5.0, "max_speed": 15.0,
		"min_lifetime": 1.0, "max_lifetime": 2.0,
		"min_scale": 0.1, "max_scale": 0.3,
		"min_rotation_speed": -0.4, "max_rotation_speed": 0.4,
		"deceleration": 0.3,
		"additive": true,
	})

	# Setup throb vignette
	_setup_throb()


func enemy_death(pos: Vector2, color: Color) -> void:
	_enemy_explode.emit_burst(pos, color, 12)


func bullet_hit(pos: Vector2, color: Color) -> void:
	_bullet_hit.emit_burst(pos, color, 3)


func player_death(pos: Vector2, color: Color) -> void:
	_player_explode.emit_burst(pos, color, 22)
	_player_smoke.emit_burst(pos, Color(0.6, 0.6, 0.6, 0.8), 15)
	_player_ring.emit_burst(pos, color, 4)


func set_throb_color(color: Color) -> void:
	_throb_color = color


func _process(_delta: float) -> void:
	_update_throb()


func _setup_throb() -> void:
	_throb_texture = preload("res://assets/sprites/backgrounds/throb-corner.png")
	var tex_size := _throb_texture.get_size()

	# Create 4 corner sprites
	var corners := [
		{"pos": Vector2.ZERO, "flip_h": false, "flip_v": false},                          # Top-left
		{"pos": Vector2(GameState.VIEWPORT_SIZE.x, 0), "flip_h": true, "flip_v": false},   # Top-right
		{"pos": Vector2(0, GameState.VIEWPORT_SIZE.y), "flip_h": false, "flip_v": true},    # Bottom-left
		{"pos": GameState.VIEWPORT_SIZE, "flip_h": true, "flip_v": true},                   # Bottom-right
	]

	for corner in corners:
		var spr := Sprite2D.new()
		spr.texture = _throb_texture
		spr.centered = false
		spr.flip_h = corner["flip_h"]
		spr.flip_v = corner["flip_v"]
		# Position: offset so sprite covers the corner
		if corner["flip_h"]:
			spr.position.x = corner["pos"].x - tex_size.x
		else:
			spr.position.x = corner["pos"].x
		if corner["flip_v"]:
			spr.position.y = corner["pos"].y - tex_size.y
		else:
			spr.position.y = corner["pos"].y

		spr.modulate = Color(1, 1, 1, 0)
		spr.z_index = 50
		# Additive blend
		var mat := CanvasItemMaterial.new()
		mat.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
		spr.material = mat

		add_child(spr)
		_throb_corners.append(spr)


func _update_throb() -> void:
	# Get beat percentage from audio manager
	var beat_pct := AudioManager.get_beat_score()
	var intensity := float(IntensityManager.current_intensity) / maxf(float(IntensityManager.max_intensity), 1.0)

	_throb_alpha = intensity * beat_pct * 0.5

	for spr in _throb_corners:
		spr.modulate = Color(_throb_color.r, _throb_color.g, _throb_color.b, _throb_alpha)


func _create_particle_system(config: Dictionary) -> Node2D:
	var sys: Node2D = ParticleSystemScript.new()
	sys.texture = config["texture"]
	sys.min_speed = config["min_speed"]
	sys.max_speed = config["max_speed"]
	sys.min_lifetime = config["min_lifetime"]
	sys.max_lifetime = config["max_lifetime"]
	sys.min_scale = config["min_scale"]
	sys.max_scale = config["max_scale"]
	sys.deceleration = config["deceleration"]
	sys.use_additive_blend = config.get("additive", true)
	if "min_rotation_speed" in config:
		sys.min_rotation_speed = config["min_rotation_speed"]
		sys.max_rotation_speed = config["max_rotation_speed"]
	add_child(sys)
	return sys
