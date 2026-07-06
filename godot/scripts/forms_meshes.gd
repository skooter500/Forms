class_name FormsMeshes
extends Object
## Shared unit meshes. Every creature body part is one of these with a
## per-instance material_override and scale — far cheaper than a mesh
## resource per part.

static var _sphere: SphereMesh
static var _sphere_lod: SphereMesh
static var _box: BoxMesh


static func sphere() -> SphereMesh:
	if _sphere == null:
		_sphere = SphereMesh.new()
		_sphere.radius = 0.5
		_sphere.height = 1.0
		_sphere.radial_segments = 24
		_sphere.rings = 12
	return _sphere


static func sphere_lod() -> SphereMesh:
	if _sphere_lod == null:
		_sphere_lod = SphereMesh.new()
		_sphere_lod.radius = 0.5
		_sphere_lod.height = 1.0
		_sphere_lod.radial_segments = 12
		_sphere_lod.rings = 6
	return _sphere_lod


static func box() -> BoxMesh:
	if _box == null:
		_box = BoxMesh.new()
	return _box


static func make_part(mesh: Mesh, mat: Material, scale: Vector3) -> MeshInstance3D:
	var mi := MeshInstance3D.new()
	mi.mesh = mesh
	mi.material_override = mat
	mi.scale = scale
	mi.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	return mi


## The signature Forms look: translucent glowing plastic.
static func make_material(c: Color, emission_energy: float = 0.5, alpha: float = 0.65) -> StandardMaterial3D:
	var m := StandardMaterial3D.new()
	m.albedo_color = Color(c.r, c.g, c.b, alpha)
	if alpha < 1.0:
		m.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	m.emission_enabled = true
	m.emission = c
	m.emission_energy_multiplier = emission_energy
	m.roughness = 0.4
	return m
