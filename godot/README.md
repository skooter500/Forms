# Infinite Forms — Godot 4 port

A GDScript port of the Unity "Infinite Forms" world (the
`paradiso1stbirthday` scene, minus XR). Everything is procedural — there are
no imported assets. Open this folder in Godot 4.6+ and run.

```
godot --path godot        # or open in the editor and press F5
```

## Controls (ported from ForceController / PlayerController / EscapeToQuit)

| Input | Action |
|---|---|
| Mouse | Yaw / pitch (rotation is slerped, floaty like the original) |
| W / S | Thrust along the camera forward |
| A / D | Strafe |
| E / F (or Q) | Fly up / down |
| Shift | 3× speed, 2× turn rate |
| J (or gamepad Y) | Click-count mode switch, 0.5 s window like the original: 1 click = **Journeying** (Cruise autopilot), 2 = **Following** (camera watches a creature), 3 = **Player** (free flight), 4 = **Show** (auto-cycles journeying and creature-following on the original timers) |
| G (or dpad-up) | Cycle the ground material (DPadY in the original) |
| Esc | Quit (EscapeToQuit) |
| Tab | Release the mouse |

Gamepad (the original Unity bindings):

| Input | Action |
|---|---|
| Left stick | Thrust / strafe ("Vertical"/"Horizontal") |
| Right stick | Yaw / pitch ("Joy X"/"Joy Y") |
| RB / LB | Fly up / down (Joystick buttons 5/4) |
| A | 3× speed boost ("Fire1") |
| Y | Mode switch click-counter (JoystickButton3) |
| Dpad up | Cycle ground material (DPadY) |

Debug flags (after `++` on the command line): `--stress` (fast spawning),
`--zoo` (one of every species), `--jellyzoo`, `--screenshot`.

## What's ported

- **WorldGenerator** — infinite tiled terrain from five plateau-clamped
  Perlin samplers (values lifted from the Unity scene), plus the glowing
  ceiling "surface" at 8000 units.
- **GameOfLifeTextureGenerator** — animated Game of Life emissive texture on
  ground and ceiling; survivors keep their colour, newborns inherit the
  average parent hue (the rainbow trails).
- **Boid + steering** — Seek, Flee-style forces, Harmonic, Hover,
  NoiseWander, JitterWander, Constrain, Cohesion/Separation/Alignment,
  OffsetPursue, plus analytic TerrainAvoidance/GroundHug replacing the
  raycast SceneAvoidance.
- **SpineAnimator / FinAnimator** — trailing segmented bodies and
  harmonic-phase fin flapping.
- **All the species** from the original scene, spawned by **Mother** in a
  shuffled bag: schools (plain, chaotic, rainbow "lifeColours", sardine),
  spine creatures (elasmosaurus, snake, ray, sperm, mermaid, random forms),
  doves (straight travellers), tenticle creatures, transparent jellies
  (Hover pulse propulsion), sandworms, tardigrade schools, and V-formations
  (FormationGenerator + OffsetPursue followers).
- **Plants & artefacts** — two **GenesisDevices** scatter TreeGen fractal
  trees, thc plants and tenticle flowers on the ground, and rotating disco
  balls in the air, on a noise-thresholded grid with pooling.

The Unity project is untouched; this port lives entirely in `godot/`.
