class_name Alignment
extends SteeringBehaviour
## Port of BGE.Forms.Alignment.


func _ready() -> void:
	super()
	boid.tag_neighbours = true


func calculate() -> Vector3:
	var steering := Vector3.ZERO
	var count := 0
	for other in boid.tagged:
		steering += other.forward
		count += 1
	if count > 0:
		steering /= count
		steering -= boid.forward
	return steering
