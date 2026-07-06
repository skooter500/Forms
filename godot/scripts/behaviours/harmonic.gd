class_name Harmonic
extends SteeringBehaviour
## Port of BGE.Forms.Harmonic: sinusoidal weave that gives every creature its
## undulating swim. theta also drives the FinAnimators.

enum Axis { HORIZONTAL, VERTICAL }

@export var speed := 100.0            # degrees per second of theta
@export var amplitude := 40.0
@export var direction: Axis = Axis.VERTICAL
@export var radius := 50.0
@export var distance := 20.0

var theta := 0.0
var ramped_speed := 0.0
var ramped_amplitude := 0.0


func _ready() -> void:
	super()
	theta = randf_range(0.0, PI)


func _process(delta: float) -> void:
	ramped_speed = lerpf(ramped_speed, speed, delta)
	theta += delta * ramped_speed * (PI / 180.0)


func calculate() -> Vector3:
	var n := sin(theta)
	ramped_amplitude = lerpf(ramped_amplitude, amplitude, get_physics_process_delta_time())

	var t := remap(n, -1.0, 1.0, -ramped_amplitude, ramped_amplitude)
	var a := deg_to_rad(t)

	var target := Vector3.ZERO
	if direction == Axis.HORIZONTAL:
		target = Vector3(sin(a), 0.0, -cos(a))
	else:
		target = Vector3(0.0, sin(a), -cos(a))
	target *= radius

	# Yaw-only frame so the weave stays creature-relative but level.
	var yaw := atan2(-boid.forward.x, -boid.forward.z)
	var local_target := target + Vector3.FORWARD * distance
	var world_target := boid.global_position + local_target.rotated(Vector3.UP, yaw)
	return boid.seek_force(world_target)
