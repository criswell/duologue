extends Control
## Main menu screen with animated logo and cycling backgrounds.

const LOGO_BORDER_FRAME_TIME := 0.075
const LOGO_FADE_IN_DURATION := 2.0
const BG_CYCLE_INTERVAL := 8.0
const BG_CROSSFADE_DURATION := 2.0

var _bg_textures: Array[Texture2D] = [
	preload("res://assets/sprites/backgrounds/background-01.png"),
	preload("res://assets/sprites/backgrounds/background-02.png"),
	preload("res://assets/sprites/backgrounds/background-03.png"),
	preload("res://assets/sprites/backgrounds/background-04.png"),
	preload("res://assets/sprites/backgrounds/background-05.png"),
]

var _logo_border_textures: Array[Texture2D] = []
var _logo_frame: int = 0
var _logo_frame_timer: float = 0.0
var _fade_timer: float = 0.0
var _bg_index: int = 0
var _bg_timer: float = 0.0
var _bg_crossfading: bool = false
var _bg_crossfade_timer: float = 0.0

@onready var logo_base: TextureRect = $LogoContainer/LogoBase
@onready var logo_border: TextureRect = $LogoContainer/LogoBorder
@onready var start_button: Button = $MenuContainer/StartButton
@onready var credits_button: Button = $MenuContainer/CreditsButton
@onready var quit_button: Button = $MenuContainer/QuitButton
@onready var menu_container: VBoxContainer = $MenuContainer
@onready var bg_current: TextureRect = $BGCurrent
@onready var bg_next: TextureRect = $BGNext


func _ready() -> void:
	# Load logo border animation frames
	for i in range(6):
		_logo_border_textures.append(
			load("res://assets/sprites/ui/logo-border-%d.png" % i)
		)

	start_button.pressed.connect(_on_start)
	credits_button.pressed.connect(_on_credits)
	quit_button.pressed.connect(_on_quit)
	start_button.grab_focus()

	# Start with logo faded out
	logo_base.modulate.a = 0.0
	logo_border.modulate.a = 0.0
	menu_container.modulate.a = 0.0

	# Initial background
	_bg_index = 0
	bg_current.texture = _bg_textures[_bg_index]

	AudioManager.play_song(AudioManager.SongID.SELECT_MENU)


func _process(delta: float) -> void:
	# Fade in logo and menu
	_fade_timer += delta
	var fade := clampf(_fade_timer / LOGO_FADE_IN_DURATION, 0.0, 1.0)
	logo_base.modulate.a = fade
	logo_border.modulate.a = fade
	menu_container.modulate.a = clampf((_fade_timer - 1.0) / 1.0, 0.0, 1.0)

	# Animate logo border
	_logo_frame_timer += delta
	if _logo_frame_timer >= LOGO_BORDER_FRAME_TIME:
		_logo_frame_timer -= LOGO_BORDER_FRAME_TIME
		_logo_frame = (_logo_frame + 1) % _logo_border_textures.size()
		logo_border.texture = _logo_border_textures[_logo_frame]

	# Background cycling
	if _bg_crossfading:
		_bg_crossfade_timer += delta
		var t := clampf(_bg_crossfade_timer / BG_CROSSFADE_DURATION, 0.0, 1.0)
		bg_next.modulate.a = t
		if t >= 1.0:
			# Swap: next becomes current
			bg_current.texture = bg_next.texture
			bg_next.modulate.a = 0.0
			_bg_crossfading = false
			_bg_timer = 0.0
	else:
		_bg_timer += delta
		if _bg_timer >= BG_CYCLE_INTERVAL:
			_bg_index = (_bg_index + 1) % _bg_textures.size()
			bg_next.texture = _bg_textures[_bg_index]
			_bg_crossfading = true
			_bg_crossfade_timer = 0.0


func _on_start() -> void:
	GameState.reset_game()
	GameState.current_state = GameState.State.GAMEPLAY


func _on_credits() -> void:
	GameState.current_state = GameState.State.CREDITS


func _on_quit() -> void:
	GameState.current_state = GameState.State.EXIT
