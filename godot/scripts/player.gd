class_name Player
extends Node3D
## Port of the original player stack (no XR):
##  - ForceController: inertial force-based flight. W/S thrust along the
##    camera forward, A/D strafe, E/F fly up/down, Shift = 3x speed and
##    2x turn rate. Mouse X yaws, mouse Y pitches; the body slerps towards
##    the desired rotation, so flight feels floaty like the Unity original.
##    Gamepad (matches the Unity bindings): left stick move, right stick
##    look, RB/LB fly up/down (Joystick buttons 5/4), A = 3x boost (Fire1),
##    Y = mode switch (JoystickButton3), dpad-up = ground material (DPadY).
##  - PlayerController states: J (or gamepad Y) counts clicks in a 0.5s
##    window like the original — 1 click = Journeying (Cruise autopilot),
##    2 = Following (watch a creature), 3 = free Player control.
##  - EscapeToQuit: Esc quits. (Tab releases the mouse instead.)
##  - WorldGenerator DPad-Y material switch: G / dpad-up cycles the ground.

enum ControlType { PLAYER, JOURNEYING, FOLLOWING }

@export var speed := 2200.0
@export var angular_speed := 12.0
@export var damping := 0.9
## Cruise.cs values, scaled to this world.
@export var cruise_height := 500.0
@export var cruise_speed := 900.0

var control_type := ControlType.PLAYER
var camera: Camera3D

var _velocity := Vector3.ZERO
var _desired_rotation: Basis
var _yaw := 0.0
var _pitch := 0.0

var _click_count := 0
var _click_elapsed := 0.0

var _followed: Node3D
var _follow_offset := Vector3.ZERO


func _ready() -> void:
	camera = Camera3D.new()
	camera.far = 20000.0
	camera.near = 1.0
	add_child(camera)
	camera.make_current()
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	_desired_rotation = basis


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.mouse_mode == Input.MOUSE_MODE_CAPTURED:
		var turn := 0.0022 * angular_speed / 12.0
		if _boosting():
			turn *= 2.0
		_yaw -= event.relative.x * turn
		_pitch = clampf(_pitch - event.relative.y * turn, -PI / 2 + 0.01, PI / 2 - 0.01)
		_desired_rotation = Basis.from_euler(Vector3(_pitch, _yaw, 0))
	elif event is InputEventMouseButton and event.pressed \
			and Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED
	elif event.is_action_pressed("quit"):
		get_tree().quit()          # EscapeToQuit.cs
	elif event.is_action_pressed("release_mouse"):
		Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	elif event.is_action_pressed("mode_tap"):
		_click_count = (_click_count + 1) % 6
		_click_elapsed = 0.0
	elif event.is_action_pressed("cycle_ground"):
		WorldGenerator.instance.cycle_ground_material()


func _process(delta: float) -> void:
	_handle_click_state(delta)
	_gamepad_look(delta)

	match control_type:
		ControlType.PLAYER:
			_force_controller(delta)
		ControlType.JOURNEYING:
			_cruise(delta)
		ControlType.FOLLOWING:
			_follow(delta)

	# Slerp towards the desired rotation like ForceController did.
	basis = basis.slerp(_desired_rotation, minf(delta * 4.0, 1.0))

	if WorldGenerator.instance != null:
		var ground: float = WorldGenerator.instance.sample_pos(global_position.x, global_position.z)
		if global_position.y < ground + 50.0:
			global_position.y = ground + 50.0
			_velocity.y = maxf(_velocity.y, 0.0)


## PlayerController.Update: count J clicks, act after a 0.5s pause.
## 1 = Journeying, 2 = Follow, 3 = Player, 4 = Show (auto-cycle).
func _handle_click_state(delta: float) -> void:
	_click_elapsed += delta
	if _click_count > 0 and _click_elapsed > 0.5:
		match _click_count:
			1:
				_show_active = false
				control_type = ControlType.JOURNEYING
			2:
				_show_active = false
				_start_following()
			3:
				_show_active = false
				control_type = ControlType.PLAYER
			4:
				_start_show()
			_:
				_show_active = false
				control_type = ControlType.PLAYER
		_click_count = 0
	if _show_active:
		_run_show(delta)


# --- Show mode (PlayerController.Show, minus the installation logos) ---
# Alternates Journeying with Following creatures on the original timers
# (journeying ~5s legs, delayMin/delayMax 30-40s, creature phases 1.5x).

