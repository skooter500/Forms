class_name Sandworm
extends Node3D
## Port of SandWorm.cs. The Unity original was a hinge-joint physics chain of
## black boxes writhing on the terrain; this port drives the head with
## steering (GroundHug + vertical Harmonic for rearing) and lets the
## segmented body trail through a SpineAnimator. Same silhouette: dark
## square plates, tapered at head and tail.

@export var body_segments := 10
@export var radius := 50.0
@export var headtail := 2
@export var worm_speed := 80.0


func _ready() -> void:
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(0.05, 0.05, 0.05)
	mat.emission_enabled = true
	mat.emission = Color(0.4, 0.1, 0.0)
	mat.emission_energy_multiplier = 0.25
	mat.roughness = 0.9

	var boid := Boid.new()
	boid.name = "Head"
	boid.max_speed = worm_speed
	boid.max_force = worm_speed * 5.0
	add_child(boid)

	var hug := GroundHug.new()
	hug.hover_height = radius
	hug.weight = 2.0
	boid.add_child(hug)

	var rear := Harmonic.new()
	rear.direction = Harmonic.Axis.VERTICAL
	rear.speed = randf_range(40.0, 80.0)
	rear.amplitude = randf_range(50.0, 90.0)
	rear.radius = radius * 3.0
	rear.distance = radius * 2.0
	boid.add_child(rear)

	var wander := NoiseWander.new()
	wander.radius = 300.0
	wander.distance = 80.0
	wander.noisiness = 0.2
	wander.weight = 1.2
	boid.add_child(wander)

	var constrain := Constrain.new()
	constrain.radius = 3000.0
	boid.add_child(constrain)

	var depth := radius * 0.4
	var bones: Array[Node3D] = []
	var pos := Vector3.ZERO
	for i in body_segments:
		var r := radius
		if i < headtail:
			r = radius * pow(0.6, headtail - i)
		if i > body_segments - headtail - 1:
			r = radius * pow(0.8, i - (body_segments - headtail - 1))

		var holder: Node3D
		if i == 0:
			holder = boid
		else:
			holder = Node3D.new()
			holder.name = "Segment%d" % i
			add_child(holder)
			holder.position = pos

		var mesh := MeshInstance3D.new()
		var box := BoxMesh.new()
		box.material = mat
		mesh.mesh = box
		mesh.scale = Vector3(r * 2.0, r * 2.0, depth)
		holder.add_child(mesh)

		if i > 0:
			bones.append(holder)
		pos += Vector3(0, 0, depth * 2.0)

	var spine := SpineAnimator.new()
	spine.bond_damping = 6.0
	spine.angular_bond_damping = 8.0
	add_child(spine)
	spine.assign_bones(boid, bones)
