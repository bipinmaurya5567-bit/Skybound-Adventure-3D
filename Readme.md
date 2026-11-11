# ☁️ Skybound Adventure 3D

[![Unity](https://img.shields.io/badge/Unity-URP%20%2F%202022%2B-black?logo=unity&logoColor=white)](https://unity.com/)
[![Blender](https://img.shields.io/badge/Blender-3D%20Assets-orange?logo=blender&logoColor=white)](https://www.blender.org/)
[![Mixamo](https://img.shields.io/badge/Mixamo-Humanoid%20Rigging-blue)](https://www.mixamo.com/)
[![Language](https://img.shields.io/badge/C%23-.NET-blueviolet?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

An immersive third-person 3D aerial platformer featuring handcrafted floating sky islands, custom low-poly environment assets, and fluid humanoid locomotion. Built from the ground up using **Blender**, **Adobe Mixamo**, and **Unity (URP)**.

---

## 📸 Showcase

| Third-Person Gameplay | Aerial Level Overview |
| :---: | :---: |
| ![Gameplay Preview](media/gameplay.png) | ![Level Overview](media/level_overview.png) |
| *Core third-person player controller on floating biomes* | *Suspension bridge network connecting aerial islands* |

| Island Environment & Pickups | Aerial Obstacle Course |
| :---: | :---: |
| ![Island Detail](media/island_detail.png) | ![Obstacle Track](media/obstacle_course.png) |
| *Low-poly foliage, collectible coins, and props* | *Challenging platforming alignment and cloud backdrop* |

---

## ✨ Features

- **Stylized Floating Biomes:** Modular sky islands featuring custom low-poly rocks, stylized canopies, suspension bridges, and dynamic cloudscapes.
- **Custom 3D Art Pipeline:** Environment models, foliage, and platforms handcrafted and unwrapped directly in Blender.
- **Humanoid Animation System:** Character rigs animated with Adobe Mixamo and sequenced through Unity's Animator state machines.
- **Responsive Platforming:** Physics-based third-person controller calibrated for precise jumps, sprint momentum, and camera tracking.
- **Optimized Rendering:** Built on Unity's Universal Render Pipeline (URP) with custom stylized materials and lighting.

---

## 🛠️ Tech Stack & Workflow

- **Engine:** Unity (Universal Render Pipeline - URP)
- **3D Modeling & Texturing:** Blender
- **Rigging & Motion Capture:** Adobe Mixamo
- **Scripting:** C# (.NET)
- **Version Control:** Git + Git LFS

---

## 🎮 Controls

| Action | Keyboard / Mouse Input |
| :--- | :--- |
| **Move (Forward / Backward / Strafe)** | `W` / `A` / `S` / `D` or `Arrow Keys` |
| **Jump** | `Spacebar` |
| **Sprint** | `Left Shift` |
| **Orbit Camera** | `Mouse Movement` |

---

## 📂 Project Structure

```text
Skybound-Adventure-3D/
├── Assets/
│   ├── Art/                 # 3D models (.fbx/.blend), materials, textures
│   ├── Animations/          # Mixamo humanoid animations and Animator Controllers
│   ├── Prefabs/             # Modular island pieces, bridges, and collectibles
│   ├── Scenes/              # Level scenes (SampleScene.unity)
│   └── Scripts/             # Player controller, camera, and gameplay scripts
├── Packages/                # Unity Package Manager manifest
├── ProjectSettings/         # Engine settings, input mapping, and URP configs
├── media/                   # Showcase preview screenshots for README
├── .gitignore               # Unity Git ignore configuration
└── README.md                # Project documentation
