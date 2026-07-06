class_name Constrain
extends SteeringBehaviour
## Port of BGE.Forms.Constrain: spring the boid back inside a sphere.

@export var centre_on_position := true
@export var centre := Vector3.ZERO
@export var radius := 1000.0


func _ready() -> void:
	super()
	if centre_on_position:
		centre = boid.global_position


func calculate() -> Vector3:
	var to_target := boid.global_position - centre
	if to_target.length() > radius:
		return to_target.normalized() * (radius - to_target.length())
	return Vector3.ZERO
