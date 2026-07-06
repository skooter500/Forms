class_name SchoolGenerator
extends Node3D
## Port of BGE.Forms.SchoolGenerator/School: a flock of small fish boids
## using classic cohesion/separation/alignment plus a constrain sphere.

@export var boid_count := 40
@export var radius := 800.0
@export var neighbour_distance := 300.0
@export var fish_size := 20.0
@export var fish_speed := 150.0
@export var color := Color(0.4, 0.9, 1.0)
## >1 gives every member a trailing segmented body (tardigrade schools).
@export var member_parts := 1
## Multiplies wander jitter ("chaoticSchool" species).
@export var chaos := 1.0
## lifeColoursSchool: every member gets its own hue.
@export var rainbow := false

var boids: Array[Boid] = []


func _ready() -> void:
	var mat := FormsMeshes.make_material(color, 0.6)

	for i in boid_count:
		var fish := Boid.new()
		fish.name = "Fish%d" % i
		fish.max_speed = fish_speed * randf_range(0.9, 1.1)
		fish.max_force = fish_speed * 4.0
		fish.school = self
		fish.tag_neighbours_dither = 0.2

		var cohesion := Cohesion.new()
		cohesion.weight = 2.0
		fish.add_child(cohesion)

		var separation := Separation.new()
		separation.weight = 60.0
		fish.add_child(separation)

		var alignment := Alignment.new()
		alignment.weight = 1.5
		fish.add_child(alignment)

		var wander := JitterWander.new()
		wander.radius = 30.0
		wander.jitter = 150.0 * chaos
		wander.distance = 60.0
		wander.weight = 2.0 * chaos
		fish.add_child(wander)

		var constrain := Constrain.new()
		constrain.centre_on_position = false
		constrain.radius = radius
		constrain.weight = 1.0
		fish.add_child(constrain)

		var avoid := TerrainAvoidance.new()
		avoid.min_clearance = 200.0
		avoid.weight = 2.0
		fish.add_child(avoid)

		var member_mat := mat
		if rainbow:
			member_mat = FormsMeshes.make_material(Color.from_hsv(randf(), 0.8, 1.0), 0.6)

		# Tardigrades are stubby; plain fish are elongated.
		var stubby := member_parts > 1
		var member_scale := Vector3(fish_size * 0.7, fish_size * 0.6, fish_size) if stubby \
			else Vector3(fish_size * 0.4, fish_size * 0.4, fish_size)
		fish.add_child(FormsMeshes.make_part(FormsMeshes.box(), member_mat, member_scale))

		add_child(fish)
		fish.global_position = global_position + _random_inside_unit_sphere() * radius * 0.5
		fish.rotate_y(randf_range(0, TAU))
		fish.velocity = -fish.global_basis.z * fish.max_speed * 0.5
		constrain.centre = global_position
		boids.append(fish)

		# Trailing body segments for multi-part members.
		if member_parts > 1:
			var bones: Array[Node3D] = []
			for p in range(1, member_parts):
				var seg := Node3D.new()
				add_child(seg)
				seg.global_position = fish.global_position + fish.global_basis.z * fish_size * p
				var s := fish_size * (1.0 - 0.15 * p)
				var seg_scale := Vector3(s * 0.7, s * 0.6, s) if stubby else Vector3(s * 0.4, s * 0.4, s)
				seg.add_child(FormsMeshes.make_part(FormsMeshes.box(), member_mat, seg_scale))
				bones.append(seg)
			var spine := SpineAnimator.new()
			spine.bond_damping = 8.0
			add_child(spine)
			spine.assign_bones(fish, bones)


	# TrailRenderer stand-in: glowing ribbons behind every member.
	var trails := SchoolTrails.new()
	trails.school = self
	trails.width = fish_size * 0.3
	add_child(trails)


static func _random_inside_unit_sphere() -> Vector3:
	while true:
		var v := Vector3(randf_range(-1, 1), randf_range(-1, 1), randf_range(-1, 1))
		if v.length_squared() <= 1.0:
			return v
	return Vector3.ZERO
