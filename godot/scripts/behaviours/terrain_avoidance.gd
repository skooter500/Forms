class_name TerrainAvoidance
extends SteeringBehaviour
## Stands in for BGE.Forms.SceneAvoidance. The Unity original raycast against
## terrain colliders; here we sample the terrain height field directly and
## push away from the ground and the ceiling surface.

@export var min_clearance := 300.0
@export var look_ahead := 500.0


func calculate() -> Vector3:
	var wg := WorldGenerator.instance
	if wg == null:
		return Vector3.ZERO
	var steering := Vector3.ZERO
	var ahead: Vector3 = boid.global_position + boid.forward * look_ahead

	for p in [boid.global_position, ahead]:
		var ground: float = wg.sample_pos(p.x, p.z)
		var clearance: float = p.y - ground
		if clearance < min_clearance:
			steering.y += (min_clearance - clearance) / min_clearance * boid.max_force
		var headroom: float = wg.surface_height - p.y
		if headroom < min_clearance:
			steering.y -= (min_clearance - headroom) / min_clearance * boid.max_force
	return steering
