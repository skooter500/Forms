class_name OffsetPursue
extends SteeringBehaviour
## Port of BGE.Forms.OffsetPursue: hold a formation offset relative to a
## leader boid, with velocity look-ahead.

var leader_boid: Boid
var offset := Vector3.ZERO


func setup(p_leader: Boid) -> void:
	leader_boid = p_leader


func _ready() -> void:
	super()
	if leader_boid != null:
		var to_me: Vector3 = boid.global_position - leader_boid.global_position
		offset = leader_boid.global_basis.get_rotation_quaternion().inverse() * to_me


func calculate() -> Vector3:
	if leader_boid == null or not is_instance_valid(leader_boid):
		return Vector3.ZERO
	var target: Vector3 = leader_boid.global_position \
		+ leader_boid.global_basis.get_rotation_quaternion() * offset
	var look_ahead: float = (target - boid.global_position).length() / boid.max_speed
	target += leader_boid.velocity * look_ahead
	return boid.seek_force(target)
