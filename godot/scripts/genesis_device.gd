class_name GenesisDevice
extends Node3D
## Port of BGE.Forms.GenesisDevice: scatters plants/artefacts on a grid of
## noise-thresholded cells around the player, pooling instances that fall
## outside the active square. Two live in the scene: a Ground device for
## flora and an Air device for floating artefacts.

enum Positioning { GROUND, AIR }

@export var positioning: Positioning = Positioning.GROUND
@export var radius := 5          # cells around the player
@export var gap := 800.0         # cell size
@export var threshold := 0.62
@export var spawns_per_tick := 2
@export var air_height := 1500.0

var player: Node3D
## Array of Callables, each returning a fresh plant/artefact Node3D. Cycled
## like the Unity prefabs array.
var factories: Array[Callable] = []

var _alive := {}     # "x_z" -> Node3D
var _dead: Array[Node3D] = []
var _next_factory := 0
var _noise := FastNoiseLite.new()
var _timer := 0.0


func _ready() -> void:
	_noise.noise_type = FastNoiseLite.TYPE_PERLIN
	_noise.frequency = 1.0
	_noise.seed = randi()


func _sample(x: float, z: float) -> float:
	return _noise.get_noise_2d(x * 0.0004, z * 0.0004) * 0.5 + 0.5


func _process(delta: float) -> void:
	if player == null or WorldGenerator.instance == null or factories.is_empty():
		return
	_timer += delta
	if _timer < 0.2:
		return
	_timer = 0.0

	var wg := WorldGenerator.instance
	var centre := Vector3(roundf(player.global_position.x / gap), 0, roundf(player.global_position.z / gap)) * gap
	var bottom_left := centre - Vector3(gap, 0, gap) * radius
	var top_right := centre + Vector3(gap, 0, gap) * radius

	# Retire plants outside the active square into the pool.
	for key in _alive.keys():
		var plant: Node3D = _alive[key]
		var p := plant.global_position
		if p.x < bottom_left.x or p.x > top_right.x or p.z < bottom_left.z or p.z > top_right.z:
			plant.visible = false
			plant.set_process(false)
			_dead.append(plant)
			_alive.erase(key)

	var spawned := 0
	for row in radius * 2 + 1:
		for col in radius * 2 + 1:
			var pos := bottom_left + Vector3(col * gap, 0, row * gap)
			var key := "%d_%d" % [int(pos.x), int(pos.z)]
			if _alive.has(key):
				continue
			if _sample(pos.x, pos.z) <= threshold:
				continue
			var ground := wg.sample_pos(pos.x, pos.z)
			pos.y = ground if positioning == Positioning.GROUND else ground + air_height
			_alive[key] = _spawn(pos)
			spawned += 1
			if spawned >= spawns_per_tick:
				return


func _spawn(pos: Vector3) -> Node3D:
	var plant: Node3D
	if not _dead.is_empty():
		plant = _dead.pop_back()
		plant.visible = true
		plant.set_process(true)
	else:
		plant = factories[_next_factory].call()
		_next_factory = (_next_factory + 1) % factories.size()
		add_child(plant)
	plant.global_position = pos
	plant.rotate_y(randf_range(0, TAU))
	return plant
