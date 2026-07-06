class_name FormationGenerator
extends Node3D
## Port of BGE.Forms.FormationGenerator: a leader creature with followers of
## the same species holding a V formation via OffsetPursue (bigSnakeFormation,
## rayFamily, mermaids in the original scene).

@export var side_width := 2
@export var gap := 400.0
@export var variance := 0.1

## Callable returning a fresh CreatureGenerator for the species (leader and
## followers share it; followers are scaled down slightly).
var make_creature: Callable


func _ready() -> void:
	if make_creature.is_null():
		return
	var leader: CreatureGenerator = make_creature.call()
	leader.name = "Leader"
	add_child(leader)
	var leader_boid := leader.boid

	for i in range(1, side_width + 1):
		for side in [-1.0, 1.0]:
			var offset := Vector3(gap * i * side, 0, gap * i)
			offset.x *= randf_range(1.0 - variance, 1.0)
			offset.y += gap * randf_range(-variance, variance)
			offset.z *= randf_range(1.0 - variance, 1.0 + variance)

			var follower: CreatureGenerator = make_creature.call()
			follower.name = "Follower%d%s" % [i, "L" if side < 0 else "R"]
			follower.vertical_size *= randf_range(0.6, 0.85)
			follower.position = offset
			add_child(follower)

			# Followers pursue the leader instead of wandering off.
			var fb := follower.boid
			for b in fb.get_children():
				if b is NoiseWander or b is Constrain:
					b.active = false
			var pursue := OffsetPursue.new()
			pursue.setup(leader_boid)
			fb.add_child(pursue)
