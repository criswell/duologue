extends Area2D
## Player bullet. Fires in a direction, despawns off-screen.
## Original: 10 px/frame @ 60fps = 600 px/s, circle collision from 24x24 sprite.

const SPEED := 600.0
const RADIUS := 12.0

var direction := Vector2.RIGHT
var bullet_color := Color.WHITE
var active := false


func _ready() -> void:
	# Set up collision
	var shape := CircleShape2D.new()
	shape.radius = RADIUS
	$CollisionShape2D.shape = shape

	# Layer 3 (bullets), detect layer 2 (enemies)
	collision_layer = 4   # bit 3
	collision_mask = 2    # bit 2

	area_entered.connect(_on_area_entered)
	set_process(false)
	visible = false


func fire(from: Vector2, dir: Vector2, color: Color) -> void:
	global_position = from
	direction = dir.normalized()
	bullet_color = color
	rotation = direction.angle()
	$BaseSprite.modulate = color
	active = true
	visible = true
	set_process(true)


func _process(delta: float) -> void:
	position += direction * SPEED * delta

	# Despawn when off-screen (with margin)
	var vp_size := GameState.VIEWPORT_SIZE
	var margin := 50.0
	if position.x < -margin or position.x > vp_size.x + margin \
		or position.y < -margin or position.y > vp_size.y + margin:
		deactivate()


func deactivate() -> void:
	active = false
	visible = false
	set_process(false)


func _on_area_entered(_area: Area2D) -> void:
	# Phase 4 will handle enemy hit logic here
	pass
