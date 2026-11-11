# Skybound Adventure 3D

[![Unity](https://img.shields.io/badge/Unity-2022%2B%20%2F%206-black?logo=unity&logoColor=white)](https://unity.com/)
[![Blender](https://img.shields.io/badge/Blender-3D%20Assets-orange?logo=blender&logoColor=white)](https://www.blender.org/)
[![Mixamo](https://img.shields.io/badge/Mixamo-Rigging%20%26%20Animations-blue)](https://www.mixamo.com/)
[![Language](https://img.shields.io/badge/Language-C%23-blueviolet?logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

An immersive third-person 3D platformer featuring floating sky islands, custom-modeled low-poly environments, and fluid humanoid animations. Built from scratch across Blender, Adobe Mixamo, and Unity.

---

## 📸 Showcase

| Gameplay Preview | Level Overview |
| :---: | :---: |
| ![Gameplay](media/gameplay.png) | ![Level Design](media/level_design.png) |

| Character Modeling (Blender) | Animation Pipeline (Mixamo) |
| :---: | :---: |
| ![Blender Modeling](media/blender_model.png) | ![Mixamo Rig](media/mixamo_anim.png) |

---

## ✨ Features

- **Stylized Sky Islands:** Handcrafted floating biomes, suspension bridges, stone platforms, and organic foliage.
- **Custom 3D Art Pipeline:** Low-poly environment elements and props modeled and unwrapped directly in Blender.
- **Humanoid Animation System:** Character movement, idle, running, and jumping states rigged through Mixamo and orchestrated via Unity's Animator Controller.
- **Responsive Platforming Controls:** Tight third-person character movement and camera controls designed for aerial platform navigation.

---

## 🛠️ Tech Stack & Workflow

- **Engine:** Unity (Universal Render Pipeline - URP)
- **3D Modeling & Texturing:** Blender
- **Rigging & Motion Capture:** Adobe Mixamo
- **Scripting:** C# (.NET)
- **Version Control:** Git + Git LFS

---

## 🎮 Controls

| Action | Key / Input |
| :--- | :--- |
| **Move Forward / Backward** | `W` / `S` or `Up` / `Down` Arrow |
| **Move Left / Right** | `A` / `D` or `Left` / `Right` Arrow |
| **Jump** | `Spacebar` |
| **Look / Orbit Camera** | `Mouse Movement` |
| **Sprint** | `Left Shift` |

---

## 📂 Project Structure

```text
Skybound-Adventure-3D/
├── Assets/
│   ├── Art/                 # Blender source models, textures, materials
│   ├── Animations/          # Mixamo humanoid animation clips & controller
│   ├── Prefabs/             # Modular island pieces, bridges, and hazards
│   ├── Scenes/              # Game level scenes
│   └── Scripts/             # Player controllers, camera logic, game managers
├── Packages/                # Unity Package Manager manifest
├── ProjectSettings/         # Engine input, graphics, and tag configs
├── media/                   # README preview screenshots and GIFs
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- [Unity Hub](https://unity.com/download) with the appropriate Unity Editor version installed.
- Git and [Git LFS](https://git-lfs.github.com/) configured on your machine.

### Installation

1. Clone the repository:
   ```bash
   git clone [https://github.com/](https://github.com/)<YOUR_GITHUB_USERNAME>/Skybound-Adventure-3D.git
   ```
2. Open **Unity Hub**.
3. Click **Add** > **Add project from disk**.
4. Select the cloned `Skybound-Adventure-3D` root folder.
5. Launch the project, navigate to `Assets/Scenes/`, and open `SampleScene.unity`.
6. Press the **Play** button in the top toolbar to test.

---

## 👤 Author

**Bipin Maurya**  
- GitHub: [@YOUR_GITHUB_USERNAME](https://github.com/<YOUR_GITHUB_USERNAME>)  
- LinkedIn: [Bipin Maurya](https://www.linkedin.com/in/)
-