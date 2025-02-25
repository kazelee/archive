extends Node2D

@export var max_health := 1:
	set(v):
		max_health = v
		self.health = min(health, max_health)
		emit_signal("max_health_changed", max_health)

var health := max_health:
	set(v):
		health = v
		emit_signal("health_changed", health)
		if health <= 0:
			emit_signal("no_health")

signal no_health
signal health_changed(value)
signal max_health_changed(value)

func _ready():
	self.health = max_health
