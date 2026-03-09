extends Control
## Game over screen. Shows final score and offers restart or menu.


func _ready() -> void:
	$VBoxContainer/ScoreLabel.text = "Score: %d" % GameState.scores[0]
	$VBoxContainer/RestartButton.pressed.connect(_on_restart)
	$VBoxContainer/MenuButton.pressed.connect(_on_menu)
	$VBoxContainer/RestartButton.grab_focus()


func _on_restart() -> void:
	GameState.reset_game()
	GameState.current_state = GameState.State.GAMEPLAY


func _on_menu() -> void:
	GameState.current_state = GameState.State.MAIN_MENU
