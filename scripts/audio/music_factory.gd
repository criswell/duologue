## Defines all song configurations.
## Ported from MusicFactory.cs — every song arrangement, intensity map, and
## track definition lives here.

# BPM constants from AudioConstants.cs
const BPM_120 := 500.0        # 1000 * 60 / 120
const BPM_140 := 429.130      # 3433.039 / 8.0 (measured from original)
const BPM_170 := 352.941      # 1000 * 60 / 170


static func get_song(song_id: int) -> Dictionary:
	match song_id:
		0: return _select_menu()       # SELECT_MENU
		1: return _dance_8ths()        # DANCE_8THS
		2: return _land_of_sand()      # LAND_OF_SAND_16THS
		3: return _credits()           # CREDITS
		4: return _ultrafix()          # ULTRAFIX
		5: return _win_one()           # WIN_ONE
		6: return _second_chance()     # SECOND_CHANCE
		7: return _superbowl_intro()   # SUPERBOWL_INTRO
		8: return _superbowl()         # SUPERBOWL
		9: return _tr8or()             # TR8OR
	return {}


# --- Managed (looping) songs ---

static func _select_menu() -> Dictionary:
	return {"type": "managed", "file": "res://assets/audio/music/SelectMenu.wav"}


static func _credits() -> Dictionary:
	return {"type": "managed", "file": "res://assets/audio/music/credits.wav"}


static func _tr8or() -> Dictionary:
	return {"type": "managed", "file": "res://assets/audio/music/tr8or.wav"}


static func _superbowl_intro() -> Dictionary:
	return {"type": "managed", "file": "res://assets/audio/music/Superintro/superbowlsix.wav"}


# --- Beat + Intensity songs ---

static func _dance_8ths() -> Dictionary:
	var p := "res://assets/audio/music/Intensity/"
	return {
		"type": "beat",
		"bpm": BPM_140,
		"tracks": [
			{"name": "Bass8th",  "cues": _repeat([p + "bass8th.wav"], 8)},
			{"name": "BassPlus", "cues": _seq(p, "bassplus-", 1, 8)},
			{"name": "Organ",    "cues": _seq(p, "organ-", 1, 8)},
			{"name": "Guitar",   "cues": _seq(p, "guitar-", 1, 8)},
		],
		"intensity_map": [
			# [Bass8th, BassPlus, Organ, Guitar]
			[true,  false, false, false],  # Intensity 0
			[true,  true,  false, false],  # Intensity 1
			[true,  true,  true,  false],  # Intensity 2
			[true,  true,  true,  true],   # Intensity 3
			[true,  true,  true,  true],   # Intensity 4
		],
	}


static func _land_of_sand() -> Dictionary:
	var p := "res://assets/audio/music/LandOfSand8ths/"
	return {
		"type": "beat",
		"bpm": BPM_140,
		"tracks": [
			{"name": "BassDrum",  "cues": _repeat([p + "LoSBassDrum-01.wav"], 16)},
			{"name": "HiHat",     "cues": _cycle(_seq(p, "LoSHiHat-", 1, 4, "%02d"), 16)},
			{"name": "BassSynth", "cues": _cycle(_seq(p, "LoSBassSynth-", 1, 8, "%02d"), 16)},
			{"name": "Stabs",     "cues": _cycle(_seq(p, "LoSStabs-", 1, 8, "%02d"), 16)},
			{"name": "Melody",    "cues": _cycle(_seq(p, "LoSMelody-", 1, 8, "%02d"), 16)},
			{"name": "Accent",    "cues": _cycle(_seq(p, "LoSAccent-", 1, 8, "%02d"), 16)},
			{"name": "Toms",      "cues": _cycle(_seq(p, "LoSToms-", 1, 8, "%02d"), 16)},
		],
		"intensity_map": [
			# [BD, HH, BS, St, Mel, Acc, Toms]
			[true,  true,  false, false, false, false, false],  # 0
			[true,  true,  true,  false, false, false, false],  # 1
			[true,  true,  true,  true,  false, false, false],  # 2
			[true,  true,  false, true,  true,  false, false],  # 3
			[true,  true,  false, true,  true,  true,  false],  # 4
			[true,  false, false, true,  true,  true,  true],   # 5
		],
	}


static func _ultrafix() -> Dictionary:
	var p := "res://assets/audio/music/ultrafix/"
	return {
		"type": "beat",
		"bpm": BPM_120,
		"tracks": [
			{"name": "BassLine", "cues": _seq(p, "bass_line-", 1, 8)},
			{"name": "Drums",    "cues": _seq(p, "drum-", 1, 8)},
			{"name": "FX",       "cues": _seq(p, "fx-", 1, 8)},
			{"name": "Synth",    "cues": _seq(p, "synth-", 1, 8)},
		],
		"intensity_map": [
			# [BassLine, Drums, FX, Synth]
			[true,  false, false, false],  # 0
			[true,  true,  false, false],  # 1
			[true,  true,  true,  false],  # 2
			[true,  true,  true,  true],   # 3
		],
	}


