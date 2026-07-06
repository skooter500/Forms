class_name PerlinSampler
extends RefCounted
## Port of BGE.Forms.PerlinNoiseSampler / Sampler.
## Perlin noise with a "plateau" band: noise values between low and high are
## clamped to the midpoint, which is what carves the flat valleys and mesas.

enum Op { ADD, SUBTRACT, MULTIPLY, DIVIDE, IGNORE }

var op: int = Op.ADD
var low := 1.0
var high := 0.0
var origin := 0.0
var scale := 0.2
var height := 100.0

var _noise := FastNoiseLite.new()


func _init(p_low: float, p_high: float, p_origin: float, p_scale: float, p_height: float) -> void:
	low = p_low
	high = p_high
	origin = p_origin
	scale = p_scale
	height = p_height
	_noise.noise_type = FastNoiseLite.TYPE_PERLIN
	_noise.frequency = 1.0
	_noise.seed = 1


func sample(x: float, y: float) -> float:
	# Unity's Mathf.PerlinNoise returns ~[0,1]; FastNoiseLite returns [-1,1].
	var n := _noise.get_noise_2d(origin + x * scale, origin + y * scale) * 0.5 + 0.5
	var mid := 0.5
	if n > high:
		n = mid + (n - high)
	elif n < low:
		n = mid + (n - low)
	else:
		n = mid
	return n * height


func operate(input: float, x: float, y: float) -> float:
	match op:
		Op.ADD:
			return input + sample(x, y)
		Op.SUBTRACT:
			return input - sample(x, y)
		Op.MULTIPLY:
			return input * sample(x, y)
		Op.DIVIDE:
			return input / sample(x, y)
		_:
			return input
