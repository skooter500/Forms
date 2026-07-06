class_name NoiseWander
extends SteeringBehaviour
## Port of BGE.Forms.NoiseWander: Perlin-driven wander target on a circle
## ahead of the boid.

@export var range_angle := PI
@export var radius := 50.0
@export var distance := 5.0
@export var noisiness := 0.2

var _noise := FastNoiseLite.new()
var _t := 0.0


func _ready() -> void:
	super()
	_noise.noise_type = FastNoiseLite.TYPE_PERLIN
	_noise.frequency = 1.0
	_noise.seed = randi()
	_t = randf_range(0.0, 1000.0)


func calculate() -> Vector3:
	var n := _noise.get_noise_2d(_t, 0.0) * 0.5 + 0.5
	var theta := remap(n, 0.0, 1.0, PI - range_angle, PI + range_angle)
	# Unity local (sin, 0, -cos) with +Z forward == Godot (sin, 0, cos) with -Z forward.
	var target := Vector3(sin(theta), 0.0, cos(theta)) * radius
	var local_target := target + Vector3.FORWARD * distance
	var world_target := boid.global_position + boid.global_basis.get_rotation_quaternion() * local_target

	_t += noisiness * get_physics_process_delta_time()
	var desired := (world_target - boid.global_position).normalized() * boid.max_speed
	return desired - boid.velocity
