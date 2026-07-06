class_name Hover
extends Harmonic
## Port of BGE.Forms.Hover: pulsed thrust along the boid's forward vector on
## the rising half of the sine cycle — jellyfish propulsion. Also drives the
## jelly bell pulse via theta.

var _old_theta := 0.0


func _ready() -> void:
	super()
	_old_theta = theta


func _process(_delta: float) -> void:
	# Theta is advanced in calculate() (like the Unity original, which used
	# boid.TimeDelta there), so nothing to do per-frame.
	pass


func calculate() -> Vector3:
	var delta := get_physics_process_delta_time()
	theta = fmod(theta, TAU)
	ramped_amplitude = lerpf(ramped_amplitude, amplitude, delta)
	ramped_speed = lerpf(ramped_speed, speed, delta)
	theta += delta * ramped_speed * (PI / 180.0)

	var theta_delta := theta - _old_theta
	var force := Vector3.ZERO
	if (theta < PI and theta_delta > 0) or (theta > PI and theta_delta < 0):
		force = boid.forward * absf(theta_delta) * ramped_amplitude
	_old_theta = theta
	return force
