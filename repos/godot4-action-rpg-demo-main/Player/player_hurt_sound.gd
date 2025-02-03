extends AudioStreamPlayer2D

func _ready() -> void:
	connect("finished", Callable(self, "queue_free"))
