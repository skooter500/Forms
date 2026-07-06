class_name CreatureGenerator
extends Node3D
## Port of BGE.Forms.CreatureGenerator: assembles a segmented creature whose
## part sizes follow a sine sweep. The head is a Boid with steering
## behaviours; body parts trail it via a SpineAnimator; some parts carry
## flapping fins driven by the Harmonic behaviour.
## Godot convention: the creature faces -Z, body trails towards +Z.

@export var num_parts := 10
@export var theta_start := 1.34
@export var frequency := 0.5
@export var gap := 0.0
@export var vertical_size := 200.0
@export var fin_parts: Array[int] = []
@export var fin_rotation_offset := 20.0
@export var fin_scale := 1.0
## Ray-like creatures: parts squashed vertically and widened.
@export var flatten := false
## Doves: travel in a straight line forever (Seek far target, upright) instead
## of wandering inside a constrain sphere.
@export var travel := false

@export var color_a := Color(0.2, 0.6, 1.0)
@export var color_b := Color(1.0, 0.4, 0.8)

@export var max_speed := 100.0
@export var max_force := 400.0

@export var harmonic_speed := 100.0
@export var harmonic_amplitude := 40.0
@export var harmonic_radius := 100.0
@export var harmonic_distance := 50.0

@export var constrain_radius := 6000.0

var boid: Boid


func _ready() -> void:
	build()


func build() -> void:
	if boid != null:
		return

	# Rainbow gradient along the body between the two palette hues, on
	# translucent glowing boxes — the signature look of the original
	# creature prefabs (CreatureColours shader).
	var mats: Array[StandardMaterial3D] = []
	for i in num_parts:
		var t := float(i) / maxf(num_parts - 1, 1.0)
		var h := fmod(lerpf(color_a.h, color_a.h + 0.6, t), 1.0)
		mats.append(FormsMeshes.make_material(Color.from_hsv(h, 0.9, 1.0), 1.1))
	var mat_b := FormsMeshes.make_material(color_b, 1.1)

	# Part sizes and local positions (Unity CreateCreatureParams, forward flipped).
	var theta := theta_start
	var theta_inc := PI * frequency / num_parts
	var last_size := 0.0
	var pos := Vector3.ZERO
	var sizes: Array[float] = []
	var positions: Array[Vector3] = []
	for i in num_parts:
		var part_size := vertical_size * absf(sin(theta))
		part_size = maxf(part_size, vertical_size * 0.05)
		theta += theta_inc
		pos += Vector3(0, 0, (last_size + part_size) / 2.0 + gap)
		last_size = part_size
		sizes.append(part_size)
		positions.append(pos)

	# Head: the boid with its steering behaviours.
	boid = Boid.new()
	boid.name = "Head"
	boid.max_speed = max_speed
	boid.max_force = max_force
	add_child(boid)
	boid.position = positions[0]

	var harmonic := Harmonic.new()
	harmonic.speed = harmonic_speed
	harmonic.amplitude = harmonic_amplitude
	harmonic.radius = harmonic_radius
	harmonic.distance = harmonic_distance
	harmonic.direction = Harmonic.Axis.VERTICAL
	harmonic.weight = 1.0
	boid.add_child(harmonic)

	var avoid := TerrainAvoidance.new()
	avoid.min_clearance = vertical_size * 2.0 + 200.0
	avoid.weight = 2.0
	boid.add_child(avoid)

	if travel:
		# Dove behaviour: pick a target far away in the facing direction and
		# just keep going (the Unity DoveController re-targets forever).
		boid.keep_upright = true
		var seek := Seek.new()
		seek.target = global_position + -global_basis.z * 1_000_000.0
		boid.add_child(seek)
	else:
		var wander := NoiseWander.new()
		wander.radius = 400.0
		wander.distance = 100.0
		wander.noisiness = 0.15
		wander.weight = 0.6
		boid.add_child(wander)

		var constrain := Constrain.new()
		constrain.radius = constrain_radius
		constrain.weight = 1.0
		boid.add_child(constrain)

	# Body parts.
	var bones: Array[Node3D] = []
	var fin_number := 0
	for i in num_parts:
		var holder: Node3D
		if i == 0:
			holder = boid
		else:
			holder = Node3D.new()
			holder.name = "Part%d" % i
			add_child(holder)
			holder.position = positions[i]
			bones.append(holder)

		var part_scale := Vector3(sizes[i] * 1.8, sizes[i] * 0.35, sizes[i]) if flatten \
			else Vector3.ONE * sizes[i]
		holder.add_child(FormsMeshes.make_part(FormsMeshes.box(), mats[i], part_scale))

		if fin_parts.has(i):
			for side in [-1.0, 1.0]:
				holder.add_child(_make_fin(sizes[i], side, fin_number, harmonic, mat_b))
			fin_number += 1

	var spine := SpineAnimator.new()
	spine.name = "SpineAnimator"
	add_child(spine)
	spine.assign_bones(boid, bones)


func _make_fin(part_size: float, side: float, fin_number: int, harmonic: Harmonic, mat: Material) -> FinAnimator:
	var scale := part_size / (fin_number + 1) * fin_scale
	var pivot := FinAnimator.new()
	pivot.name = "Fin%s%d" % ["L" if side < 0 else "R", fin_number]
	pivot.position = Vector3(side * part_size * 0.45, 0, 0)
	pivot.harmonic = harmonic
	pivot.amplitude = 55.0
	pivot.wigglyness = 1.0
	pivot.rotation_offset = 220.0 - fin_number * fin_rotation_offset
	pivot.flip_direction = side > 0
	pivot.axis = FinAnimator.Axis.Z

	# Wing: a long thin swept panel plus a shorter tip panel angled further
	# back, so it reads as a wing rather than a pancake.
	var span := scale * 1.3
	var chord := scale * 0.55
	var thin := scale * 0.05
	var wing := FormsMeshes.make_part(FormsMeshes.box(), mat, Vector3(span, thin, chord))
	wing.position = Vector3(side * span * 0.5, 0, 0)
	wing.rotation.y = -side * 0.18   # sweep back
	pivot.add_child(wing)

	var tip := FormsMeshes.make_part(FormsMeshes.box(), mat,
		Vector3(span * 0.55, thin, chord * 0.55))
	tip.position = Vector3(side * (span * 0.95), 0, chord * 0.28)
	tip.rotation.y = -side * 0.45
	pivot.add_child(tip)
	return pivot
