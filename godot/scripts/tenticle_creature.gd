class_name TenticleCreature
extends Node3D
## Port of BGE.Forms.TenticleCreatureGenerator (spelling preserved from the
## original). A head with a radial ring of tentacles, each tentacle a chain
## of shrinking segments driven by a SpineAnimator whose head is a swaying
## pivot. Covers three of the original species families:
##   SWIM   — tenticleCreature: swims, tentacles trailing behind
##   JELLY  — controlableJelly: hovers with pulsed Hover propulsion, bell pulses
##   FLOWER — tenticleFlower: rooted to the ground, tentacles waving upward

enum Mode { SWIM, JELLY, FLOWER }

@export var mode: Mode = Mode.SWIM
@export var num_tenticles := 8
@export var head_scale := 100.0
@export var tenticle_scale := 25.0
@export var tenticle_segments := 8
@export var color_a := Color(0.9, 0.4, 1.0)
@export var color_b := Color(0.3, 0.9, 0.8)
@export var max_speed := 60.0

var boid: Boid
var hover: Hover

var _pivots: Array[Node3D] = []
var _pivot_bases: Array[Basis] = []
var _phases: Array[float] = []
var _bell: MeshInstance3D
var _time := 0.0


func _ready() -> void:
	# Jellies are ghostly — barely-there translucent bell.
	var mat_a := FormsMeshes.make_material(color_a, 0.25, 0.28) if mode == Mode.JELLY \
		else FormsMeshes.make_material(color_a)

	var head_holder: Node3D
	if mode == Mode.FLOWER:
		head_holder = Node3D.new()
		head_holder.name = "Base"
		add_child(head_holder)
	else:
		boid = Boid.new()
		boid.name = "Head"
		boid.max_speed = max_speed
		boid.max_force = max_speed * 4.0
		add_child(boid)
		head_holder = boid
		_add_behaviours()

	var bell_scale := Vector3.ONE * head_scale
	if mode == Mode.JELLY:
		bell_scale = Vector3(head_scale, head_scale * 0.55, head_scale)
	elif mode == Mode.FLOWER:
		bell_scale = Vector3(head_scale, head_scale * 0.4, head_scale)
	_bell = FormsMeshes.make_part(FormsMeshes.sphere(), mat_a, bell_scale)
	head_holder.add_child(_bell)

	# Tentacle ring. Directions are in the head's local space:
	# SWIM   — trailing behind (+Z, since creatures face -Z)
	# JELLY  — hanging below (-Y)
	# FLOWER — reaching up (+Y)
	var theta_inc := TAU / num_tenticles
	for i in num_tenticles:
		var theta := i * theta_inc
		var ring := Vector3(sin(theta), 0, cos(theta))
		var along: Vector3
		var spread := 0.45
		match mode:
			Mode.SWIM:
				along = (Vector3(0, 0, 1) + ring * spread).normalized()
			Mode.JELLY:
				along = (Vector3(0, -1, 0) + ring * spread).normalized()
			Mode.FLOWER:
				along = (Vector3(0, 1, 0) + ring * spread).normalized()
		_make_tentacle(head_holder, ring * head_scale * 0.4, along, theta)


func _add_behaviours() -> void:
	if mode == Mode.JELLY:
		hover = Hover.new()
		hover.speed = randf_range(60.0, 120.0)
		hover.amplitude = max_speed * 20.0
		boid.add_child(hover)
		boid.apply_banking = false
		boid.max_turn_degrees = 10.0
		boid.damping = 0.2
		# Tilt the jelly so its forward (thrust) vector points mostly up.
		rotation.x = randf_range(0.35, 0.6) * PI
	else:
		var harmonic := Harmonic.new()
		harmonic.speed = randf_range(80.0, 120.0)
		harmonic.amplitude = randf_range(40.0, 60.0)
		harmonic.radius = randf_range(50.0, 150.0)
		harmonic.distance = randf_range(20.0, 80.0)
		boid.add_child(harmonic)

		var wander := NoiseWander.new()
		wander.radius = 400.0
		wander.distance = 100.0
		wander.noisiness = 0.15
		wander.weight = 0.6
		boid.add_child(wander)

	var avoid := TerrainAvoidance.new()
	avoid.min_clearance = head_scale * 2.0 + 200.0
	avoid.weight = 2.0
	boid.add_child(avoid)

	var constrain := Constrain.new()
	constrain.radius = 4000.0
	boid.add_child(constrain)


func _make_tentacle(head: Node3D, local_pos: Vector3, along: Vector3, theta: float) -> void:
	var pivot := Node3D.new()
	pivot.name = "TentaclePivot%d" % _pivots.size()
	head.add_child(pivot)
	pivot.position = local_pos
	# Orient the pivot so its -Z axis points along the tentacle direction.
	var up := Vector3.UP if absf(along.dot(Vector3.UP)) < 0.99 else Vector3.RIGHT
	pivot.basis = Basis.looking_at(along, up)
	_pivots.append(pivot)
	_pivot_bases.append(pivot.basis)
	_phases.append(theta)

	var bones: Array[Node3D] = []
	var seg_size := tenticle_scale
	var dist := seg_size
	var base_hue := color_b.h
	for s in tenticle_segments:
		var seg := Node3D.new()
		seg.name = "Seg%d_%d" % [_pivots.size() - 1, s]
		add_child(seg)
		seg.global_position = pivot.global_position \
			+ (pivot.global_basis * Vector3.FORWARD) * dist
		var h := fmod(base_hue + 0.08 * s, 1.0)
		var seg_alpha := 0.45 if mode == Mode.JELLY else 0.65
		# Elongated along the chain and overlapping, so the tentacle reads
		# as a continuous tapering ribbon rather than a string of beads.
		seg.add_child(FormsMeshes.make_part(FormsMeshes.box(),
			FormsMeshes.make_material(Color.from_hsv(h, 0.9, 1.0), 1.0, seg_alpha),
			Vector3(seg_size * 0.6, seg_size * 0.6, seg_size * 1.5)))
		bones.append(seg)
		var next_size := seg_size * 0.82
		dist += (seg_size + next_size) * 0.62
		seg_size = next_size

	var spine := SpineAnimator.new()
	spine.bond_damping = 8.0
	spine.angular_bond_damping = 10.0
	add_child(spine)
	spine.assign_bones(pivot, bones)


func _process(delta: float) -> void:
	_time += delta
	var sway_amp := 0.45 if mode == Mode.FLOWER else 0.28
	var sway_speed := 1.2 if mode == Mode.FLOWER else 2.2
	for i in _pivots.size():
		_pivots[i].basis = _pivot_bases[i] \
			* Basis(Vector3.RIGHT, sin(_time * sway_speed + _phases[i]) * sway_amp)
	if mode == Mode.JELLY and hover != null:
		var pulse := 1.0 + 0.15 * sin(hover.theta)
		_bell.scale = Vector3(head_scale * pulse, head_scale * 0.55 / pulse, head_scale * pulse)
