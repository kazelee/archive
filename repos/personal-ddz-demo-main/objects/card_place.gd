@tool
extends Node2D

var _side_distance := 32
var _rest_num := 17

var selected_cards := []

@onready var card_back: Sprite2D = $CardBack
@onready var card_rest: Label = $CardRest


func reset() -> void:
	card_back.hide()
	card_rest.hide()
	update_cards([])


func init_others_cards() -> void:
	card_back.show()
	card_rest.show()


func update_others_cards(num: int) -> void:
	_rest_num = num
	card_rest.text = str(num)


func minus_others_cards(minus: int) -> void:
	_rest_num -= minus
	card_rest.text = str(_rest_num)
	if _rest_num == 0:
		card_back.hide()
		card_rest.hide()


func update_cards(cards: Array) -> void:
	print(cards)
	for node in get_children():
		if node.owner == null:
			node.queue_free()

	if cards.size() == 0:
		return
	_side_distance = 700 / cards.size() if 700 / cards.size() < 40 else 40

	for index in range(cards.size()):
		var card := Card.new()
		card.side_distance = _side_distance

		if index == cards.size() - 1:
			card.is_last_one = true

		card.card_id = cards[index]
		add_child(card)
		card.position = Vector2.RIGHT * (index - (cards.size() - 1) / 2) * _side_distance
		card.interact.connect(_select_card.bind(cards[index], card))


func select_cards(cards: Array) -> void:
	selected_cards = []
	for node in get_children():
		if node is Card and node.card_id in cards:
			selected_cards.append(node.card_id)
			node.position = Vector2(node.position.x, -32)
		else:
			node.position = Vector2(node.position.x, 0)


func _select_card(id: int, card: Card) -> void:
	if id not in selected_cards:
		selected_cards.append(id)
		card.position = Vector2(card.position.x, -32)
	else:
		selected_cards.erase(id)
		card.position = Vector2(card.position.x, 0)
#	print(selected_cards)
