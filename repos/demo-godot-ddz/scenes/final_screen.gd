extends TextureRect

signal button_pressed

enum Select {
	RESTART, BACK
}

@onready var title_label: Label = $TitleLabel
@onready var score_table: GridContainer = $ScoreTable
@onready var buttons: HBoxContainer = $Buttons


func init_scores(my_id: int, new_scores: Dictionary, final_scores: Dictionary) -> void:
	if my_id != 1:
		buttons.hide()

	if new_scores[my_id] > 0:
		title_label.text = "您赢了！"
		SoundManager.play_sound("win")
	else:
		title_label.text = "您输了T_T"
		SoundManager.play_sound("lose")

	for node in score_table.get_children():
		if node.owner == null:
			node.queue_free()

	for id in new_scores.keys():
		var id_label := Label.new()
		id_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		id_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		id_label.text = str(id) + " (you)" if id == my_id else str(id)
		score_table.add_child(id_label)

		var new_label := Label.new()
		new_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		new_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		new_label.text = str(new_scores[id])
		score_table.add_child(new_label)

		var final_label := Label.new()
		final_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
		final_label.vertical_alignment = VERTICAL_ALIGNMENT_CENTER
		final_label.text = str(final_scores[id])
		score_table.add_child(final_label)


func _on_restart_pressed() -> void:
	button_pressed.emit(Select.RESTART)


func _on_back_to_screen_pressed() -> void:
	button_pressed.emit(Select.BACK)
