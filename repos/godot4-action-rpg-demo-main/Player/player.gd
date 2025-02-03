extends CharacterBody2D

const PlayerHurtSound = preload("res://Player/player_hurt_sound.tscn")

const ACCELERATION: float = 500
const FRICTION: float = 500
const MAX_SPEED: float = 80
const ROLL_SPEED: float = 125

enum State {
	MOVE,
	ROLL,
	ATTACK,
}

var state := State.MOVE
var input_vector := Vector2.DOWN
var last_direction := Vector2.DOWN
var roll_vector := Vector2.DOWN

@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var animation_tree: AnimationTree = $AnimationTree
@onready var attack_timer: Timer = $AttackTimer
@onready var roll_timer: Timer = $RollTimer
@onready var sword_hitbox: Area2D = $HitboxPivot/SwordHitbox
var stats := PlayerStats
@onready var hurtbox: Area2D = $Hurtbox
@onready var blink_animation_player: AnimationPlayer = $BlinkAnimationPlayer


func _ready() -> void:
	randomize()
	stats.connect("no_health", Callable(self, "queue_free"))
	animation_tree.active = true
	sword_hitbox.knockback_vector = input_vector


func _process(_delta: float) -> void:
	update_animation_parameters()


func _physics_process(delta: float) -> void:
	input_vector = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down").normalized()
	if input_vector != Vector2.ZERO:
		last_direction = input_vector
		roll_vector = input_vector
		sword_hitbox.knockback_vector = input_vector 
	
	match state:
		State.MOVE:
			move_state(delta)
			
		State.ROLL:
			roll_state()
			
		State.ATTACK:
			attack_state()


func move_state(delta: float) -> void:
	if input_vector != Vector2.ZERO:
		velocity = velocity.move_toward(input_vector * MAX_SPEED, ACCELERATION * delta)
	else:
		velocity = velocity.move_toward(Vector2.ZERO, FRICTION * delta)
	
	move_and_slide()
	
	if Input.is_action_just_pressed("attack"):
		attack_timer.start()
		state = State.ATTACK

	if Input.is_action_just_pressed("roll"):
		roll_timer.start()
		state = State.ROLL


func attack_state() -> void:
	velocity = Vector2.ZERO


func roll_state() -> void:
	velocity = roll_vector * ROLL_SPEED
	move_and_slide()


func update_animation_parameters() -> void:
	if velocity == Vector2.ZERO:
		animation_tree["parameters/conditions/idle"] = true
		animation_tree["parameters/conditions/is_running"] = false
	else:
		animation_tree["parameters/conditions/idle"] = false
		animation_tree["parameters/conditions/is_running"] = true

	if Input.is_action_just_pressed("attack"):
		animation_tree["parameters/conditions/attack"] = true
	else:
		animation_tree["parameters/conditions/attack"] = false
	
	if Input.is_action_just_pressed("roll"):
		animation_tree["parameters/conditions/roll"] = true
	else:
		animation_tree["parameters/conditions/roll"] = false
	
	if velocity == Vector2.ZERO:
		# 修复初始状态下，攻击默认方向为(-1,0)的bug
		animation_tree["parameters/Attack/blend_position"] = last_direction
		animation_tree["parameters/Roll/blend_position"] = last_direction
	
	if velocity != Vector2.ZERO:
		var direction: Vector2 = input_vector if input_vector != Vector2.ZERO else last_direction
		animation_tree["parameters/Idle/blend_position"] = direction
		animation_tree["parameters/Run/blend_position"] = direction
		animation_tree["parameters/Attack/blend_position"] = direction
		animation_tree["parameters/Roll/blend_position"] = direction


func _on_attack_timer_timeout() -> void:
	state = State.MOVE

func _on_roll_timer_timeout() -> void:
	state = State.MOVE


func _on_hurtbox_area_entered(area: Area2D) -> void:
	stats.health -= area.damage
	hurtbox.start_invincibility(0.5)
	hurtbox.create_hit_effect()
	var playerHurtSound = PlayerHurtSound.instantiate()
	get_tree().current_scene.add_child(playerHurtSound)


func _on_hurtbox_invincibility_started() -> void:
	blink_animation_player.play("start")


func _on_hurtbox_invincibility_ended() -> void:
	blink_animation_player.play("stop")
