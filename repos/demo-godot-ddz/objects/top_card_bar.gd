@tool
extends Node2D

@onready var card_1: Sprite2D = $Card1
@onready var card_2: Sprite2D = $Card2
@onready var card_3: Sprite2D = $Card3
@onready var base: Label = $Base
@onready var times: Label = $Times

var times_score := 1


func reset() -> void:
	var card_back := preload("res://assets/cards/card54.png")
	card_1.texture = card_back
	card_2.texture = card_back
	card_3.texture = card_back
	base.text = "底分: 50"
	times.text = "倍数: 1"
	times_score = 1


func set_top_cards(cards: Array) -> void:
	card_1.texture = load("res://assets/cards/card%02d.png" % cards[0])
	card_2.texture = load("res://assets/cards/card%02d.png" % cards[1])
	card_3.texture = load("res://assets/cards/card%02d.png" % cards[2])


func set_scores(score: int, is_landlord: bool = false) -> void:
	if is_landlord:
		base.text = "底分: 100"
	times_score = score
	times.text = "倍数: " + str(score)

func double_scores() -> void:
	times_score *= 2
	times.text = "倍数: " + str(times_score)
