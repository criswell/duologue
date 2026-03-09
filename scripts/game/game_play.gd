extends Node2D
## Main gameplay scene. Coordinates player, enemies, waves, and HUD.

@onready var score_label: Label = $HUD/ScoreLabel
@onready var lives_label: Label = $HUD/LivesLabel
@onready var wave_label: Label = $HUD/WaveLabel
@onready var entity_layer: Node2D = $EntityLayer
@onready var effects_layer: Node2D = $EffectsLayer
@onready var background: ParallaxBackground = $Background
@onready var bg_sprite: Sprite2D = $Background/BGLayer/BGSprite

var _fx_manager: Node2D = null
var _pause_overlay: ColorRect = null

const GAMEPLAY_SONGS: Array[AudioManager.SongID] = [
	AudioManager.SongID.SECOND_CHANCE,
	AudioManager.SongID.DANCE_8THS,
	AudioManager.SongID.ULTRAFIX,
	AudioManager.SongID.LAND_OF_SAND_16THS,
	AudioManager.SongID.WIN_ONE,
	AudioManager.SongID.SUPERBOWL,
]

const _ColorState := preload("res://scripts/color_state.gd")

const WAVE_INTRO_DURATION := 2.0
const WAVE_DELAY_DURATION := 1.0
const CLOUD_SCROLL_SPEED := 30.0

var _bg_textures: Array[Texture2D] = [
	preload("res://assets/sprites/backgrounds/background-01.png"),
	preload("res://assets/sprites/backgrounds/background-02.png"),
	preload("res://assets/sprites/backgrounds/background-03.png"),
	preload("res://assets/sprites/backgrounds/background-04.png"),
	preload("res://assets/sprites/backgrounds/background-05.png"),
]

enum Phase { WAVE_INTRO, PLAYING, WAVE_CLEAR, GAME_OVER }

var _phase := Phase.WAVE_INTRO
var _phase_timer: float = 0.0
var _current_song_index: int = 0

var _player_scene: PackedScene = preload("res://scenes/game/player.tscn")
var _player: Area2D = null

# Enemy scenes
var _enemy_scenes: Dictionary = {
	"buzzsaw": preload("res://scenes/enemies/buzzsaw.tscn"),
	"gloop": preload("res://scenes/enemies/gloop.tscn"),
	"wiggles": preload("res://scenes/enemies/wiggles.tscn"),
	"ember": preload("res://scenes/enemies/ember.tscn"),
	"annmoeba": preload("res://scenes/enemies/annmoeba.tscn"),
}

# Enemy pool
var _enemy_pool: Dictionary = {}  # type_name -> Array of enemy nodes
var _active_enemies: Array[Area2D] = []

# Wave system
var _wave_manager: Node = null
var _wavelet_enemies: Array[Dictionary] = []  # Enemies waiting to spawn
var _wavelet_spawn_timer: float = 0.0
var _wavelet_spawned_count: int = 0


func _ready() -> void:
	GameState.score_changed.connect(_on_score_changed)
	GameState.player_died.connect(_on_player_died)
	GameState.pause_toggled.connect(_on_pause_toggled)
	GameState.current_play_state = GameState.PlayState.PLAYING

	# FX manager
	_fx_manager = preload("res://scripts/fx/fx_manager.gd").new()
	effects_layer.add_child(_fx_manager)

	# Spawn player
	_player = _player_scene.instantiate()
	entity_layer.add_child(_player)

	# Pre-pool enemies
	for type_name in _enemy_scenes:
		_enemy_pool[type_name] = []
		for i in range(15):
			var enemy: Area2D = _enemy_scenes[type_name].instantiate()
			_enemy_pool[type_name].append(enemy)

	# Wave manager
	_wave_manager = preload("res://scripts/game/wave_manager.gd").new()
	add_child(_wave_manager)

	# Pause overlay
	_pause_overlay = ColorRect.new()
	_pause_overlay.color = Color(0.0, 0.0, 0.0, 0.6)
	_pause_overlay.set_anchors_preset(Control.PRESET_FULL_RECT)
	_pause_overlay.visible = false
	_pause_overlay.mouse_filter = Control.MOUSE_FILTER_IGNORE
	var pause_label := Label.new()
	pause_label.text = "PAUSED"
	var pause_font := load("res://assets/fonts/inero.ttf") as Font
	pause_label.add_theme_font_override("font", pause_font)
	pause_label.add_theme_font_size_override("font_size", 48)
	pause_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	pause_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
	pause_label.set_anchors_preset(Control.PRESET_FULL_RECT)
	_pause_overlay.add_child(pause_label)
	_pause_overlay.process_mode = Node.PROCESS_MODE_ALWAYS
	$HUD.add_child(_pause_overlay)

	IntensityManager.reset()
	_current_song_index = randi() % GAMEPLAY_SONGS.size()
	AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])

	# Start first wave
	_wave_manager.generate_first_wave()
	GameState.current_wave = 1
	_start_wave_intro()