var _show_active := false
var _show_timer := 0.0
var _show_following := false


func _start_show() -> void:
	_show_active = true
	_show_following = false
	control_type = ControlType.JOURNEYING
	_show_timer = 5.0 + randf_range(30.0, 40.0)


func _run_show(delta: float) -> void:
	_show_timer -= delta
	if _show_timer > 0.0:
		return
	if _show_following:
		_show_following = false
		control_type = ControlType.JOURNEYING
		_show_timer = 5.0 + randf_range(30.0, 40.0)
	else:
		_show_following = true
		_start_following()
		_show_timer = randf_range(45.0, 60.0)


func _boosting() -> bool:
	# Shift on keyboard, A on the pad (Unity "Fire1").
	return Input.is_action_pressed("boost")


func _gamepad_look(delta: float) -> void:
	var look_x := Input.get_axis("look_left", "look_right")
	var look_y := Input.get_axis("look_up", "look_down")
	if absf(look_x) > 0.01 or absf(look_y) > 0.01:
		var rate := 2.0 * (2.0 if _boosting() else 1.0)
		_yaw -= look_x * delta * rate
		_pitch = clampf(_pitch - look_y * delta * rate, -PI / 2 + 0.01, PI / 2 - 0.01)
		_desired_rotation = Basis.from_euler(Vector3(_pitch, _yaw, 0))


func _force_controller(delta: float) -> void:
	var cont_speed := speed
	if _boosting():
		cont_speed *= 3.0

	# All bindings live in the Input Map (project.godot): WASD/left stick,
	# E/F/Q + RB/LB fly (ForceController Joystick buttons 5/4).
	var walk := Input.get_axis("back", "forward")
	var strafe := Input.get_axis("strafe_left", "strafe_right")
	var fly := Input.get_axis("fly_down", "fly_up")

	# Forces along the camera frame (ForceController.Walk/Strafe/Fly).
	var forward := -camera.global_basis.z
	var right := camera.global_basis.x
	_velocity += (forward * walk + right * strafe + Vector3.UP * fly) * cont_speed * delta
	_velocity *= 1.0 - damping * delta
	global_position += _velocity * delta


## Cruise.cs: autopilot — hold height above the terrain, push forward.
func _cruise(delta: float) -> void:
	var wg := WorldGenerator.instance
	var forward: Vector3 = -basis.z
	forward.y = 0
	forward = forward.normalized()

	var height := global_position.y - wg.sample_pos(global_position.x, global_position.z)
	if height < cruise_height - 50.0:
		_velocity.y += cruise_speed * delta
	elif height > cruise_height + 50.0:
		_velocity.y -= cruise_speed * delta

	# Terrain rising ahead? Climb (the forward raycast in Cruise.cs).
	var ahead: Vector3 = global_position + forward * 1000.0
	if wg.sample_pos(ahead.x, ahead.z) > global_position.y - 100.0:
		_velocity.y += cruise_speed * 3.0 * delta

	_velocity += forward * cruise_speed * delta
	_velocity *= 1.0 - damping * delta
	global_position += _velocity * delta


func _start_following() -> void:
	if Mother.instance == null:
		return
	_followed = Mother.instance.get_random_creature()
	if _followed == null:
		control_type = ControlType.JOURNEYING
		return
	control_type = ControlType.FOLLOWING
	# Sit at a viewing distance behind/beside the creature (SpawnParameters
	# viewingDistance, FollowCoRoutine).
	var viewing_distance := randf_range(400.0, 800.0)
	_follow_offset = Vector3(randf_range(-0.5, 0.5), randf_range(0.1, 0.4), 1.0).normalized() * viewing_distance


func _follow(delta: float) -> void:
	if _followed == null or not is_instance_valid(_followed):
		_start_following()
		return
	var target: Vector3 = _followed.global_position \
		+ _followed.global_basis * _follow_offset
	var ground: float = WorldGenerator.instance.sample_pos(target.x, target.z)
	target.y = maxf(target.y, ground + 50.0)
	global_position = global_position.lerp(target, minf(delta * 2.0, 1.0))

	var to_creature: Vector3 = _followed.global_position - global_position
	if to_creature.length() > 1.0:
		_desired_rotation = Basis.looking_at(to_creature.normalized(), Vector3.UP)
		_yaw = _desired_rotation.get_euler().y
		_pitch = _desired_rotation.get_euler().x
	_velocity = Vector3.ZERO
