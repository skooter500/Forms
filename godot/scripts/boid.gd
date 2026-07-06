class_name Boid
extends Node3D
## Port of BGE.Forms.Boid — the steering-force integrator at the heart of
## every creature. Behaviours are child nodes extending SteeringBehaviour;
## forces are accumulated in priority (tree) order up to max_force.
## Godot convention: forward is -Z.

@export var mass := 1.0
@export var max_speed := 20.0
@export var max_force := 10.0
@export var damping := 0.01
@export var apply_banking := true
@export var keep_upright := false
@export var straightening_tendancy := 0.2
@export var rolling_tendancy := 0.05
@export var max_turn_degrees := 180.0
@export var limit_up_and_down := 1.0

var velocity := Vector3.ZERO
var acceleration := Vector3.ZERO
var force := Vector3.ZERO
var speed := 0.0
var bank := 0.0
var suspended := false

var school: SchoolGenerator = null
var tag_neighbours := false
var tag_neighbours_dither := 0.5
var tagged: Array[Boid] = []

var behaviours: Array[SteeringBehaviour] = []

var forward: Vector3:
	get:
		return -global_basis.z


func _ready() -> void:
	# Deferred: builders often add_child(boid) first and attach behaviours
	# afterwards, so collect once the whole creature is assembled.
	_collect_behaviours.call_deferred()


func _collect_behaviours() -> void:
	behaviours.clear()
	for child in get_children():
		if child is SteeringBehaviour:
			behaviours.append(child)


func _physics_process(delta: float) -> void:
	if suspended:
		return
	force = calculate_force()

	var new_acceleration := force / mass
	var smooth_rate: float = clampf(9.0 * delta, 0.15, 0.4) / 2.0
	acceleration = acceleration.lerp(new_acceleration, smooth_rate)
	velocity += acceleration * delta

	# Banking: blend a global up (self-righting) with turn acceleration.
	var global_up := Vector3(0, straightening_tendancy, 0)
	var accel_up := acceleration * rolling_tendancy
	accel_up.y = 0
	var bank_up := accel_up + global_up
	var temp_up := global_basis.y.lerp(bank_up, delta)

	speed = velocity.length()
	if speed > max_speed:
		velocity = velocity.normalized() * max_speed
		speed = max_speed

	if speed > 0.01:
		var desired := velocity.normalized()
		if keep_upright:
			desired.y = 0
			desired = desired.normalized()
		var new_forward := _rotate_toward(forward, desired, deg_to_rad(max_turn_degrees) * delta)
		if apply_banking and absf(new_forward.dot(temp_up.normalized())) < 0.99:
			global_basis = Basis.looking_at(new_forward, temp_up.normalized())
		else:
			global_basis = Basis.looking_at(new_forward, Vector3.UP)

	global_position += velocity * delta
	velocity *= 1.0 - damping * delta

	# How much are we banked? Drives the fin animation.
	var right := global_basis.x
	var flat_right := Vector3(right.x, 0, right.z)
	if flat_right.length() > 0.001:
		bank = rad_to_deg(right.angle_to(flat_right.normalized()))
		bank = bank if right.y > 0 else -bank


static func _rotate_toward(current: Vector3, target: Vector3, max_radians: float) -> Vector3:
	var angle := current.angle_to(target)
	if angle < 0.0001 or angle <= max_radians:
		return target
	var axis := current.cross(target)
	if axis.length() < 0.0001:
		axis = Vector3.UP
	return current.rotated(axis.normalized(), max_radians)


func calculate_force() -> Vector3:
	var total := Vector3.ZERO
	if tag_neighbours and school != null:
		_tag_neighbours_simple(school.neighbour_distance)
	for behaviour in behaviours:
		if not behaviour.active:
			continue
		var f := behaviour.calculate() * behaviour.weight
		var remaining := max_force - total.length()
		if remaining <= 0:
			break
		total += f if f.length() < remaining else f.normalized() * remaining
	total.y *= limit_up_and_down
	return total


func seek_force(target: Vector3) -> Vector3:
	var desired := (target - global_position).normalized() * max_speed
	return desired - velocity


func flee_force(target: Vector3) -> Vector3:
	var desired := (global_position - target).normalized() * max_speed
	return desired - velocity


func _tag_neighbours_simple(in_range: float) -> void:
	if randf() >= tag_neighbours_dither:
		return
	tagged.clear()
	var range_sq := in_range * in_range
	for other in school.boids:
		if other != self and not other.suspended:
			if global_position.distance_squared_to(other.global_position) < range_sq:
				tagged.append(other)