func _process(delta: float) -> void:
	# Scroll clouds
	background.scroll_offset.x += CLOUD_SCROLL_SPEED * delta

	match _phase:
		Phase.WAVE_INTRO:
			_process_wave_intro(delta)
		Phase.PLAYING:
			_process_playing(delta)
		Phase.WAVE_CLEAR:
			_process_wave_clear(delta)
		Phase.GAME_OVER:
			pass

	_update_hud()


func _process_wave_intro(delta: float) -> void:
	_phase_timer += delta
	if _phase_timer >= WAVE_INTRO_DURATION:
		_phase = Phase.PLAYING
		_start_wavelet()


func _process_playing(delta: float) -> void:
	# Spawn queued enemies
	_wavelet_spawn_timer += delta
	_spawn_queued_enemies()

	# Clean up dead enemies
	_active_enemies = _active_enemies.filter(func(e: Area2D) -> bool: return e.active)

	# Check if wavelet is complete (all spawned and all dead)
	if _wavelet_spawned_count >= _wavelet_enemies.size() and _active_enemies.size() == 0:
		# Try next wavelet
		if not _wave_manager.advance_wavelet():
			# Wave complete
			_phase = Phase.WAVE_CLEAR
			_phase_timer = 0.0


func _process_wave_clear(delta: float) -> void:
	_phase_timer += delta
	if _phase_timer >= WAVE_DELAY_DURATION:
		_wave_manager.advance_wave()
		GameState.current_wave = _wave_manager.major_wave
		_apply_color_state()
		_maybe_change_song()
		_start_wave_intro()


func _start_wave_intro() -> void:
	_phase = Phase.WAVE_INTRO
	_phase_timer = 0.0


func _start_wavelet() -> void:
	var wavelet: Dictionary = _wave_manager.get_current_wavelet()
	if wavelet.is_empty():
		return
	_wavelet_enemies = wavelet["enemies"]
	_wavelet_spawned_count = 0
	_wavelet_spawn_timer = 0.0


func _spawn_queued_enemies() -> void:
	while _wavelet_spawned_count < _wavelet_enemies.size():
		var enemy_def: Dictionary = _wavelet_enemies[_wavelet_spawned_count]
		if _wavelet_spawn_timer < enemy_def["spawn_delay"]:
			break  # Not time yet for this enemy

		var enemy := _get_pooled_enemy(enemy_def["type"])
		if not enemy:
			break  # Pool exhausted

		var pos := _random_edge_position()
		enemy.activate(pos, _wave_manager.current_color_state, enemy_def["polarity"], enemy_def["hp"], 0.5)
		_active_enemies.append(enemy)
		_wavelet_spawned_count += 1


func _get_pooled_enemy(type_name: String) -> Area2D:
	if type_name not in _enemy_pool:
		return null
	var pool: Array = _enemy_pool[type_name]
	for enemy in pool:
		if not enemy.active:
			if not enemy.is_inside_tree():
				entity_layer.add_child(enemy)
			return enemy
	# Pool exhausted — create one more
	var new_enemy: Area2D = _enemy_scenes[type_name].instantiate()
	pool.append(new_enemy)
	entity_layer.add_child(new_enemy)
	return new_enemy


