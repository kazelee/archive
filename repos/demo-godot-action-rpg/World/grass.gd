extends Node2D

const GrassEffect = preload("res://Effects/grass_effect.tscn")


func create_grass_effect() -> void:
	# 将场景初始化并引用
	var grassEffect = GrassEffect.instantiate()
	# 获取当前所在场景(World)并添加effect效果
	var world = get_tree().current_scene
	world.add_child(grassEffect)
	grassEffect.global_position = global_position


func _on_hurtbox_area_entered(_area: Area2D) -> void:
	create_grass_effect()
	queue_free()
