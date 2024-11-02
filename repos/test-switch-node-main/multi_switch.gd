@tool
extends Node2D

signal pressed

@export var full_num := 5
#@export var already_num := 1:
#	set(v):
#		already_num = v
#		update_nodes(already_num)
@export var pressed_times := 0:
	set(v):
		pressed_times = v
		if pressed_times == 0:
			update_nodes(full_num)
		else:
			update_nodes(pressed_times)

@export var node_id := 0
@export var neigbors: Array[int] = []


func _ready() -> void:
	for i in range(full_num):
		var node := Sprite2D.new()
		node.texture = preload("res://node-red.png")
		node.scale = Vector2.ONE * 0.5
		node.position = Vector2.UP * (50 * i + 25)
		add_child(node)
#		node.owner = self

func update_nodes(num: int) -> void:
	for node in get_children():
		if node.owner == null:
			node.queue_free()

	for i in range(full_num):
		var node := Sprite2D.new()
		node.texture = preload("res://node-red.png")
		node.scale = Vector2.ONE * 0.5
		node.position = Vector2.UP * (50 * i + 25)
		add_child(node)

	for i in range(num):
		var node := Sprite2D.new()
		node.texture = preload("res://node-blue.png")
		node.scale = Vector2.ONE * 0.5
		node.position = Vector2.UP * (50 * i + 25)
		node.z_index = 99
		add_child(node)


func add_for_others() -> void:
	pressed_times = (pressed_times + 1) % full_num


func _on_button_pressed() -> void:
	pressed_times = (pressed_times + 2) % full_num
	pressed.emit(neigbors)
