@tool
extends Area2D
class_name Card

signal interact

var is_last_one: bool = false
var card_id: int: set = set_card_id

var _texture: Texture
var side_distance := 40

func set_card_id(id: int) -> void:
	assert(id >= 0 and id < 54)
	card_id = id

	_set_texture(load("res://assets/cards/card%02d.png" % card_id))


func _input_event(viewport: Viewport, event: InputEvent, shape_idx: int) -> void:
	if not event.is_action_pressed("interact"):
		return

	_interact()


func _interact() -> void:
	interact.emit()


func _set_texture(texture: Texture) -> void:
	_texture = texture

	for node in get_children():
		if node.owner == null:
			node.queue_free()

	if _texture == null:
		return

	var sprite := Sprite2D.new()
	sprite.texture = _texture
	add_child(sprite)

	var rect := RectangleShape2D.new()
	var collider := CollisionShape2D.new()

	if is_last_one:
		rect.extents = _texture.get_size() / 2
	else:
		rect.extents = Vector2i(side_distance / 2, _texture.get_size().y / 2)
		collider.position = Vector2.LEFT * (45 - side_distance / 2)

	collider.shape = rect
	add_child(collider)
