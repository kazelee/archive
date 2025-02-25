@tool
extends Sprite2D

@onready var time_label: Label = $Time
@onready var timer: Timer = $Timer


# 根据倒计时timer的timeleft设置label
func _process(_delta: float) -> void:
	time_label.text = str(int(timer.time_left + 1))


# 传入时间，显示倒计时闹钟，开始倒计时
func start(time: int) -> void:
	timer.start(time)
	time_label.show()


# 时间到了，直接删除自身
func _on_timer_timeout() -> void:
	queue_free()
