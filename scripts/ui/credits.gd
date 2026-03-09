extends Control
## Credits screen.


func _ready() -> void:
	$VBoxContainer/BackButton.pressed.connect(_on_back)
	$VBoxContainer/BackButton.grab_focus()


func _on_back() -> void:
	GameState.current_state = GameState.State.MAIN_MENU
