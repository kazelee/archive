extends HBoxContainer

signal has_called

@onready var score_1: Button = $Score1
@onready var score_2: Button = $Score2


func reset() -> void:
	score_1.disabled = false
	score_2.disabled = false


func init_buttons(high: int) -> void:
	if high >= 1:
		score_1.disabled = true
	if high >= 2:
		score_2.disabled = true


func _on_score_0_pressed() -> void:
	has_called.emit(0)


func _on_score_1_pressed() -> void:
	has_called.emit(1)


func _on_score_2_pressed() -> void:
	has_called.emit(2)


func _on_score_3_pressed() -> void:
	has_called.emit(3)
