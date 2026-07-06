class_name GameOfLifeTexture
extends Node
## Port of BGE.Forms.GameOfLifeTextureGenerator: an animated Game of Life
## grid used as the emissive texture on the ground and ceiling.
## Hot loops are allocation-free: neighbours are summed inline and pixels
## are written straight into a byte buffer through a hue LUT.

@export var size := 128
@export var step_interval := 0.25
@export var reseed_interval := 30.0

var texture: ImageTexture

## Cell values are hue indices: 0 = dead, 1..249 = position on the hue
## wheel. Survivors keep their colour; newborns average their parents'
## (the original NewCellColor rule).
var _cells: PackedByteArray
var _next: PackedByteArray
var _image: Image
var _pixels: PackedByteArray      # RGB8 buffer written each render
var _lut_r: PackedByteArray       # hue index -> RGB
var _lut_g: PackedByteArray
var _lut_b: PackedByteArray
var _step_timer := 0.0
var _reseed_timer := 0.0


func _ready() -> void:
	_cells.resize(size * size)
	_next.resize(size * size)
	_pixels.resize(size * size * 3)
	_lut_r.resize(250)
	_lut_g.resize(250)
	_lut_b.resize(250)
	for i in range(1, 250):
		var c := Color.from_hsv((i - 1) / 248.0, 1.0, 1.0)
		_lut_r[i] = int(c.r * 255.0)
		_lut_g[i] = int(c.g * 255.0)
		_lut_b[i] = int(c.b * 255.0)
	_seed_random()
	_image = Image.create(size, size, false, Image.FORMAT_RGB8)
	_render()
	texture = ImageTexture.create_from_image(_image)


func _process(delta: float) -> void:
	_step_timer += delta
	_reseed_timer += delta
	if _reseed_timer > reseed_interval:
		_reseed_timer = 0.0
		_seed_random()
	if _step_timer > step_interval:
		_step_timer = 0.0
		_step()
		_render()
		texture.update(_image)


func _seed_random() -> void:
	for i in _cells.size():
		if randf() < 0.3:
			_cells[i] = randi_range(1, 249)


func _step() -> void:
	for y in size:
		var ym := ((y - 1 + size) % size) * size
		var yp := ((y + 1) % size) * size
		var yo := y * size
		for x in size:
			var xm := (x - 1 + size) % size
			var xp := (x + 1) % size
			var n := 0
			var age_sum := 0
			var a := _cells[ym + xm]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[ym + x]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[ym + xp]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[yo + xm]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[yo + xp]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[yp + xm]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[yp + x]
			if a > 0:
				n += 1
				age_sum += a
			a = _cells[yp + xp]
			if a > 0:
				n += 1
				age_sum += a
			var age := _cells[yo + x]
			if age > 0:
				_next[yo + x] = age if (n == 2 or n == 3) else 0
			else:
				_next[yo + x] = clampi(age_sum / 3, 1, 249) if n == 3 else 0
	var tmp := _cells
	_cells = _next
	_next = tmp


func _render() -> void:
	var count := size * size
	for i in count:
		var age := _cells[i]
		var o := i * 3
		_pixels[o] = _lut_r[age]
		_pixels[o + 1] = _lut_g[age]
		_pixels[o + 2] = _lut_b[age]
	_image.set_data(size, size, false, Image.FORMAT_RGB8, _pixels)
