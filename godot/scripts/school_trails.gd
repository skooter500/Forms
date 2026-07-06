class_name SchoolTrails
extends MeshInstance3D
## Stand-in for the TrailRenderers on the original school fish.
## All ribbons of a school are batched into ONE triangle-strip surface
## (strips joined with degenerate triangles), rebuilt from packed arrays at
## 30 Hz — cheap enough for several schools of 60 fish.

var school: SchoolGenerator
var trail_time := 1.4
var point_interval := 0.06
var width := 5.0
var rebuild_interval := 1.0 / 30.0
var max_trail_distance := 5000.0

var _mesh := ArrayMesh.new()
var _histories := {}     # Boid -> {"pts": Array[Vector3], "times": Array[float], "color": Color}
var _point_accum := 0.0
var _rebuild_accum := 0.0
var _verts := PackedVector3Array()
var _cols := PackedColorArray()


func _ready() -> void:
	top_level = true
	global_transform = Transform3D.IDENTITY
	mesh = _mesh
	cast_shadow = GeometryInstance3D.SHADOW_CASTING_SETTING_OFF
	var m := StandardMaterial3D.new()
	m.shading_mode = BaseMaterial3D.SHADING_MODE_UNSHADED
	m.vertex_color_use_as_albedo = true
	m.blend_mode = BaseMaterial3D.BLEND_MODE_ADD
	m.cull_mode = BaseMaterial3D.CULL_DISABLED
	material_override = m


func _fish_color(fish: Boid) -> Color:
	for child in fish.get_children():
		if child is MeshInstance3D and child.material_override is StandardMaterial3D:
			return (child.material_override as StandardMaterial3D).emission
	return Color.WHITE


func _process(delta: float) -> void:
	if school == null:
		return
	var cam := get_viewport().get_camera_3d()
	if cam == null:
		return
	_point_accum += delta
	_rebuild_accum += delta
	if _rebuild_accum < rebuild_interval:
		return
	_rebuild_accum = 0.0
	var push := _point_accum >= point_interval
	if push:
		_point_accum = 0.0

	var now := Time.get_ticks_msec() / 1000.0
	var cam_pos := cam.global_position
	var max_dist_sq := max_trail_distance * max_trail_distance

	_verts.clear()
	_cols.clear()

	for fish: Boid in school.boids:
		if not is_instance_valid(fish):
			continue
		var fish_pos := fish.global_position
		if fish_pos.distance_squared_to(cam_pos) > max_dist_sq:
			_histories.erase(fish)
			continue
		var hist: Dictionary = _histories.get_or_add(fish,
			{"pts": [], "times": [], "color": _fish_color(fish)})
		var pts: Array = hist["pts"]
		var times: Array = hist["times"]
		if push:
			pts.append(fish_pos)
			times.append(now)
		while times.size() > 0 and now - times[0] > trail_time:
			pts.remove_at(0)
			times.remove_at(0)
		var n := pts.size()
		if n < 2:
			continue

		var col: Color = hist["color"]
		var first_written := _verts.size() == 0
		for i in n:
			var p: Vector3 = pts[i]
			var t: float = times[i]
			var fade: float = 1.0 - (now - t) / trail_time
			var dir: Vector3 = (pts[i + 1] - p) if i < n - 1 else (p - pts[i - 1])
			var side := dir.cross(cam_pos - p)
			var sl := side.length()
			side = side / sl if sl > 0.001 else Vector3.UP
			var half: Vector3 = side * (width * fade * 0.5)
			var c := Color(col.r * fade * 0.7, col.g * fade * 0.7, col.b * fade * 0.7)
			var a := p - half
			var b := p + half
			if i == 0 and not first_written:
				# Stitch strips with degenerate triangles.
				_verts.append(_verts[_verts.size() - 1])
				_cols.append(Color.BLACK)
				_verts.append(a)
				_cols.append(Color.BLACK)
			_verts.append(a)
			_cols.append(c)
			_verts.append(b)
			_cols.append(c)

	_mesh.clear_surfaces()
	if _verts.size() >= 3:
		var arrays := []
		arrays.resize(Mesh.ARRAY_MAX)
		arrays[Mesh.ARRAY_VERTEX] = _verts
		arrays[Mesh.ARRAY_COLOR] = _cols
		_mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLE_STRIP, arrays)
