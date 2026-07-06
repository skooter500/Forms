class_name Seek
extends SteeringBehaviour
## Port of BGE.Forms.Seek.

@export var target_node: Node3D = null
@export var target := Vector3.ZERO


func calculate() -> Vector3:
	if target_node != null:
		target = target_node.global_position
	return boid.seek_force(target)
