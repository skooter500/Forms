class_name Separation
extends SteeringBehaviour
## Port of BGE.Forms.Seperation (sic).


func _ready() -> void:
	super()
	boid.tag_neighbours = true


func calculate() -> Vector3:
	var steering := Vector3.ZERO
	for other in boid.tagged:
		if other != boid:
			var to_entity: Vector3 = boid.global_position - other.global_position
			var d := to_entity.length()
			if d > 0.001:
				steering += to_entity.normalized() / d
	return steering
