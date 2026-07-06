class_name PartBatcher
extends MultiMeshInstance3D
## Draws all body parts of a creature (or all fish of a school) as ONE
## MultiMesh draw call with per-instance transform + colour, instead of one
## MeshInstance3D and one material per part. Each instance mirrors a source
## Node3D (a spine bone, fin pivot, fish boid...) combined with a fixed
## local offset/scale.

static var _shader: Shader

var _sources: Array[Node3D] = []
var _locals: Array[Transform3D] = []
var _colors: PackedColorArray = PackedColorArray()
var _mm := MultiMesh.new()


static func instanced_material(emission_energy: float) -> ShaderMaterial:
	if _shader == null:
		_shader = Shader.new()
		_shader.code = """
shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back;
uniform float emission_energy = 1.0;
void fragment() {
	ALBEDO = COLOR.rgb;
	ALPHA = COLOR.a;
	EMISSION = COLOR.rgb * emission_energy;
	ROUGHNESS = 0.4;
}
"""
	var m := ShaderMaterial.new()
	m.shader = _shader
	m.set_shader_parameter("emission_energy", emission_energy)
	return m


func _init(part_mesh: Mesh, emission_energy: float = 1.0) -> void:
	_mm.transform_format = MultiMesh.TRANSFORM_3D
	_mm.use_colors = true
	_mm.mesh = part_mesh
	multimesh = _mm
	material_override = instanced_material(emission_energy)
	cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF


func _ready() -> void:
	top_level = true
	global_transform = Transform3D.IDENTITY


## `local` is the part's offset/scale relative to its source node.
func add_part(source: Node3D, local: Transform3D, color: Color) -> void:
	_sources.append(source)
	_locals.append(local)
	_colors.append(color)


## Call once after all parts are added.
func commit() -> void:
	_mm.instance_count = _sources.size()
	for i in _sources.size():
		_mm.set_instance_color(i, _colors[i])
		if is_instance_valid(_sources[i]):
			_mm.set_instance_transform(i, _sources[i].global_transform * _locals[i])


func _process(_delta: float) -> void:
	for i in _sources.size():
		var s := _sources[i]
		if is_instance_valid(s):
			_mm.set_instance_transform(i, s.global_transform * _locals[i])


static func part_scale(scale: Vector3) -> Transform3D:
	return Transform3D(Basis.from_scale(scale), Vector3.ZERO)
