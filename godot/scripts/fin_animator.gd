class_name FinAnimator
extends Node3D
## Port of BGE.Forms.FinAnimator: a fin pivot that flaps in phase with the
## boid's Harmonic behaviour.

enum Axis { X, Y, Z }

@export var axis: Axis = Axis.Z
@export var amplitude := 40.0
@export var rotation_offset := 220.0
@export var wigglyness := 1.0
@export var flip_direction := false

var harmonic: Harmonic

var _initial_amplitude := 1.0
var _lerped_amplitude := 0.0


func _ready() -> void:
	if harmonic != null:
		_initial_amplitude = maxf(harmonic.amplitude, 0.001)


func _physics_process(delta: float) -> void:
	if harmonic == null:
		return
	_lerped_amplitude = lerpf(_lerped_amplitude, amplitude, delta)
	var offset := deg_to_rad(rotation_offset)
	var angle := sin(harmonic.theta * wigglyness + offset) \
		* (harmonic.ramped_amplitude / _initial_amplitude) * _lerped_amplitude
	if flip_direction:
		angle = -angle
	var r := deg_to_rad(angle)
	match axis:
		Axis.X:
			rotation = Vector3(r, 0, 0)
		Axis.Y:
			rotation = Vector3(0, r, 0)
		Axis.Z:
			rotation = Vector3(0, 0, r)
