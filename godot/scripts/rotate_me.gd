class_name RotateMe
extends Node
## Port of BGE.Forms.RotateMe: slowly spin the parent around an axis.

@export var speed := 0.1
@export var axis := Vector3.UP

var _lerped_speed := 0.0


func _process(delta: float) -> void:
	_lerped_speed = lerpf(_lerped_speed, speed, delta)
	var parent := get_parent() as Node3D
	if parent != null:
		parent.rotate(axis.normalized(), _lerped_speed * delta * TAU)
