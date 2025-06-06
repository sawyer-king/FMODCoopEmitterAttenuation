# FMOD Coop Emitter Attenuation
[![Unity 2021.3+](https://img.shields.io/badge/unity-2021.3%2B-blue.svg)](https://unity3d.com/get-unity/download)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](LICENSE.md)

A script used to trigger an 3D FMOD Event and update a distance parameter for that event that is calculated from the closest of multiple player GameObjects in Unity. Primarily to be used in Coop games with a single listener.
```
This script requires FMOD and Unity.
```

## How to Use
- Add this script and a collider component (usually a Sphere Collider) to the GameObject that you want sound to emit from, make sure IsTrigger is set to True, then set a radius that makes sense for the scale of your game.
- Add the Player GameObjects to the list in the script that you want to calculate distance from.
- Set Radius length for the scale of the distance parameter.
- Set the name of the FMOD distance parameter as a string.
- Set the reference for the 3D FMOD Event.
