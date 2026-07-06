extends Node3D
## Entry point for the Godot port of Infinite Forms (paradiso1stbirthday
## scene, minus XR). Builds the environment, infinite terrain, creature
## spawner and fly-cam player in code — the world is entirely procedural.


func _ready() -> void:
	add_child(preload("res://scripts/debug_screenshot.gd").new())
	_setup_environment()

	var world := WorldGenerator.new()
	world.name = "WorldGenerator"
	add_child(world)

	var player := Player.new()
	player.name = "Player"
	add_child(player)
	var ground := world.sample_pos(0.0, 0.0)
	player.global_position = Vector3(0, ground + 1000.0, 0)
	world.player = player

	var mother := Mother.new()
	mother.name = "Mother"
	mother.player = player
	if OS.get_cmdline_user_args().has("--stress"):
		mother.spawn_interval = 0.1
		mother.max_creatures = 32
	add_child(mother)
	if OS.get_cmdline_user_args().has("--zoo"):
		mother.max_creatures = 40
		mother.spawn_zoo()
	if OS.get_cmdline_user_args().has("--jellyzoo"):
		mother.max_creatures = 40
		mother.spawn_zoo(["jelly", "tenticle_creature", "jelly", "tenticle_creature"])

	# Plants on the ground (trees, thc, tenticle flowers)...
	var flora := GenesisDevice.new()
	flora.name = "GenesisDeviceGround"
	flora.player = player
	flora.positioning = GenesisDevice.Positioning.GROUND
	flora.gap = 900.0
	flora.radius = 5
	flora.threshold = 0.6
	flora.factories = [
		func() -> Node3D:
			var tree := TreeGen.new()
			tree.size = randf_range(250.0, 600.0)
			tree.depth = randi_range(2, 3)
			tree.color = Color.from_hsv(randf_range(0.6, 0.85), 0.5, 0.9)
			return tree,
		func() -> Node3D: return Plants.make_thc(randf_range(100.0, 250.0)),
		func() -> Node3D:
			var flower := TenticleCreature.new()
			flower.mode = TenticleCreature.Mode.FLOWER
			flower.num_tenticles = randi_range(6, 10)
			flower.head_scale = randf_range(60.0, 140.0)
			flower.tenticle_scale = flower.head_scale * 0.25
			flower.color_a = Color.from_hsv(randf(), 0.8, 1.0)
			flower.color_b = Color.from_hsv(randf(), 0.8, 1.0)
			return flower,
	]
	add_child(flora)

	# ...and mysterious artefacts floating in the air.
	var artefacts := GenesisDevice.new()
	artefacts.name = "GenesisDeviceAir"
	artefacts.player = player
	artefacts.positioning = GenesisDevice.Positioning.AIR
	artefacts.gap = 3500.0
	artefacts.radius = 2
	artefacts.threshold = 0.72
	artefacts.air_height = 2500.0
	artefacts.factories = [
		func() -> Node3D: return Plants.make_disco_ball(randf_range(100.0, 250.0)),
	]
	add_child(artefacts)


func _setup_environment() -> void:
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	# The original fades to black at the horizon; keep fog and backdrop
	# matched so tile edges disappear into it.
	env.background_color = Color(0.01, 0.008, 0.02)

	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.35, 0.32, 0.5)
	env.ambient_light_energy = 1.2

	env.fog_enabled = true
	env.fog_mode = Environment.FOG_MODE_EXPONENTIAL
	env.fog_light_color = Color(0.01, 0.008, 0.02)
	env.fog_density = 0.0004
	env.fog_sky_affect = 0.0

	env.glow_enabled = true
	env.glow_bloom = 0.1
	env.glow_hdr_threshold = 0.9
	env.tonemap_mode = Environment.TONE_MAPPER_ACES

	var we := WorldEnvironment.new()
	we.environment = env
	add_child(we)

	var light := DirectionalLight3D.new()
	light.light_energy = 0.6
	light.light_color = Color(0.7, 0.8, 1.0)
	light.rotation_degrees = Vector3(-45, 30, 0)
	light.shadow_enabled = false
	add_child(light)
