class_name Mother
extends Node3D
## Port of BGE.Forms.Mother: keeps a population of procedural creatures alive
## around the player — spawning species in front, culling ones left far
## behind. The species roster mirrors the paradiso1stbirthday scene:
## schools (plain/chaotic/rainbow/sardine), spine creatures (elasmosaurus,
## snake, ray, sperm, mermaid + random forms), doves, tenticle creatures,
## jellies, sandworms, tardigrade schools and V-formations.

@export var max_creatures := 8
@export var spawn_interval := 1.0
@export var fov_degrees := 20.0
@export var spawn_min_distance := 1500.0
@export var spawn_max_distance := 4000.0
@export var cull_distance := 9000.0

static var instance: Mother

var player: Node3D


func _ready() -> void:
	instance = self


## PlayerController.PickNewTarget: a creature for the camera to follow.
## Prefer things that actually roam (not rooted/ground species).
func get_random_creature() -> Node3D:
	var candidates := _alive.filter(func(c): return is_instance_valid(c) and not c is Sandworm)
	if candidates.is_empty():
		return null
	var creature: Node3D = candidates.pick_random()
	# Follow the head boid if there is one, so the camera tracks motion.
	if creature is CreatureGenerator and (creature as CreatureGenerator).boid != null:
		return (creature as CreatureGenerator).boid
	if creature is TenticleCreature and (creature as TenticleCreature).boid != null:
		return (creature as TenticleCreature).boid
	return creature

var _alive: Array[Node3D] = []
var _timer := 0.0
var _species_bag: Array[String] = []


func _process(delta: float) -> void:
	if player == null or WorldGenerator.instance == null:
		return
	_timer += delta
	if _timer < spawn_interval:
		return
	_timer = 0.0

	# Cull creatures far behind the player (Unity: angle > 80 deg and far).
	for i in range(_alive.size() - 1, -1, -1):
		var c := _alive[i]
		var to_creature := c.global_position - player.global_position
		var dist := to_creature.length()
		var behind := rad_to_deg((-player.global_basis.z).angle_to(to_creature)) > 80.0 and dist > 3000.0
		if behind or dist > cull_distance:
			c.queue_free()
			_alive.remove_at(i)

	if _alive.size() < max_creatures:
		_alive.append(_spawn_next_species())


## Shuffled-bag species selection so every type shows up regularly.
const SPECIES := [
	"school", "chaotic_school", "rainbow_school", "sardine_school",
	"elasmosaurus", "snake", "ray", "sperm", "mermaid", "random_form",
	"dove", "tenticle_creature", "jelly", "sandworm",
	"tardigrade_school", "formation",
]


## Debug helper: one of every species in a ring around the player.
func spawn_zoo(only: Array = []) -> void:
	var species: Array = only if not only.is_empty() else SPECIES
	var n := species.size()
	for i in n:
		_species_bag = [species[i]]
		var save_min := spawn_min_distance
		var save_max := spawn_max_distance
		var save_fov := fov_degrees
		spawn_min_distance = (700.0 if not only.is_empty() else 900.0) + 500.0 * (i % 4)
		spawn_max_distance = spawn_min_distance + 100.0
		fov_degrees = 60.0
		_alive.append(_spawn_next_species())
		spawn_min_distance = save_min
		spawn_max_distance = save_max
		fov_degrees = save_fov


func _next_species() -> String:
	if _species_bag.is_empty():
		_species_bag.assign(SPECIES)
		_species_bag.shuffle()
	return _species_bag.pop_back()


