class_name Cohesion
extends SteeringBehaviour
## Port of BGE.Forms.Cohesion.


func _ready() -> void:
	super()
	boid.tag_neighbours = true


func calculate() -> Vector3:
	var centre := Vector3.ZERO
	var count := 0
	for other in boid.tagged:
		centre += other.global_position
		count += 1
	if count == 0:
		return Vector3.ZERO
	centre /= count
	var f := boid.seek_force(centre)
	return f.normalized() if f.length() > 0.001 else Vector3.ZERO
