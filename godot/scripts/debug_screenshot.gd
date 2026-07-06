extends Node
## Dev helper: pass `--screenshot` (with `++` separator) to save frames to
## the path given by SCREENSHOT_DIR and quit. Not part of the game.

var _frames := 0
var _dir := "/tmp"


func _ready() -> void:
	if not OS.get_cmdline_user_args().has("--screenshot"):
		queue_free()
		return
	_dir = OS.get_environment("SCREENSHOT_DIR")
	if _dir.is_empty():
		_dir = "/tmp"


func _process(_delta: float) -> void:
	_frames += 1
	if _frames in [60, 300, 600]:
		var img := get_viewport().get_texture().get_image()
		img.save_png("%s/forms_%04d.png" % [_dir, _frames])
	if _frames >= 600:
		get_tree().quit()
