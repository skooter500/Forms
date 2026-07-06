class_name WorldGenerator
extends Node3D
## Port of BGE.Forms.WorldGenerator: infinite tiled terrain generated around
## the player from a stack of plateau-clamped Perlin samplers, plus the
## glowing "surface" ceiling high above. Values match paradiso1stbirthday.unity.

static var instance: WorldGenerator

@export var quads_per_tile := 10
@export var half_tile := 3
@export var cell_size := 400.0
@export var surface_height := 8000.0
@export var tiles_per_frame := 1
## Unity textureScaling 0.05: the life texture repeats ~20x per texture-size
## worth of cells, giving the fine pixelly trails of the original.
@export var texture_scaling := 0.05

var player: Node3D
var samplers: Array[PerlinSampler] = []

var _tiles := {}                # "x_z" -> Node3D
var _wanted := {}               # "x_z" -> Vector3 tile origin, rebuilt on player tile change
var _build_queue: Array[Vector3] = []
var _last_tile := Vector2i(1 << 30, 1 << 30)
var _ground_material: StandardMaterial3D
var _ceiling_material: StandardMaterial3D
var _texture_generator: GameOfLifeTexture


func _ready() -> void:
	instance = self

	# Sampler stack lifted from the paradiso1stbirthday scene.
	samplers.append(PerlinSampler.new(1.0, 0.0, 0.0, 0.0009, 1.0))
	samplers.append(PerlinSampler.new(1.0, 0.0, 4.76, 0.02, 1.0))
	samplers.append(PerlinSampler.new(0.5, 0.5, 8.38, 0.01, 1000.0))
	samplers.append(PerlinSampler.new(0.1, 0.461, 9.28, 0.3, 50.0))
	samplers.append(PerlinSampler.new(0.24, 0.71, 5.46, 0.21, 9000.0))

	_texture_generator = GameOfLifeTexture.new()
	add_child(_texture_generator)

	_ground_material = _make_material(1.6)
	_ceiling_material = _make_material(1.2)
	_ceiling_material.cull_mode = BaseMaterial3D.CULL_DISABLED


func _make_material(energy: float) -> StandardMaterial3D:
	# Dark faceted ground; all colour comes from the life texture's emission.
	var m := StandardMaterial3D.new()
	m.albedo_color = Color(0.06, 0.07, 0.13)
	m.emission_enabled = true
	# Multiply, not add: the emission colour tints the life texture instead
	# of washing the whole surface out.
	m.emission_operator = BaseMaterial3D.EMISSION_OP_MULTIPLY
	m.emission = Color.WHITE
	m.emission_texture = _texture_generator.texture
	m.emission_energy_multiplier = energy
	m.texture_filter = BaseMaterial3D.TEXTURE_FILTER_NEAREST
	m.roughness = 0.8
	return m


## Port of the DPadY ground-material switch in WorldGenerator.Update.
var _ground_style := 0

func cycle_ground_material() -> void:
	_ground_style = (_ground_style + 1) % 4
	match _ground_style:
		0:
			_ground_material.emission = Color.WHITE
			_ground_material.emission_energy_multiplier = 1.6
		1:
			_ground_material.emission = Color(0.3, 0.8, 1.5)
			_ground_material.emission_energy_multiplier = 2.2
		2:
			_ground_material.emission = Color(1.5, 0.5, 1.2)
			_ground_material.emission_energy_multiplier = 2.2
		3:
			_ground_material.emission = Color.WHITE
			_ground_material.emission_energy_multiplier = 0.6


func sample_pos(x: float, z: float) -> float:
	return sample_cell(x / cell_size, z / cell_size)


func sample_cell(x: float, z: float) -> float:
	var s := 0.0
	for sampler in samplers:
		s = sampler.operate(s, x, z)
	return s


