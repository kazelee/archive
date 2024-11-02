extends Node2D

@onready var multi_switch: Node2D = $MultiSwitch
@onready var multi_switch_2: Node2D = $MultiSwitch2
@onready var multi_switch_3: Node2D = $MultiSwitch3
@onready var multi_switch_4: Node2D = $MultiSwitch4


func _ready() -> void:
	Hud.hide()
	multi_switch.pressed.connect(send_ids)
	multi_switch_2.pressed.connect(send_ids)
	multi_switch_3.pressed.connect(send_ids)
	multi_switch_4.pressed.connect(send_ids)


func send_ids(ids: Array):
	for node in get_children():
		if node.node_id in ids:
			node.add_for_others()
