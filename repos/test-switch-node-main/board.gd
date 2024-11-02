extends Node2D

signal line_created

const LINE_TEXTURE = preload("res://line.png")

@onready var spawn: Node2D = $Spawn

var is_second_point := false
var first_pos: Vector2
var first_id: int

func _ready() -> void:
	Hud.added_nodes.connect(_update_board)
#	Switch.linked.connect(_update_line)


func _update_board(num: int) -> void:
	print("update_board with: %s" % num)
	for node in spawn.get_children():
		node.queue_free()
#		if node is Switch:
#			node.queue_free()

	for index in range(num):
		var node := preload("res://switch.tscn").instantiate()
		node.id = index
		node.position = Vector2(index % ceili(sqrt(num)) * 105, index / ceili(sqrt(num)) * 105)
		print("[%s] position: (%s, %s)" % [index, node.position.x, node.position.y])
		spawn.add_child(node)
		node.owner = spawn

		node.linked.connect(_update_line)
		node.pressed.connect(_update_others)


func _update_line(pos: Vector2, id: int) -> void:
	prints(pos, id)
	if is_second_point:
		is_second_point = false
		# 在同一个节点上按下和松开鼠标
		if first_pos == pos and first_id == id:
			return
		var line := Line2D.new()
		spawn.add_child(line)

		line.add_point(first_pos)
		line.add_point(pos)
		line.width = LINE_TEXTURE.get_size().y
		line.texture = LINE_TEXTURE
		line.texture_mode = Line2D.LINE_TEXTURE_TILE
		line.default_color = Color.WHITE
		line.show_behind_parent = true

#		line_created.emit(first_id, id)
		for node in spawn.get_children():
			if not node is Switch:
				return
			if node.id == first_id:
				node.get_my_neighbor(id)
			if node.id == id:
				node.get_my_neighbor(first_id)
	else:
		first_pos = pos
		first_id = id
		is_second_point = true


func _update_others(ids: Array) -> void:
	for node in spawn.get_children():
		if node is Switch and node.id in ids:
#			node.pressed_times = (node.pressed_times + 1) % 2
			node.add_times()
