class_name JitterWander
extends SteeringBehaviour
## Port of BGE.Forms.JitterWander: classic Reynolds wander — jittered target
## on a sphere projected ahead of the boid.

@export var radius := 10.0
@export var jitter := 5.0
@export var distance := 15.0

var _target := Vector3.ZERO


func _ready() -> void:
	super()
	_target = _random_inside_unit_sphere() * radius


static func _random_inside_unit_sphere() -> Vector3:
	while true:
		var v := Vector3(randf_range(-1, 1), randf_range(-1, 1), randf_range(-1, 1))
		if v.length_squared() <= 1.0:
			return v
	return Vector3.ZERO


func calculate() -> Vector3:
	_target += _random_inside_unit_sphere() * jitter * get_physics_process_delta_time()
	_target = _target.normalized() * radius
	var local_target := _target + Vector3.FORWARD * distance
	var world_target := boid.global_position + boid.global_basis.get_rotation_quaternion() * local_target
	return world_target - boid.global_position
