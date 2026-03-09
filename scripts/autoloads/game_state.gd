extends Node
## Global game state manager.
## Replaces LocalInstanceManager from the XNA original.
## Tracks current screen/state, player data, and game-wide settings.

# Mirrors the original GameState enum, trimmed for prototype scope.
enum State {
	NONE,
	COMPANY_INTRO,
	MAIN_MENU,
	PLAYER_SELECT,
	GAMEPLAY,
	GAME_OVER,
	CREDITS,
	EXIT,
}

# Mirrors GamePlayState from GamePlayScreenManager.cs
enum PlayState {
	WAVE_INTRO,
	SPAWNING,
	PLAYING,
	LOAD_NEXT_WAVE,
	DELAY,
	GAME_OVER,
}

signal state_changed(old_state: State, new_state: State)
signal play_state_changed(old_state: PlayState, new_state: PlayState)
signal player_died(player_index: int)
signal score_changed(player_index: int, new_score: int)

const MAX_PLAYERS := 4
const MAX_BULLETS_PER_PLAYER := 10
const MAX_ENEMIES := 100
const VIEWPORT_SIZE := Vector2(1280, 720)
const EXTRA_LIFE_SCORE := 3000
const MAX_SCORE := 199999

var current_state: State = State.NONE:
	set(value):
		var old := current_state
		current_state = value
		last_state = old
		state_changed.emit(old, value)

var last_state: State = State.NONE

var current_play_state: PlayState = PlayState.WAVE_INTRO:
	set(value):
		var old := current_play_state
		current_play_state = value
		play_state_changed.emit(old, value)

# Per-player data. Index 0 is always the local player in single-player.
var scores: Array[int] = [0, 0, 0, 0]
var lives: Array[int] = [3, 3, 3, 3]
var active_players: int = 1

# Wave tracking
var current_wave: int = 0
var is_paused: bool = false

# Input mode detection
enum InputMode { KEYBOARD_MOUSE, GAMEPAD }
var input_mode: InputMode = InputMode.KEYBOARD_MOUSE


func _ready() -> void:
	process_mode = Node.PROCESS_MODE_ALWAYS


func _input(event: InputEvent) -> void:
	# Auto-detect input mode for UI hints and aim behavior
	if event is InputEventKey or event is InputEventMouseMotion or event is InputEventMouseButton:
		if input_mode != InputMode.KEYBOARD_MOUSE:
			input_mode = InputMode.KEYBOARD_MOUSE
	elif event is InputEventJoypadButton or event is InputEventJoypadMotion:
		if input_mode != InputMode.GAMEPAD:
			input_mode = InputMode.GAMEPAD


func reset_game() -> void:
	scores = [0, 0, 0, 0]
	lives = [3, 3, 3, 3]
	current_wave = 0
	current_play_state = PlayState.WAVE_INTRO
	is_paused = false


func add_score(player_index: int, points: int) -> void:
	var old_score := scores[player_index]
	scores[player_index] = mini(scores[player_index] + points, MAX_SCORE)
	score_changed.emit(player_index, scores[player_index])

	# Extra life check (every EXTRA_LIFE_SCORE points)
	var old_lives_earned := old_score / EXTRA_LIFE_SCORE
	var new_lives_earned := scores[player_index] / EXTRA_LIFE_SCORE
	if new_lives_earned > old_lives_earned:
		lives[player_index] += (new_lives_earned - old_lives_earned)


func lose_life(player_index: int) -> void:
	lives[player_index] -= 1
	if lives[player_index] <= 0:
		player_died.emit(player_index)
