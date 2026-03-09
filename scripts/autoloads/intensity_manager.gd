extends Node
## Manages the game's intensity level which drives the dynamic music system.
## Replaces IntensityNotifier from the XNA original.
## Intensity ranges from 0 to max_intensity (varies per song).

signal intensity_changed(old_level: int, new_level: int)

const DECAY_TIME := 6.0       # Seconds with no kills before intensity drops
const KILLS_TO_INCREASE := 2  # Kills needed to bump intensity

var current_intensity: int = 0:
	set(value):
		var old := current_intensity
		current_intensity = clampi(value, 0, max_intensity)
		if old != current_intensity:
			intensity_changed.emit(old, current_intensity)

## Max intensity is set dynamically based on the current song's intensity map.
var max_intensity: int = 7

var _kill_counter: int = 0
var _decay_timer: float = 0.0


func _process(delta: float) -> void:
	if GameState.current_state != GameState.State.GAMEPLAY:
		return
	if GameState.current_play_state != GameState.PlayState.PLAYING:
		return

	_decay_timer += delta
	if _decay_timer >= DECAY_TIME:
		_decay_timer = 0.0
		_kill_counter = 0
		detensify()


func intensify() -> void:
	_kill_counter += 1
	_decay_timer = 0.0
	if _kill_counter >= KILLS_TO_INCREASE:
		_kill_counter = 0
		current_intensity += 1


func register_kill() -> void:
	intensify()


func detensify() -> void:
	current_intensity -= 1


func reset() -> void:
	current_intensity = 0
	_kill_counter = 0
	_decay_timer = 0.0


## Called by AudioManager when a new song starts to set the correct max.
func set_max_from_song(song_def: Dictionary) -> void:
	if "intensity_map" in song_def:
		max_intensity = song_def["intensity_map"].size() - 1
	else:
		max_intensity = 0
