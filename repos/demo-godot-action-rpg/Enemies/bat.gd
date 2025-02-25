extends CharacterBody2D

const EnemyDeathEffect = preload("res://Effects/enemy_death_effect.tscn")

@export var ACCELERATION := 300.0
@export var MAX_SPEED := 50.0
@export var FRICTION := 200.0
@export var WANDER_TARGET_RANGE := 4


enum State {
	IDLE,
	WANDER,
	CHASE,
}

var state := State.CHASE
var knockback := Vector2.ZERO
const KNOCKBACK_SPEED := 120.0

@onready var stats: Node2D = $Stats
@onready var player_detection_zone: Area2D = $PlayerDetectionZone
@onready var animation_sprite: AnimatedSprite2D = $AnimationSprite
@onready var hurtbox: Area2D = $Hurtbox
@onready var soft_collision: Area2D = $SoftCollision
@onready var wander_controller: Node2D = $WanderController
@onready var animation_player: AnimationPlayer = $AnimationPlayer


func _ready() -> void:
	state = pick_random_state([State.IDLE, State.WANDER])


func _physics_process(delta: float) -> void:
	velocity = velocity.move_toward(Vector2.ZERO, FRICTION * delta)
	
	move_and_slide()
	
	match state:
		State.IDLE:
			velocity = velocity.move_toward(Vector2.ZERO, FRICTION * delta)
			seek_player()
			if not wander_controller.get_time_left() > 0:
				update_wander()

		State.WANDER:
			seek_player()
			if not wander_controller.get_time_left() > 0:
				update_wander()
			accelerate_towards_point(wander_controller.target_position, delta)
			if global_position.distance_to(wander_controller.target_position) <= WANDER_TARGET_RANGE:
				update_wander()
			
		State.CHASE:
			var player = player_detection_zone.player
			if player != null:
				accelerate_towards_point(player.global_position, delta)
			else:
				state = State.IDLE
				
			animation_sprite.flip_h = velocity.x < 0
	
	if soft_collision.is_colliding():
		velocity += soft_collision.get_push_vector() * delta * 400
	
	move_and_slide()


func accelerate_towards_point(point, delta):
	var direction = global_position.direction_to(point)
	velocity = velocity.move_toward(direction * MAX_SPEED, ACCELERATION * delta)
	animation_sprite.flip_h = velocity.x < 0


func seek_player():
	if player_detection_zone.can_see_player():
		state = State.CHASE


func update_wander():
	state = pick_random_state([State.IDLE, State.WANDER])
	wander_controller.start_wander_timer(randf_range(1, 3))


func pick_random_state(state_list):
	state_list.shuffle()
	return state_list.pop_front()


func _on_hurtbox_area_entered(area: Area2D) -> void:
	stats.health -= area.damage
	velocity = area.knockback_vector * KNOCKBACK_SPEED
	hurtbox.create_hit_effect()
	hurtbox.start_invincibility(0.4)


func _on_stats_no_health() -> void:
	queue_free()
	var enemyDeathEffect = EnemyDeathEffect.instantiate()
	var world = get_tree().current_scene
	world.add_child(enemyDeathEffect)
	enemyDeathEffect.global_position = global_position


func _on_hurtbox_invincibility_started() -> void:
	animation_player.play("start")


func _on_hurtbox_invincibility_ended() -> void:
	animation_player.play("stop")
