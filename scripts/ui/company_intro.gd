extends Control
## Funavision company intro. Logo fades in, holds, fades out, then transitions.

enum IntroState { BLACK, FADE_IN, HOLD, FADE_OUT }

const FADE_IN_DURATION := 1.5
const HOLD_DURATION := 2.0
const FADE_OUT_DURATION := 1.0

var _state := IntroState.BLACK
var _timer: float = 0.0

@onready var logo: TextureRect = $Logo
@onready var motto: TextureRect = $Motto


func _ready() -> void:
	logo.modulate.a = 0.0
	motto.modulate.a = 0.0
	_state = IntroState.BLACK
	_timer = 0.5  # Brief black pause


func _process(delta: float) -> void:
	_timer -= delta

	match _state:
		IntroState.BLACK:
			if _timer <= 0.0:
				_state = IntroState.FADE_IN
				_timer = FADE_IN_DURATION
		IntroState.FADE_IN:
			var t := 1.0 - _timer / FADE_IN_DURATION
			logo.modulate.a = clampf(t, 0.0, 1.0)
			motto.modulate.a = clampf(t - 0.3, 0.0, 1.0)  # Motto fades in slightly after
			if _timer <= 0.0:
				logo.modulate.a = 1.0
				motto.modulate.a = 1.0
				_state = IntroState.HOLD
				_timer = HOLD_DURATION
		IntroState.HOLD:
			if _timer <= 0.0:
				_state = IntroState.FADE_OUT
				_timer = FADE_OUT_DURATION
		IntroState.FADE_OUT:
			var t := _timer / FADE_OUT_DURATION
			logo.modulate.a = clampf(t, 0.0, 1.0)
			motto.modulate.a = clampf(t, 0.0, 1.0)
			if _timer <= 0.0:
				GameState.current_state = GameState.State.MAIN_MENU


func _input(event: InputEvent) -> void:
	# Skip intro on any button press
	if event is InputEventKey and event.pressed:
		GameState.current_state = GameState.State.MAIN_MENU
	elif event is InputEventJoypadButton and event.pressed:
		GameState.current_state = GameState.State.MAIN_MENU