func _random_edge_position() -> Vector2:
	var vp := GameState.VIEWPORT_SIZE
	var margin := 30.0
	match randi() % 4:
		0: return Vector2(randf_range(0, vp.x), -margin)
		1: return Vector2(randf_range(0, vp.x), vp.y + margin)
		2: return Vector2(-margin, randf_range(0, vp.y))
		_: return Vector2(vp.x + margin, randf_range(0, vp.y))


func _apply_color_state() -> void:
	if _player:
		_player.color_state_index = _wave_manager.current_color_state
		_player._update_colors()
	# Cycle background image
	var bg_index: int = (_wave_manager.major_wave - 1) % _bg_textures.size()
	bg_sprite.texture = _bg_textures[bg_index]
	# Update throb color to match current color state
	if _fx_manager:
		var throb_color := _ColorState.get_color(
			_wave_manager.current_color_state,
			_ColorState.Polarity.POSITIVE,
			_ColorState.Variant.LIGHT
		)
		_fx_manager.set_throb_color(throb_color)


func _maybe_change_song() -> void:
	# Change song every 3 major waves
	if _wave_manager.minor_wave == 1 and _wave_manager.major_wave % 3 == 1 and _wave_manager.major_wave > 1:
		_current_song_index = (_current_song_index + 1) % GAMEPLAY_SONGS.size()
		IntensityManager.reset()
		AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])


func _update_hud() -> void:
	score_label.text = "Score: %d" % GameState.scores[0]
	lives_label.text = "Lives: %d" % GameState.lives[0]

	match _phase:
		Phase.WAVE_INTRO:
			wave_label.text = "Wave %s\nGet Ready!" % _wave_manager.get_wave_label()
		Phase.PLAYING:
			wave_label.text = "Wave %s | Enemies: %d" % [
				_wave_manager.get_wave_label(), _active_enemies.size()
			]
		Phase.WAVE_CLEAR:
			wave_label.text = "Wave %s Cleared!" % _wave_manager.get_wave_label()
		Phase.GAME_OVER:
			wave_label.text = "GAME OVER\nFinal Score: %d\nPress ENTER to restart" % GameState.scores[0]


func _exit_tree() -> void:
	# Free pooled enemies not in the scene tree to avoid RID leaks.
	# Enemies that ARE in the tree will be freed by the tree itself.
	for type_name in _enemy_pool:
		for enemy in _enemy_pool[type_name]:
			if is_instance_valid(enemy) and not enemy.is_inside_tree():
				enemy.free()
	_enemy_pool.clear()


func _on_score_changed(player_index: int, _new_score: int) -> void:
	if player_index == 0:
		_update_hud()


func _on_player_died(player_index: int) -> void:
	if player_index == 0:
		_update_hud()
		if GameState.lives[0] <= 0:
			_phase = Phase.GAME_OVER
			# Deactivate all enemies
			for enemy in _active_enemies:
				if enemy.active:
					enemy.deactivate()
			_active_enemies.clear()
			# Transition to game over screen after a brief delay
			get_tree().create_timer(2.0).timeout.connect(func() -> void:
				GameState.current_state = GameState.State.GAME_OVER
			)


func _on_pause_toggled(paused: bool) -> void:
	_pause_overlay.visible = paused


func _input(event: InputEvent) -> void:

	# --- Temporary testing controls ---
	if event is InputEventKey and event.pressed:
		match event.keycode:
			KEY_UP:
				IntensityManager.intensify()
				IntensityManager.intensify()
			KEY_DOWN:
				IntensityManager.detensify()
			KEY_RIGHT:
				_current_song_index = (_current_song_index + 1) % GAMEPLAY_SONGS.size()
				IntensityManager.reset()
				AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])
			KEY_LEFT:
				_current_song_index = (_current_song_index - 1 + GAMEPLAY_SONGS.size()) % GAMEPLAY_SONGS.size()
				IntensityManager.reset()
				AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])
