extends Node2D
class_name Switch

signal linked
signal pressed

@onready var sprite_2d: Sprite2D = $Sprite2D

@export var mod := 2

var selected := false
var draggable := false
var offset: Vector2
var pressed_times := 0:
	set(v):
		pressed_times = v
#		print(pressed_times)
		if pressed_times == 0:
			sprite_2d.texture = preload("res://node-blue.png")
		else:
			sprite_2d.texture = preload("res://node-red.png")

var linking := false
var id := -1
var mouse_pos: Vector2
var neighbors := []

#func _ready() -> void:
##	await owner.ready
#	await get_tree().create_timer(0.5).timeout
#	# board.gd
#	owner.line_created.connect(get_my_neighbor)


func _process(delta: float) -> void:
	if draggable:
		if Input.is_action_just_pressed("interact"):
			mouse_pos = get_global_mouse_position()
			offset = get_global_mouse_position() - global_position
			Mouse.is_dragging = true

		if Input.is_action_pressed("interact"):
			global_position = get_global_mouse_position() - offset
		elif Input.is_action_just_released("interact"):
			# 鼠标位置不变时，认定为原地点击，改变状态
			if mouse_pos == get_global_mouse_position():
#				pressed_times = (pressed_times + 1) % mod
				add_times()
			Mouse.is_dragging = false
		return

	if linking:
		if Input.is_action_just_pressed("interact"):
#			Mouse.is_linking = true
			linked.emit(position, id)
		if Input.is_action_just_released("interact"):
#			Mouse.is_linking = false
			linked.emit(position, id)
		return

	# 只有游戏模式才会运行
	if selected and Input.is_action_just_released("interact"):
#		pressed_times = (pressed_times + 1) % mod
		add_times()
		# for neighbors
		pressed.emit(neighbors)


func add_times() -> void:
	pressed_times = (pressed_times + 1) % mod


func get_my_neighbor(nei_id: int) -> void:
	if nei_id not in neighbors:
		neighbors.append(nei_id)
#	if id_1 == id and id_2 not in neighbors:
#		neighbors.append(id_2)
#	if id_2 == id and id_1 not in neighbors:
#		neighbors.append(id_1)


func _on_area_2d_mouse_entered() -> void:
	if Mouse.is_dragging:
		return
#	if Mouse.is_linking:
#		linking = true
#		return

	selected = true
	# 编辑模式的状态优先
	if Hud.is_edit_mode:
		if Hud.is_linking:
			linking = true
			draggable = false
		else:
			draggable = true
			linking = false

func _on_area_2d_mouse_exited() -> void:
	if not Mouse.is_dragging:
		selected = false
		draggable = false
		linking = false
