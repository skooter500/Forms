class_name Plants
extends Object
## Factory helpers for the flora and artefacts spawned by the GenesisDevice:
## thc plants (WorldGenerator.GenerateFlora spawned these on tiles in Unity)
## and the rotating disco-ball artefact (DiscoBall.prefab + RotateMe).


static func make_thc(plant_size: float = 150.0) -> Node3D:
	var root := Node3D.new()
	root.name = "Thc"
	var mat := StandardMaterial3D.new()
	var c := Color(0.2, randf_range(0.7, 1.0), 0.25)
	mat.albedo_color = c
	mat.emission_enabled = true
	mat.emission = c
	mat.emission_energy_multiplier = 0.35

	var stem := MeshInstance3D.new()
	var cyl := CylinderMesh.new()
	cyl.height = 1.0
	cyl.top_radius = 0.02
	cyl.bottom_radius = 0.04
	cyl.radial_segments = 6
	cyl.material = mat
	stem.mesh = cyl
	stem.scale = Vector3.ONE * plant_size
	stem.position.y = plant_size * 0.5
	root.add_child(stem)

	# Radial serrated leaves, drooping like the real thing.
	var leaf_mesh := SphereMesh.new()
	leaf_mesh.radius = 0.5
	leaf_mesh.height = 1.0
	leaf_mesh.radial_segments = 6
	leaf_mesh.rings = 3
	leaf_mesh.material = mat
	var leaves := randi_range(6, 9)
	for i in leaves:
		var leaf := MeshInstance3D.new()
		leaf.mesh = leaf_mesh
		var theta := TAU * i / leaves
		leaf.position = Vector3(0, plant_size * randf_range(0.8, 1.0), 0)
		leaf.scale = Vector3(plant_size * 0.08, plant_size * 0.02, plant_size * 0.5)
		leaf.rotation = Vector3(deg_to_rad(randf_range(15, 35)), theta, 0)
		root.add_child(leaf)
	return root


static func make_disco_ball(ball_radius: float = 150.0) -> Node3D:
	var root := Node3D.new()
	root.name = "DiscoBall"
	var mesh := MeshInstance3D.new()
	var sphere := SphereMesh.new()
	sphere.radius = ball_radius
	sphere.height = ball_radius * 2.0
	sphere.radial_segments = 16
	sphere.rings = 8
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color(0.9, 0.9, 1.0)
	mat.metallic = 1.0
	mat.roughness = 0.05
	mat.emission_enabled = true
	mat.emission = Color(0.6, 0.7, 1.0)
	mat.emission_energy_multiplier = 0.5
	sphere.material = mat
	mesh.mesh = sphere
	root.add_child(mesh)

	var rot := RotateMe.new()
	rot.speed = 0.05
	root.add_child(rot)
	return root
