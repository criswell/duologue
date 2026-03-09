extends Node
## Procedural wave generator. Survival-mode style with escalating difficulty.
## Generates waves with wavelets (sequential enemy groups).
##
## Wave structure: Major.Minor (e.g. 1-1, 1-2, 1-3, 2-1...)
## Color state changes every ~3 major waves.

const _ColorState := preload("res://scripts/color_state.gd")

signal wave_started(major: int, minor: int)
signal wavelet_started(wavelet_index: int)
signal wave_cleared()
signal all_enemies_spawned()

const MAX_MINOR := 3
const WAVES_PER_COLOR_CHANGE := 3
const MIN_ENEMIES := 4
const MAX_ENEMIES := 40
const MAX_WAVE_FOR_SCALING := 30  # Difficulty fully ramps by wave 30
const MIN_SPAWN_DELAY := 0.3
const MAX_SPAWN_DELAY := 2.0
const BASE_HP := 1

# Enemy tier unlocks by major wave
const ENEMY_TIERS := [
	["buzzsaw", "ember"],                                    # Tier 0: waves 1-3
	["buzzsaw", "ember", "wiggles"],                         # Tier 1: waves 4-7
	["buzzsaw", "ember", "wiggles", "gloop"],                # Tier 2: waves 8-12
	["buzzsaw", "ember", "wiggles", "gloop", "annmoeba"],    # Tier 3: waves 13+
]

var major_wave: int = 0
var minor_wave: int = 0
var current_color_state: int = 0
var _waves_since_color_change: int = 0

# Current wave data
var _wavelets: Array = []  # Array of wavelet dicts
var _current_wavelet_index: int = 0


func get_wave_label() -> String:
	return "%d-%d" % [major_wave, minor_wave]


func generate_first_wave() -> void:
	major_wave = 1
	minor_wave = 1
	current_color_state = 0
	_waves_since_color_change = 0
	_generate_wave()


func advance_wave() -> void:
	minor_wave += 1
	if minor_wave > MAX_MINOR:
		minor_wave = 1
		major_wave += 1

		_waves_since_color_change += 1
		if _waves_since_color_change >= WAVES_PER_COLOR_CHANGE:
			_waves_since_color_change = 0
			current_color_state = randi() % _ColorState.STATE_COUNT

	_generate_wave()


func get_current_wavelet() -> Dictionary:
	if _current_wavelet_index < _wavelets.size():
		return _wavelets[_current_wavelet_index]
	return {}


func advance_wavelet() -> bool:
	## Returns true if there's another wavelet, false if wave is complete.
	_current_wavelet_index += 1
	if _current_wavelet_index < _wavelets.size():
		wavelet_started.emit(_current_wavelet_index)
		return true
	return false


func _generate_wave() -> void:
	_wavelets.clear()
	_current_wavelet_index = 0

	var t := clampf(float(major_wave - 1) / float(MAX_WAVE_FOR_SCALING), 0.0, 1.0)

	# Number of wavelets: 1-3 based on minor wave
	var num_wavelets := mini(minor_wave, 3)

	# Total enemies for this wave, split across wavelets
	var total_enemies := int(lerpf(MIN_ENEMIES, MAX_ENEMIES, t)) + randi() % 3
	var enemies_per_wavelet := maxi(total_enemies / num_wavelets, 2)

	# Enemy types available at this wave
	var tier_index := clampi(int(t * ENEMY_TIERS.size()), 0, ENEMY_TIERS.size() - 1)
	var available_types: Array = ENEMY_TIERS[tier_index]

	# HP scaling
	var max_hp := BASE_HP + int(t * 3.0)

	for w in range(num_wavelets):
		var wavelet := {}
		var enemies: Array[Dictionary] = []
		var count := enemies_per_wavelet
		if w == num_wavelets - 1:
			# Last wavelet gets remaining enemies
			count = total_enemies - enemies_per_wavelet * w

		for i in range(count):
			var type_name: String = available_types[randi() % available_types.size()]
			var hp := randi() % max_hp + 1
			var polarity := randi() % 2
			var spawn_delay := lerpf(0.0, lerpf(MAX_SPAWN_DELAY, MIN_SPAWN_DELAY, t), float(i) / maxf(count - 1, 1))

			enemies.append({
				"type": type_name,
				"hp": hp,
				"polarity": polarity,
				"spawn_delay": spawn_delay,
			})

		wavelet["enemies"] = enemies
		_wavelets.append(wavelet)

	wave_started.emit(major_wave, minor_wave)
	wavelet_started.emit(0)
