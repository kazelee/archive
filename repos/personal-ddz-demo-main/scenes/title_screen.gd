extends TextureRect

func _ready() -> void:
	SoundManager.play_music("res://assets/audio/bg_room.mp3")


func _on_ai_mode_pressed() -> void:
	Game.is_ai_mode = true
	Game.new_round()
	get_tree().change_scene_to_file("res://scenes/game_scene.tscn")


func _on_player_mode_pressed() -> void:
	get_tree().change_scene_to_file("res://scenes/online_screen.tscn")


func _on_quit_pressed() -> void:
	get_tree().quit()
