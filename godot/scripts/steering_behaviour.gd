class_name SteeringBehaviour
extends Node
## Port of BGE.Forms.SteeringBehaviour. Behaviours are children of a Boid
## and return a steering force from calculate().

@export var weight := 1.0
@export var active := true

var boid: Boid


func _ready() -> void:
	boid = get_parent() as Boid


func calculate() -> Vector3:
	return Vector3.ZERO
