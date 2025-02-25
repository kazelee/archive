@tool
extends Node2D

const Clock := preload("res://objects/clock.tscn")
const LeadLabel := preload("res://objects/lead_label.tscn")

enum Position {
	DOWN, RIGHT, LEFT
}

#var _get_position # func(i, s) -> Vec2
var my_position : Position

func clear() -> void:
	for node in get_children():
		if node.owner == null:
			node.queue_free()


func show_count_down(time: int) -> void:
	clear()
	var clock = Clock.instantiate()
	add_child(clock)
	clock.start(time)


func show_call_result(score: int) -> void:
	clear()
	var call_label := LeadLabel.instantiate()
	call_label.text = ["不叫", "一分", "两分", "三分"][score]
	add_child(call_label)


func show_lead_result(cards: Array) -> void:
	clear()
	if cards == []:
		var pass_label := LeadLabel.instantiate()
		pass_label.text = "不要"
		add_child(pass_label)
	else:
		_update_cards(cards)


func init_lead_position(pos: Position) -> void:
	my_position = pos
#	match pos:
#		Position.DOWN:
#			_get_position = func(i: int, s: int) -> Vector2:
#				return Vector2((i - (s - 1) / 2) * 16, 0)
#
#		Position.RIGHT:
#			_get_position = func(i: int, s: int) -> Vector2:
#				var ns := s if s <= 10 else 10
#				return Vector2(((i % 10) - ns + 1) * 16, i / 10 * 16)
#
#		Position.LEFT:
#			_get_position = func(i: int, s: int) -> Vector2:
#				return Vector2(i % 10 * 16, i / 10 * 16)


func _update_cards(cards: Array) -> void:
	for index in range(cards.size()):
		var card := Sprite2D.new()
		card.texture = load("res://assets/cards/card%02d.png" % cards[index])
		card.scale = Vector2.ONE * 0.5
		add_child(card)
#		card.position = _get_position.call(index, cards.size())
		var i := index
		var s := cards.size()
		match my_position:
			Position.DOWN:
				card.position = Vector2((i - (s - 1) / 2) * 16, 0)

			Position.RIGHT:
				var ns := s if s <= 10 else 10
				card.position = Vector2(((i % 10) - ns + 1) * 16, i / 10 * 16)

			Position.LEFT:
				card.position = Vector2(i % 10 * 16, i / 10 * 16)
