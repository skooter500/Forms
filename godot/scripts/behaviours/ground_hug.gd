class_name GroundHug
extends SteeringBehaviour
## Stands in for SeekGround/gravity on the sandworms: steer strongly towards
## a point just above the terrain surface directly below the boid.

@export var hover_height := 50.0


func calculate() -> Vector3:
	var wg := WorldGenerator.instance
	if wg == null:
		return Vector3.ZERO
	var p := boid.global_position
	var target := Vector3(p.x, wg.sample_pos(p.x, p.z) + hover_height, p.z)
	return boid.seek_force(target)
