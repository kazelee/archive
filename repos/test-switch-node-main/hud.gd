extends CanvasLayer

signal added_nodes

var is_edit_mode := true
#var is_moving := false
var is_linking := false

@onready var edit_mode: Button = $VBoxContainer/EditMode
@onready var add_node: Button = $VBoxContainer/AddNode
@onready var drag: Button = $VBoxContainer/Drag
@onready var link: Button = $VBoxContainer/Link
@onready var line_edit: LineEdit = $LineEdit


func _on_edit_mode_pressed() -> void:
	if is_edit_mode:
		is_edit_mode = false
		edit_mode.text = "游戏模式"
		add_node.disabled = true
		drag.disabled = true
		link.disabled = true
		line_edit.hide()
	else:
		is_edit_mode = true
		edit_mode.text = "编辑模式"
		add_node.disabled = false
		drag.disabled = false
		link.disabled = false


func _on_line_edit_text_submitted(new_text: String) -> void:
	var num := int(new_text)
	added_nodes.emit(num)
	line_edit.hide()


func _on_add_node_pressed() -> void:
	line_edit.show()


func _on_drag_pressed() -> void:
	is_linking = false
#	is_moving = true


func _on_link_pressed() -> void:
	is_linking = true
#	is_moving = false
