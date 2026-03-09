extends Node
## Central audio manager. Handles both managed (looping) and beat-synced
## (snippet-stitching) songs with intensity-driven track layering.
## Ported from Song.cs, BeatWidget.cs, AudioHelper.cs.

const _MusicFactory := preload("res://scripts/audio/music_factory.gd")

enum SongID {
	SELECT_MENU,
	DANCE_8THS,
	LAND_OF_SAND_16THS,
	CREDITS,
	ULTRAFIX,
	WIN_ONE,
	SECOND_CHANCE,
	SUPERBOWL_INTRO,
	SUPERBOWL,
	TR8OR,
}

signal beat_hit(beat_index: int, beat_count: int)

var _current_song_id: SongID = SongID.SELECT_MENU
var _song_def: Dictionary = {}
var _is_playing: bool = false
var _is_managed: bool = false

# Beat tracking (replaces BeatWidget)
var _beat_timer: float = 0.0
var _beat_length_ms: float = 0.0
const MUSIC_VOLUME_DB := -2.5  # ~75% volume
var _current_beat: int = 0
var _beat_count: int = 0

# Track state
var _track_players: Array[AudioStreamPlayer] = []
var _track_enabled: Array[bool] = []
var _track_streams: Array[Array] = []  # [track_idx][beat_idx] = AudioStream

# Managed song player
var _managed_player: AudioStreamPlayer = null

# Preloaded streams cache
var _stream_cache: Dictionary = {}  # path -> AudioStream


func _ready() -> void:
	_setup_buses()
	IntensityManager.intensity_changed.connect(_on_intensity_changed)


func _process(delta: float) -> void:
	if not _is_playing or _is_managed:
		return

	_beat_timer += delta * 1000.0  # Convert to milliseconds

	if _beat_timer >= _beat_length_ms:
		_beat_timer -= _beat_length_ms  # Preserve remainder for accuracy
		_advance_beat()


func _setup_buses() -> void:
	# Create buses: Music, SFX, UI under Master
	for bus_name in ["Music", "SFX", "UI"]:
		if AudioServer.get_bus_index(bus_name) == -1:
			var idx := AudioServer.bus_count
			AudioServer.add_bus(idx)
			AudioServer.set_bus_name(idx, bus_name)
			AudioServer.set_bus_send(idx, "Master")

	# Create track sub-buses under Music (max 7 tracks + 1 managed)
	var music_bus := "Music"
	for bus_suffix in ["Managed", "Track_0", "Track_1", "Track_2", "Track_3", "Track_4", "Track_5", "Track_6"]:
		if AudioServer.get_bus_index(bus_suffix) == -1:
			var idx := AudioServer.bus_count
			AudioServer.add_bus(idx)
			AudioServer.set_bus_name(idx, bus_suffix)
			AudioServer.set_bus_send(idx, music_bus)


func play_song(song_id: SongID) -> void:
	stop()
	_current_song_id = song_id
	_song_def = _MusicFactory.get_song(song_id)

	if _song_def.is_empty():
		push_error("No song definition for: %s" % SongID.keys()[song_id])
		return

	# Set intensity max for this song
	IntensityManager.set_max_from_song(_song_def)

	if _song_def["type"] == "managed":
		_play_managed()
	else:
		_play_beat()


func stop() -> void:
	_is_playing = false

	# Stop managed player
	if _managed_player:
		_managed_player.stop()
		_managed_player.queue_free()
		_managed_player = null

	# Stop and free track players
	for player in _track_players:
		if is_instance_valid(player):
			player.stop()
			player.queue_free()
	_track_players.clear()
	_track_enabled.clear()
	_track_streams.clear()
	_stream_cache.clear()

	_beat_timer = 0.0
	_current_beat = 0


func fade_in(song_id: SongID, duration_ms: float = 100.0) -> void:
	play_song(song_id)
	if _is_managed and _managed_player:
		# Start silent, tween to full volume
		var music_bus_idx := AudioServer.get_bus_index("Music")
		AudioServer.set_bus_volume_db(music_bus_idx, -80.0)
		var tween := create_tween()
		tween.tween_method(
			func(db: float): AudioServer.set_bus_volume_db(music_bus_idx, db),
			-80.0, 0.0, duration_ms / 1000.0
		)


func fade_out(duration_ms: float = 100.0) -> void:
	if not _is_playing:
		return
	var music_bus_idx := AudioServer.get_bus_index("Music")
	var current_db := AudioServer.get_bus_volume_db(music_bus_idx)
	var tween := create_tween()
	tween.tween_method(
		func(db: float): AudioServer.set_bus_volume_db(music_bus_idx, db),
		current_db, -80.0, duration_ms / 1000.0
	)
	tween.tween_callback(stop)