func _spawn_next_species() -> Node3D:
	var kind := _next_species()
	var creature: Node3D
	var on_ground := false
	match kind:
		"school":
			creature = _make_school(1.0, false, randi_range(25, 45), randf_range(10, 30))
		"chaotic_school":
			creature = _make_school(3.0, false, randi_range(20, 35), randf_range(10, 25))
		"rainbow_school":
			creature = _make_school(1.0, true, randi_range(25, 45), randf_range(10, 30))
		"sardine_school":
			creature = _make_school(1.2, false, 60, randf_range(6, 12))
		"elasmosaurus":
			creature = _spine(10, 1.34, 0.5, randf_range(150, 250), 0.02, [3, 6], 20.0)
		"snake":
			creature = _spine(15, 1.5, 0.5, randf_range(90, 160), -0.04, [], 20.0)
		"ray":
			creature = _spine(10, 3.86, 8.5, randf_range(150, 250), -0.01, [4, 7], 90.0)
			(creature as CreatureGenerator).flatten = true
			(creature as CreatureGenerator).fin_scale = 1.6
		"sperm":
			creature = _spine(7, 3.43, 0.9, randf_range(40, 80), 0.0, [], 20.0)
			(creature as CreatureGenerator).max_speed = randf_range(140, 200)
		"mermaid":
			creature = _spine(5, 3.68, 1.6, randf_range(80, 140), 0.0, [1], 20.0)
		"random_form":
			creature = _spine(randi_range(7, 15), randf_range(0.5, 3.8),
				randf_range(0.5, 2.5), randf_range(80, 300),
				randf_range(-0.05, 0.05), [randi_range(1, 4)], 20.0)
		"dove":
			creature = _spine(5, 2.0, 1.2, randf_range(60, 110), 0.0, [1], 20.0)
			var d := creature as CreatureGenerator
			d.travel = true
			d.fin_scale = 2.5
			d.max_speed = randf_range(120, 180)
		"tenticle_creature":
			creature = _tenticle(TenticleCreature.Mode.SWIM)
		"jelly":
			creature = _tenticle(TenticleCreature.Mode.JELLY)
		"sandworm":
			var w := Sandworm.new()
			w.name = "Sandworm"
			w.radius = randf_range(30.0, 90.0)
			w.body_segments = randi_range(8, 14)
			creature = w
			on_ground = true
		"tardigrade_school":
			var t := SchoolGenerator.new()
			t.name = "TardigradeSchool"
			t.boid_count = randi_range(10, 16)
			t.member_parts = 3
			t.radius = randf_range(400, 700)
			t.fish_size = randf_range(30, 50)
			t.fish_speed = randf_range(60, 100)
			t.color = _palette()[0]
			creature = t
		"formation":
			var f := FormationGenerator.new()
			f.name = "Formation"
			f.side_width = randi_range(1, 3)
			var preset := ["snake", "ray", "mermaid"].pick_random() as String
			f.make_creature = func() -> CreatureGenerator:
				var vs := randf_range(90, 160)
				match preset:
					"ray":
						var r := _spine(10, 3.86, 8.5, vs, -0.01, [4, 7], 90.0)
						r.flatten = true
						return r
					"mermaid":
						return _spine(5, 3.68, 1.6, vs, 0.0, [1], 20.0)
					_:
						return _spine(15, 1.5, 0.5, vs, -0.04, [], 20.0)
			f.gap = randf_range(300, 500)
			creature = f
		_:
			creature = _spine(10, 1.34, 0.5, 150, 0.02, [3, 6], 20.0)

	var pos := _find_place()
	if on_ground:
		pos.y = WorldGenerator.instance.sample_pos(pos.x, pos.z) + 50.0
	# Position before adding: _ready builds the creature in place (constrain
	# centres and spine offsets are captured from the spawn position).
	creature.position = pos
	creature.rotate_y(randf_range(0, TAU))
	add_child(creature)
	return creature


func _find_place() -> Vector3:
	var wg := WorldGenerator.instance
	var r := Vector3.FORWARD * randf_range(spawn_min_distance, spawn_max_distance)
	r = r.rotated(Vector3.UP, deg_to_rad(randf_range(-fov_degrees, fov_degrees)))
	var pos: Vector3 = player.global_transform * r
	var ground: float = wg.sample_pos(pos.x, pos.z)
	var ceiling: float = wg.surface_height - 500.0
	pos.y = clampf(randf_range(ground + 500.0, ground + 3000.0), ground + 300.0, ceiling)
	return pos


static func _palette() -> Array[Color]:
	var h := randf()
	var h2 := fmod(h + randf_range(0.25, 0.5), 1.0)
	return [
		Color.from_hsv(h, randf_range(0.7, 1.0), 1.0),
		Color.from_hsv(h2, randf_range(0.7, 1.0), 1.0),
	]


func _spine(parts: int, theta: float, freq: float, vs: float, gap_frac: float,
		fins: Array, fin_rot: float) -> CreatureGenerator:
	var c := CreatureGenerator.new()
	c.name = "Creature"
	c.num_parts = parts
	c.theta_start = theta
	c.frequency = freq
	c.vertical_size = vs
	c.gap = vs * gap_frac
	var fp: Array[int] = []
	fp.assign(fins)
	c.fin_parts = fp
	c.fin_rotation_offset = fin_rot

	var colors := _palette()
	c.color_a = colors[0]
	c.color_b = colors[1]

	c.max_speed = randf_range(60.0, 140.0)
	c.max_force = c.max_speed * 4.0
	c.harmonic_speed = randf_range(80.0, 120.0)
	c.harmonic_amplitude = randf_range(40.0, 80.0)
	c.harmonic_radius = randf_range(50.0, 200.0)
	c.harmonic_distance = randf_range(20.0, 100.0)
	return c


func _tenticle(mode: TenticleCreature.Mode) -> TenticleCreature:
	var t := TenticleCreature.new()
	t.name = "TenticleCreature"
	t.mode = mode
	t.num_tenticles = randi_range(6, 10)
	t.head_scale = randf_range(80.0, 180.0)
	t.tenticle_scale = t.head_scale * randf_range(0.2, 0.3)
	t.tenticle_segments = randi_range(5, 8)
	var colors := _palette()
	t.color_a = colors[0]
	t.color_b = colors[1]
	t.max_speed = randf_range(40.0, 80.0) if mode == TenticleCreature.Mode.SWIM else randf_range(20.0, 40.0)
	return t


func _make_school(chaos: float, rainbow: bool, count: int, size: float) -> SchoolGenerator:
	var s := SchoolGenerator.new()
	s.name = "School"
	s.boid_count = count
	s.radius = randf_range(500.0, 1000.0)
	s.fish_size = size
	s.fish_speed = randf_range(100.0, 200.0)
	s.chaos = chaos
	s.rainbow = rainbow
	s.color = _palette()[0]
	return s
