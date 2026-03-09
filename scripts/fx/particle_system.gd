extends Node2D
## Pooled particle system. Emits bursts of textured particles with physics.
## Supports additive and alpha blend modes.
##
## Usage: call emit_burst(position, color, count) to fire particles.

const MAX_PARTICLES := 200

var texture: Texture2D = null
var min_speed: float = 20.0
var max_speed: float = 50.0
var min_lifetime: float = 0.5
var max_lifetime: float = 1.0
var min_scale: float = 0.5
var max_scale: float = 1.0
var min_rotation_speed: float = -2.0
var max_rotation_speed: float = 2.0
var deceleration: float = 0.8  # Multiplied against velocity each second
var use_additive_blend: bool = true

# Particle pool
var _positions: PackedVector2Array
var _velocities: PackedVector2Array
var _lifetimes: PackedFloat32Array
var _max_lifetimes: PackedFloat32Array
var _rotations: PackedFloat32Array
var _rotation_speeds: PackedFloat32Array
var _scales: PackedFloat32Array
var _colors: PackedColorArray
var _alive: Array[bool] = []
var _active_count: int = 0


func _ready() -> void:
	_positions.resize(MAX_PARTICLES)
	_velocities.resize(MAX_PARTICLES)
	_lifetimes.resize(MAX_PARTICLES)
	_max_lifetimes.resize(MAX_PARTICLES)
	_rotations.resize(MAX_PARTICLES)
	_rotation_speeds.resize(MAX_PARTICLES)
	_scales.resize(MAX_PARTICLES)
	_colors.resize(MAX_PARTICLES)
	_alive.resize(MAX_PARTICLES)
	for i in range(MAX_PARTICLES):
		_alive[i] = false

	if use_additive_blend:
		# CanvasItem blend_mode: Add
		material = CanvasItemMaterial.new()
		(material as CanvasItemMaterial).blend_mode = CanvasItemMaterial.BLEND_MODE_ADD


func _process(delta: float) -> void:
	if _active_count <= 0:
		return

	var count := 0
	for i in range(MAX_PARTICLES):
		if not _alive[i]:
			continue
		_lifetimes[i] += delta
		if _lifetimes[i] >= _max_lifetimes[i]:
			_alive[i] = false
			continue

		count += 1
		# Physics
		_velocities[i] *= (1.0 - deceleration * delta)
		_positions[i] += _velocities[i] * delta
		_rotations[i] += _rotation_speeds[i] * delta

	_active_count = count
	queue_redraw()


func _draw() -> void:
	if not texture or _active_count <= 0:
		return

	var tex_size := texture.get_size()
	var half := tex_size * 0.5

	for i in range(MAX_PARTICLES):
		if not _alive[i]:
			continue

		var t := _lifetimes[i] / _max_lifetimes[i]
		# Alpha: fade in quickly, fade out smoothly
		var alpha := 4.0 * t * (1.0 - t)
		alpha = clampf(alpha, 0.0, 1.0)

		var col := _colors[i]
		col.a = alpha

		var s := _scales[i] * (0.75 + 0.25 * t)  # Grow slightly over lifetime

		var xform := Transform2D()
		xform = xform.rotated(_rotations[i])
		xform = xform.scaled(Vector2(s, s))
		xform.origin = _positions[i]

		draw_set_transform_matrix(xform)
		draw_texture(texture, -half, col)

	draw_set_transform_matrix(Transform2D.IDENTITY)


func emit_burst(pos: Vector2, color: Color, count: int = 10, direction: Vector2 = Vector2.ZERO) -> void:
	## Emit a burst of particles at the given world position.
	## If direction is non-zero, particles bias toward that direction.
	var spawned := 0
	for i in range(MAX_PARTICLES):
		if spawned >= count:
			break
		if _alive[i]:
			continue

		_alive[i] = true
		_positions[i] = pos
		_lifetimes[i] = 0.0
		_max_lifetimes[i] = randf_range(min_lifetime, max_lifetime)
		_rotations[i] = randf() * TAU
		_rotation_speeds[i] = randf_range(min_rotation_speed, max_rotation_speed)
		_scales[i] = randf_range(min_scale, max_scale)
		_colors[i] = color

		var speed := randf_range(min_speed, max_speed)
		var angle := randf() * TAU
		if direction.length_squared() > 0.01:
			# Bias toward direction with some spread
			angle = direction.angle() + randf_range(-PI / 3.0, PI / 3.0)
		_velocities[i] = Vector2.from_angle(angle) * speed

		spawned += 1
		_active_count += 1
