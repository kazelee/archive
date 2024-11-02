extends Node

@onready var bgm_player: AudioStreamPlayer = $BGMPlayer
@onready var sound_player: AudioStreamPlayer = $SoundPlayer
#@export var default_music := preload("res://assets/audio/bg_room.mp3")


func play_music(path: String):
	if bgm_player.playing and bgm_player.stream.resource_path == path:
		return
	bgm_player.stream = load(path)
	bgm_player.play()


func bgm_continue():
	bgm_player.play()


func bgm_stop():
	bgm_player.stop()


func play_sound(sound: String):
	var res_path := ""
	match sound:
		"score_0":
			res_path = "res://assets/audio/f_score_0.mp3"
		"score_1":
			res_path = "res://assets/audio/f_score_1.mp3"
		"score_2":
			res_path = "res://assets/audio/f_score_2.mp3"
		"score_3":
			res_path = "res://assets/audio/f_score_3.mp3"
		"deal":
			res_path = "res://assets/audio/deal.mp3"
		"lose":
			res_path = "res://assets/audio/end_lose.mp3"
		"win":
			res_path = "res://assets/audio/end_win.mp3"
	if res_path != "":
		sound_player.stream = load(res_path)
		sound_player.play()


func _on_bgm_player_finished() -> void:
	bgm_player.play()