func _process(_delta: float) -> void:
	if player == null:
		return
	var tile_size := quads_per_tile * cell_size
	var tile := Vector2i(floori(player.global_position.x / tile_size), floori(player.global_position.z / tile_size))
	if tile != _last_tile:
		_last_tile = tile
		_refresh_wanted(tile)
	# Build a few tiles per frame, nearest first.
	for i in tiles_per_frame:
		if _build_queue.is_empty():
			break
		var pos: Vector3 = _build_queue.pop_front()
		var key := "%d_%d" % [int(pos.x), int(pos.z)]
		if not _tiles.has(key):
			_tiles[key] = _generate_tile(pos)


func _refresh_wanted(tile: Vector2i) -> void:
	var tile_size := quads_per_tile * cell_size
	_wanted.clear()
	for x in range(-half_tile, half_tile + 1):
		for z in range(-half_tile, half_tile + 1):
			var pos := Vector3((tile.x + x) * tile_size, 0.0, (tile.y + z) * tile_size)
			_wanted["%d_%d" % [int(pos.x), int(pos.z)]] = pos

	# Drop tiles that are no longer wanted.
	for key in _tiles.keys():
		if not _wanted.has(key):
			_tiles[key].queue_free()
			_tiles.erase(key)

	# Queue missing tiles, nearest to the player first.
	_build_queue.clear()
	for key in _wanted:
		if not _tiles.has(key):
			_build_queue.append(_wanted[key])
	var p := player.global_position
	_build_queue.sort_custom(func(a, b): return p.distance_squared_to(a) < p.distance_squared_to(b))


func _generate_tile(pos: Vector3) -> Node3D:
	var tile := Node3D.new()
	tile.name = "Tile_%d_%d" % [int(pos.x), int(pos.z)]
	add_child(tile)
	# Tile origin is its corner; vertices span [0, tile_size].
	tile.position = pos

	var ground := MeshInstance3D.new()
	ground.mesh = _generate_mesh(pos)
	ground.material_override = _ground_material
	ground.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	tile.add_child(ground)

	var ceiling := MeshInstance3D.new()
	ceiling.mesh = _generate_ceiling_mesh(pos)
	ceiling.material_override = _ceiling_material
	ceiling.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	ceiling.position.y = surface_height
	tile.add_child(ceiling)
	return tile


func _generate_mesh(pos: Vector3) -> ArrayMesh:
	var st := SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_TRIANGLES)
	var cell_origin := pos / cell_size
	for z in quads_per_tile:
		for x in quads_per_tile:
			var corners := [Vector2(x, z), Vector2(x, z + 1), Vector2(x + 1, z + 1), Vector2(x + 1, z)]
			var verts: Array[Vector3] = []
			for c: Vector2 in corners:
				var y := sample_cell(cell_origin.x + c.x, cell_origin.z + c.y)
				verts.append(Vector3(c.x * cell_size, y, c.y * cell_size))
			# Two triangles, counter-clockwise seen from above (Godot front face).
			for idx in [0, 3, 1, 3, 2, 1]:
				st.set_uv(_make_uv(cell_origin, corners[idx]))
				st.add_vertex(verts[idx])
	st.index()
	st.generate_normals()
	return st.commit()


func _make_uv(cell_origin: Vector3, corner: Vector2) -> Vector2:
	var s := float(_texture_generator.size) * texture_scaling
	return Vector2((cell_origin.x + corner.x) / s, (cell_origin.z + corner.y) / s)


func _generate_ceiling_mesh(pos: Vector3) -> ArrayMesh:
	var st := SurfaceTool.new()
	st.begin(Mesh.PRIMITIVE_TRIANGLES)
	var t := quads_per_tile * cell_size
	var cell_origin := pos / cell_size
	var corners := [Vector2(0, 0), Vector2(0, 1), Vector2(1, 1), Vector2(1, 0)]
	# Wound to face downward (visible from below).
	for idx in [0, 1, 3, 3, 1, 2]:
		var c: Vector2 = corners[idx]
		st.set_uv(_make_uv(cell_origin, c * quads_per_tile))
		st.add_vertex(Vector3(c.x * t, 0.0, c.y * t))
	st.index()
	st.generate_normals()
	return st.commit()