## Returns a score from 0.5 to 1.0 based on proximity to beat.
## 1.0 = exactly on beat, 0.5 = halfway between beats.
func get_beat_score() -> float:
	if _beat_length_ms <= 0.0:
		return 0.75
	return 0.75 + 0.25 * cos(TAU * (_beat_timer / _beat_length_ms))


# --- Private: Managed song playback ---

func _play_managed() -> void:
	var stream := _load_stream(_song_def["file"])
	if not stream:
		push_error("Failed to load managed song: %s" % _song_def["file"])
		return

	_is_managed = true

	var player := AudioStreamPlayer.new()
	player.bus = "Music"
	add_child(player)
	_track_players.append(player)
	_track_enabled.append(true)

	# Don't use AudioStreamWAV loop_mode — imported WAVs have loop_end=0
	# which creates a zero-length loop (silence). Use signal-based looping.
	player.stream = stream
	player.finished.connect(player.play)
	player.play()

	_managed_player = player
	_is_playing = true

	var music_bus_idx := AudioServer.get_bus_index("Music")
	AudioServer.set_bus_volume_db(music_bus_idx, MUSIC_VOLUME_DB)


# --- Private: Beat song playback ---

func _play_beat() -> void:
	_is_managed = false
	_beat_length_ms = _song_def["bpm"]
	var tracks: Array = _song_def["tracks"]
	_beat_count = 0

	# Preload all streams and create players
	for t_idx in range(tracks.size()):
		var track_def: Dictionary = tracks[t_idx]
		var cues: Array = track_def["cues"]
		_beat_count = maxi(_beat_count, cues.size())

		# Preload streams for this track
		var streams: Array = []
		for cue_path in cues:
			if cue_path == "":
				streams.append(null)
			else:
				streams.append(_load_stream(cue_path))
		_track_streams.append(streams)

		# Create AudioStreamPlayer for this track
		var player := AudioStreamPlayer.new()
		var bus_name := "Track_%d" % t_idx
		if AudioServer.get_bus_index(bus_name) != -1:
			player.bus = bus_name
		else:
			player.bus = "Music"
		add_child(player)
		_track_players.append(player)
		_track_enabled.append(false)

	# Apply initial intensity (level 0)
	_apply_intensity(0)

	# Reset beat state
	_beat_timer = 0.0
	_current_beat = 0
	_is_playing = true

	# Reset music bus volume
	var music_bus_idx := AudioServer.get_bus_index("Music")
	AudioServer.set_bus_volume_db(music_bus_idx, MUSIC_VOLUME_DB)

	# Play first beat immediately
	_play_current_beat()


func _advance_beat() -> void:
	_current_beat = (_current_beat + 1) % _beat_count
	_play_current_beat()
	beat_hit.emit(_current_beat, _beat_count)


func _play_current_beat() -> void:
	for t_idx in range(_track_players.size()):
		if not _track_enabled[t_idx]:
			continue
		if t_idx >= _track_streams.size():
			continue
		var streams: Array = _track_streams[t_idx]
		if _current_beat >= streams.size():
			continue
		var stream: AudioStream = streams[_current_beat]
		if stream == null:
			continue
		# Ensure snippet doesn't loop
		_set_stream_loop(stream, false)
		_track_players[t_idx].stream = stream
		_track_players[t_idx].play()


# --- Private: Intensity integration ---

func _on_intensity_changed(_old_level: int, new_level: int) -> void:
	if _is_playing and not _is_managed:
		_apply_intensity(new_level)


func _apply_intensity(level: int) -> void:
	if "intensity_map" not in _song_def:
		return
	var imap: Array = _song_def["intensity_map"]
	# Clamp to valid range
	var clamped := clampi(level, 0, imap.size() - 1)
	var track_states: Array = imap[clamped]
	for t_idx in range(mini(track_states.size(), _track_enabled.size())):
		_track_enabled[t_idx] = track_states[t_idx]


# --- Private: Stream loading ---

func _load_stream(path: String) -> AudioStream:
	if path in _stream_cache:
		return _stream_cache[path]
	if not ResourceLoader.exists(path):
		push_warning("Audio file not found: %s" % path)
		return null
	var stream := load(path) as AudioStream
	_stream_cache[path] = stream
	return stream


func _set_stream_loop(stream: AudioStream, enable: bool) -> void:
	if stream is AudioStreamWAV:
		stream.loop_mode = AudioStreamWAV.LOOP_FORWARD if enable else AudioStreamWAV.LOOP_DISABLED
