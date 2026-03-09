extends Control
## Credits screen with music.


func _ready() -> void:
	$ScrollContainer/BackButton.pressed.connect(_on_back)
	$ScrollContainer/BackButton.grab_focus()
	AudioManager.play_song(AudioManager.SongID.CREDITS)

	# Fade in
	modulate.a = 0.0
	var tween := create_tween()
	tween.tween_property(self, "modulate:a", 1.0, 1.0)


func _on_back() -> void:
	GameState.current_state = GameState.State.MAIN_MENU
