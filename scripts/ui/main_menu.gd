extends Control
## Main menu screen.


func _ready() -> void:
	$VBoxContainer/StartButton.pressed.connect(_on_start)
	$VBoxContainer/CreditsButton.pressed.connect(_on_credits)
	$VBoxContainer/QuitButton.pressed.connect(_on_quit)
	$VBoxContainer/StartButton.grab_focus()
	AudioManager.play_song(AudioManager.SongID.SELECT_MENU)


func _on_start() -> void:
	GameState.reset_game()
	GameState.current_state = GameState.State.GAMEPLAY


func _on_credits() -> void:
	GameState.current_state = GameState.State.CREDITS


func _on_quit() -> void:
	GameState.current_state = GameState.State.EXIT
