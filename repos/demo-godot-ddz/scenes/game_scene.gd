extends Sprite2D

enum Select {
	RESTART, BACK
}

@onready var top_card_bar: Node2D = $TopCardBar
@onready var final_screen: TextureRect = $FinalScreen


func _ready() -> void:
	SoundManager.play_music("res://assets/audio/bg_game.ogg")
	Game.new_game.connect(reset)
	Game.call_over.connect(refresh_top_cards)
	Game.to_show_final.connect(show_final_screen)
	final_screen.button_pressed.connect(enter_new_scene)
#	Rule.score_doubled.connect(top_card_bar.double_scores)
	Game.score_doubled.connect(top_card_bar.double_scores)


func reset() -> void:
	SoundManager.bgm_continue()
	top_card_bar.reset()


func refresh_top_cards() -> void:
	top_card_bar.set_top_cards(Game.get_cards(0))
	top_card_bar.set_scores(Game.highest_call, Game.highest_caller == Game.my_id)


func show_final_screen(my_id: int, new_scores: Dictionary, final_scores: Dictionary) -> void:
	SoundManager.bgm_stop()
	final_screen.init_scores(my_id, new_scores, final_scores)
	final_screen.show()


func enter_new_scene(select: Select) -> void:
	if select == Select.RESTART:
		self.rpc("_restart")
	else:
		self.rpc("_back_to_screen")


@rpc("any_peer", "call_local")
func _restart() -> void:
	final_screen.hide()
	Game.reset()
	Game.new_round()


@rpc("any_peer", "call_local")
func _back_to_screen() -> void:
	await get_tree().create_timer(0.5).timeout
	get_tree().change_scene_to_file("res://scenes/title_screen.tscn")
