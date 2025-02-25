extends HBoxContainer

signal has_led

enum Lead {
	PASS, HINT, SHOT
}

enum State {
	FIRST, NONE, FOLLOW
}

@onready var pass_button: Button = $Pass
@onready var hint: Button = $Hint
@onready var shot: Button = $Shot

var lead_state := State.FIRST



func init_buttons(state: State) -> void:
	match state:
		State.FIRST:
			pass_button.hide()
			hint.hide()
			shot.show()

		State.NONE:
			pass_button.show()
			hint.hide()
			shot.hide()

		State.FOLLOW:
			pass_button.show()
			hint.show()
			shot.show()


func _on_pass_pressed() -> void:
	has_led.emit(Lead.PASS)


func _on_hint_pressed() -> void:
	has_led.emit(Lead.HINT)


func _on_shot_pressed() -> void:
	has_led.emit(Lead.SHOT)