static func _win_one() -> Dictionary:
	var p := "res://assets/audio/music/winone/"
	# Beat: 4 cues repeating across 32 positions
	var beat_cues := _cycle(_seq(p, "beat-", 1, 4), 32)
	# Orion: A section (8 cues, repeated once) then B section (16 cues)
	var orion_cues: Array[String] = []
	orion_cues.append_array(_cycle(_seq(p, "orionA-", 1, 8), 16))
	orion_cues.append_array(_seq(p, "orionB-", 1, 16, "%02d"))
	# SynthBeat: A section (16 cues) then B section (16 cues)
	var synth_beat_cues: Array[String] = []
	synth_beat_cues.append_array(_seq(p, "synth_beatA-", 1, 16, "%02d"))
	synth_beat_cues.append_array(_seq(p, "synth_beatB-", 1, 16, "%02d"))
	# Rave: A section (4 cues, repeated 4 times = 16) then B section (16 cues)
	var rave_cues: Array[String] = []
	rave_cues.append_array(_cycle(_seq(p, "raveA-", 1, 4), 16))
	rave_cues.append_array(_seq(p, "raveB-", 1, 16, "%02d"))
	# Jam: A section (16 cues) then B section (16 cues)
	var jam_cues: Array[String] = []
	jam_cues.append_array(_seq(p, "jamA-", 1, 16, "%02d"))
	jam_cues.append_array(_seq(p, "jamB-", 1, 16, "%02d"))
	# Breath: empty except at beat 14
	var breath_cues: Array[String] = []
	for i in range(32):
		if i == 14:
			breath_cues.append(p + "breath.wav")
		else:
			breath_cues.append("")

	return {
		"type": "beat",
		"bpm": BPM_140,
		"tracks": [
			{"name": "Beat",      "cues": beat_cues},
			{"name": "Orion",     "cues": orion_cues},
			{"name": "SynthBeat", "cues": synth_beat_cues},
			{"name": "Rave",      "cues": rave_cues},
			{"name": "Jam",       "cues": jam_cues},
			{"name": "Breath",    "cues": breath_cues},
		],
		"intensity_map": [
			# [Beat, Orion, SynthBeat, Rave, Jam, Breath]
			[true,  false, false, false, false, false],  # 0
			[true,  true,  true,  false, false, true],   # 1
			[true,  true,  true,  true,  false, true],   # 2
			[true,  true,  true,  true,  true,  false],  # 3
			[true,  true,  true,  false, true,  false],  # 4
		],
	}


static func _second_chance() -> Dictionary:
	var p := "res://assets/audio/music/secondchance/"
	return {
		"type": "beat",
		"bpm": BPM_140,
		"tracks": [
			{"name": "Beat",    "cues": _seq(p, "beat-", 1, 16, "%02d")},
			{"name": "Nernt",   "cues": _seq(p, "nernt-", 1, 16, "%02d")},
			{"name": "Cartoon", "cues": _seq(p, "cartoon-", 1, 16, "%02d")},
		],
		"intensity_map": [
			# [Beat, Nernt, Cartoon]
			[true,  false, false],  # 0
			[true,  true,  false],  # 1
			[true,  true,  true],   # 2
		],
	}


static func _superbowl() -> Dictionary:
	var p := "res://assets/audio/music/Superbowl/"
	return {
		"type": "beat",
		"bpm": BPM_170,
		"tracks": [
			{"name": "SoloDrums", "cues": _cycle(_seq(p, "solodrums-", 1, 8), 64)},
			{"name": "BackDrums", "cues": _cycle(_seq(p, "backdrums-", 1, 8), 64)},
			{"name": "Bass",      "cues": _seq(p, "bass-", 1, 64, "%02d")},
			{"name": "BuzzOrgan", "cues": _seq(p, "buzzorgan-", 1, 64, "%02d")},
		],
		"intensity_map": [
			# [SoloDrums, BackDrums, Bass, BuzzOrgan]
			[true,  false, false, false],  # 0
			[false, true,  true,  false],  # 1
			[false, true,  true,  true],   # 2
		],
	}


# --- Helper functions for building cue arrays ---

## Generate sequential file paths: prefix + number + ".wav"
static func _seq(base_path: String, prefix: String, from: int, to: int, fmt: String = "%d") -> Array[String]:
	var result: Array[String] = []
	for i in range(from, to + 1):
		result.append(base_path + prefix + (fmt % i) + ".wav")
	return result


## Repeat a set of cues to fill target_count positions.
static func _repeat(cues: Array, target_count: int) -> Array[String]:
	var result: Array[String] = []
	for i in range(target_count):
		result.append(cues[0])
	return result


## Cycle through cues to fill target_count positions.
static func _cycle(cues: Array, target_count: int) -> Array[String]:
	var result: Array[String] = []
	for i in range(target_count):
		result.append(cues[i % cues.size()])
	return result
