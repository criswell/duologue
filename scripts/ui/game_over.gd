extends Control
## Game over screen. Shows stylized game over image, final score, and options.


func _ready() -> void:
	$VBoxContainer/ScoreLabel.text = "Final Score: %d" % GameState.scores[0]
	$VBoxContainer/WaveLabel.text = "Reached Wave %d" % GameState.current_wave
	$VBoxContainer/ButtonContainer/RestartButton.pressed.connect(_on_restart)
	$VBoxContainer/ButtonContainer/MenuButton.pressed.connect(_on_menu)
	$VBoxContainer/ButtonContainer/RestartButton.grab_focus()

	# Fade in
	modulate.a = 0.0
	var tween := create_tween()
	tween.tween_property(self, "modulate:a", 1.0, 0.5)


func _on_restart() -> void:
	GameState.reset_game()
	GameState.current_state = GameState.State.GAMEPLAY


func _on_menu() -> void:
	GameState.current_state = GameState.State.MAIN_MENU
