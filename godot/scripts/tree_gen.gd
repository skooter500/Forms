class_name TreeGen
extends Node3D
## Port of TreeGen.cs: recursive fractal tree — cylinder branches with sphere
## nodes, one vertical child plus `children` angled children per level.
## Like the Unity version (CombineMeshes) everything is baked into a single
## ArrayMesh.

@export var size := 400.0
@export var angle := 30.0
@export var branch_ratio := 0.4
@export var depth := 3
@export var children := 3
@export var stocastic := true
@export var color := Color(0.95, 0.95, 1.0)

var _st: SurfaceTool
var _branch_mesh: CylinderMesh
var _node_mesh: SphereMesh


func _ready() -> void:
	_branch_mesh = CylinderMesh.new()
	_branch_mesh.height = 1.0
	_branch_mesh.top_radius = 0.1
	_branch_mesh.bottom_radius = 0.13
	# Six segments: the chunky hexagonal prisms of the original tree parts.
	_branch_mesh.radial_segments = 6
	_branch_mesh.rings = 1

	_node_mesh = SphereMesh.new()
	_node_mesh.radius = 0.15
	_node_mesh.height = 0.3
	_node_mesh.radial_segments = 8
	_node_mesh.rings = 4

	_st = SurfaceTool.new()
	_st.begin(Mesh.PRIMITIVE_TRIANGLES)
	_create_branch(Vector3(0, size / 2.0, 0), Basis.IDENTITY, size, 1)

	var mesh_instance := MeshInstance3D.new()
	mesh_instance.mesh = _st.commit()
	mesh_instance.material_override = FormsMeshes.make_material(color, 0.4, 0.55)
	mesh_instance.cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	add_child(mesh_instance)
	_st = null


func _create_branch(position: Vector3, basis_: Basis, branch_size: float, level: int) -> void:
	var branch_xform := Transform3D(basis_.scaled(Vector3.ONE * branch_size), position)
	_st.append_from(_branch_mesh, 0, branch_xform)

	var up := basis_ * Vector3.UP
	var top := position + up * branch_size * 0.5
	_st.append_from(_node_mesh, 0, Transform3D(basis_.scaled(Vector3.ONE * branch_size), top))

	if level >= depth:
		return
	var child_size := branch_size * branch_ratio
	# Straight-up continuation.
	_create_branch(top + up * child_size * 0.7, basis_, child_size, level + 1)
	# Angled children fanned around the branch axis.
	var theta_inc := 360.0 / children
	for i in children:
		var a := randf_range(angle - 30.0, angle + 30.0) if stocastic else angle
		var q := basis_ * Basis.from_euler(Vector3(deg_to_rad(a), deg_to_rad(theta_inc * i), 0))
		var p := top + (q * Vector3.UP) * child_size * 0.7
		_create_branch(p, q, child_size, level + 1)
