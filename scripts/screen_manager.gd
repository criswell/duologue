extends Node
## Screen state machine. Manages transitions between game screens.
## Each screen is a scene that gets loaded/unloaded as the game state changes.
## Attached to the Main scene's ScreenManager node.

## Map State enum int values to scene paths directly to avoid autoload
## resolution issues at const parse time.
var _screens := {}

@onready var screen_container: Node = $ScreenContainer
var _current_screen: Node = null


func _ready() -> void:
	# Build the screen map at runtime where autoload is guaranteed available.
	_screens = {
		GameState.State.MAIN_MENU: "res://scenes/ui/main_menu.tscn",
		GameState.State.GAMEPLAY: "res://scenes/game/game_play.tscn",
		GameState.State.GAME_OVER: "res://scenes/ui/game_over.tscn",
		GameState.State.CREDITS: "res://scenes/ui/credits.tscn",
	}

	GameState.state_changed.connect(_on_state_changed)
	# Defer initial screen load to ensure tree is ready.
	call_deferred("_change_screen", GameState.State.MAIN_MENU)


func _on_state_changed(_old_state: GameState.State, new_state: GameState.State) -> void:
	if new_state == GameState.State.EXIT:
		get_tree().quit()
		return

	_change_screen(new_state)


func _change_screen(state: GameState.State) -> void:
	# Remove current screen
	if _current_screen:
		screen_container.remove_child(_current_screen)
		_current_screen.queue_free()
		_current_screen = null

	# Load new screen if we have one mapped
	if state in _screens:
		var scene := load(_screens[state]) as PackedScene
		if scene:
			_current_screen = scene.instantiate()
			screen_container.add_child(_current_screen)
		else:
			push_error("Failed to load screen scene: %s" % _screens[state])
