class_name SpineAnimator
extends Node
## Port of BGE.Forms.SpineAnimator: each bone chases the previous one at a
## fixed local-space offset, giving the trailing spine motion.

@export var bond_damping := 10.0
@export var angular_bond_damping := 12.0

var head: Node3D
var bones: Array[Node3D] = []

var _offsets: Array[Vector3] = []


func assign_bones(p_head: Node3D, p_bones: Array[Node3D]) -> void:
	head = p_head
	bones = p_bones
	_offsets.clear()
	for i in bones.size():
		var prev := head if i == 0 else bones[i - 1]
		var offset := bones[i].global_position - prev.global_position
		_offsets.append(prev.global_basis.get_rotation_quaternion().inverse() * offset)


func _physics_process(delta: float) -> void:
	for i in bones.size():
		var prev := head if i == 0 else bones[i - 1]
		var current := bones[i]

		var wanted: Vector3 = prev.global_position + prev.global_basis.get_rotation_quaternion() * _offsets[i]
		var new_pos := current.global_position.lerp(wanted, clampf(delta * bond_damping, 0.0, 1.0))
		current.global_position = new_pos

		var dir: Vector3 = prev.global_position - new_pos
		if dir.length() > 0.001:
			var up := prev.global_basis.y
			if absf(dir.normalized().dot(up)) < 0.99:
				var wanted_rot := Basis.looking_at(dir.normalized(), up).get_rotation_quaternion()
				var q := current.global_basis.get_rotation_quaternion()
				current.global_basis = Basis(q.slerp(wanted_rot, clampf(delta * angular_bond_damping, 0.0, 1.0))).scaled(current.scale)
