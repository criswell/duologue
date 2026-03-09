extends Node2D
## Main gameplay scene. Coordinates player, enemies, waves, and HUD.
## Phase 1 stub — sets up the scene structure and HUD updates.
## Phase 3+ will add player, enemies, and wave logic.

@onready var score_label: Label = $HUD/ScoreLabel
@onready var lives_label: Label = $HUD/LivesLabel
@onready var wave_label: Label = $HUD/WaveLabel


## Gameplay songs to cycle through. The original game selected based on wave/mode.
## For now, start with SecondChance (simplest beat+intensity song, good for testing).
const GAMEPLAY_SONGS: Array[AudioManager.SongID] = [
	AudioManager.SongID.SECOND_CHANCE,
	AudioManager.SongID.DANCE_8THS,
	AudioManager.SongID.ULTRAFIX,
	AudioManager.SongID.LAND_OF_SAND_16THS,
	AudioManager.SongID.WIN_ONE,
	AudioManager.SongID.SUPERBOWL,
]

## Temporary: keyboard controls for testing intensity and song switching.
## These will be removed once enemies drive intensity naturally.
var _current_song_index: int = 0


func _ready() -> void:
	GameState.score_changed.connect(_on_score_changed)
	GameState.player_died.connect(_on_player_died)
	GameState.current_play_state = GameState.PlayState.PLAYING
	_update_hud()
	# Start gameplay music
	IntensityManager.reset()
	AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])


func _update_hud() -> void:
	score_label.text = "Score: %d" % GameState.scores[0]
	lives_label.text = "Lives: %d" % GameState.lives[0]
	# Show song name and intensity for testing
	var song_names: Array[String] = ["SecondChance", "Dance8ths", "Ultrafix", "LandOfSand", "WinOne", "Superbowl"]
	var song_name: String = song_names[_current_song_index] if _current_song_index < song_names.size() else "?"
	wave_label.text = "%s | Intensity: %d/%d\n[Up/Down] intensity  [Left/Right] song" % [
		song_name, IntensityManager.current_intensity, IntensityManager.max_intensity
	]


func _on_score_changed(player_index: int, _new_score: int) -> void:
	if player_index == 0:
		_update_hud()


func _on_player_died(player_index: int) -> void:
	if player_index == 0:
		_update_hud()
		if GameState.lives[0] <= 0:
			GameState.current_state = GameState.State.GAME_OVER


func _input(event: InputEvent) -> void:
	if event.is_action_pressed("pause"):
		GameState.is_paused = !GameState.is_paused
		get_tree().paused = GameState.is_paused

	# --- Temporary testing controls (remove after Phase 4) ---
	if event is InputEventKey and event.pressed:
		match event.keycode:
			KEY_UP:
				# Increase intensity
				IntensityManager.intensify()
				IntensityManager.intensify()  # Two calls = guaranteed increase
				_update_hud()
			KEY_DOWN:
				# Decrease intensity
				IntensityManager.detensify()
				_update_hud()
			KEY_RIGHT:
				# Next song
				_current_song_index = (_current_song_index + 1) % GAMEPLAY_SONGS.size()
				IntensityManager.reset()
				AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])
				_update_hud()
			KEY_LEFT:
				# Previous song
				_current_song_index = (_current_song_index - 1 + GAMEPLAY_SONGS.size()) % GAMEPLAY_SONGS.size()
				IntensityManager.reset()
				AudioManager.play_song(GAMEPLAY_SONGS[_current_song_index])
				_update_hud()
