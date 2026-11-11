# ☁️ Skybound Adventure 3D

[![Unity](https://img.shields.io/badge/Unity-2022.3%20%2F%206%20(URP)-black?style=for-the-badge&logo=unity&logoColor=white)](https://unity.com/)
[![Blender](https://img.shields.io/badge/Blender-3.x%20%2F%204.x-E87D0D?style=for-the-badge&logo=blender&logoColor=white)](https://www.blender.org/)
[![Mixamo](https://img.shields.io/badge/Adobe%20Mixamo-Humanoid%20Rig-005fee?style=for-the-badge)](https://www.mixamo.com/)
[![C#](https://img.shields.io/badge/C%23-Language-239120?style=for-the-badge&logo=csharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Platform](https://img.shields.io/badge/Platform-Windows%20PC-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://www.microsoft.com/windows)

<img width="1486" height="812" alt="Screenshot 2026-11-11 212009" src="https://github.com/user-attachments/assets/657c2a75-7dde-4554-a641-85af78b441cd" />,<img width="1468" height="821" alt="Screenshot 2026-11-11 212211" src="https://github.com/user-attachments/assets/c8278b1d-178f-4e65-96e8-6571b9c399b1" />,<img width="1470" height="822" alt="Screenshot 2026-11-11 212059" src="https://github.com/user-attachments/assets/ea64866d-9989-408b-904e-7dc93b53eec0" />,<img width="1478" height="817" alt="Screenshot 2026-11-11 212132" src="https://github.com/user-attachments/assets/769d5be6-2105-46b2-bd53-8111ad721d3e" />


| :---: | :---: |
| ![Blender Models](media/island_detail.png) | ![Mixamo Rig](media/obstacle_course.png) |
| *Handcrafted low-poly environment props, stylized trees, and island rocks* | *Humanoid avatar configuration, blend trees, and animation transitions* |

---

## 🎮 Core Gameplay Mechanics

- **Physics-Driven Third-Person Locomotion:** Responsive character controller featuring acceleration curves, directional sprint mechanics, and momentum-preserving jumps.
- **Aerial Platform Navigation:** Precision jump mechanics calibrated for floating island distances, elevation shifts, and narrow wooden suspension bridges.
- **Orbiting Third-Person Camera:** Smooth mouse-controlled camera orbit with pitch clamping, collision buffering, and dynamic target tracking.
- **Environment Interactivity:** Modular ground detection via raycasts, obstacle collision geometry, and collectible placements.
- ---

## 🗺️ Level Design & Island Progression

The environment is designed around verticality, platform pacing, and clear visual silhouettes:

| Stage / Biome | Environmental Features | Gameplay Focus |
| :--- | :--- | :--- |
| **01. Starter Island** | Flat grassy terrain, tutorial prompts, low risk | Basic locomotion, camera orbit, jump calibration |
| **02. The Rickety Bridges** | Narrow wooden suspension planks, swaying gaps | Precision walking, jump timing, fall hazards |
| **03. Palm Oasis & Pickups** | Stylized palm trees, scattered coin collectibles | Exploration, collectible loops, sprint jumping |
| **04. Cloud Spire Summit** | High-elevation rock pillars, ascending steps | Vertical platforming, depth perception, goal completion |

---

## ⚡ Performance & Engine Optimization

- **Universal Render Pipeline (URP):** Single-pass forward rendering minimizing draw calls on desktop GPUs.
- **SRP Batcher Compatibility:** Custom materials share unified shader variants, allowing Unity to batch environment draw calls into continuous GPU buffers.
- **Low-Poly Vertex Budgets:** Island meshes and foliage retain optimized polycounts under 2,500 triangles per asset for instantaneous scene loading.
- **Mesh Colliders vs. Primitive Colliders:** Floating platforms utilize optimized Box and Capsule colliders for physics calculations rather than heavy mesh colliders.
- **Target Performance:** Rock-solid **60+ FPS** at 1080p / 1440p on integrated and dedicated graphics.

---

## 🔊 Audio & Atmospheric Design

- **Adaptive Wind Ambience:** Subtle high-altitude ambient soundscape that reinforces the sensation of floating in the open sky.
- **Dynamic Surface Footsteps:** Audio clips calibrated to surface material tags (wood planks vs. grass/stone).
- **Interactive Feedback:** Distinct audio chimes for coin pickups and jump takeoff/landing impacts.

---

## 🎯 Future Milestones & Expansions

-  Third-person character controller with sprint and jump physics
-  Handcrafted Blender 3D environment assets and stylized canopies
-  Mixamo humanoid avatar configuration and animation blend trees
-  Modular floating island network with suspension bridges
-  In-game HUD with coin counter and completion timer
-  Fall-recovery respawn trigger with particle splash effect
-  Moving cloud platforms and wind-gust physics zones
-  Export standalone Windows `.exe` and WebGL browser build on itch.io

---

## 📄 License & Terms

This project is open-source and available under the [MIT License](LICENSE). 3D character rigs and animations are powered by Adobe Mixamo under standard service terms.

---

## 📬 Connect & Collaborate

Have feedback, ideas, or want to collaborate on game development projects?

[![LinkedIn](https://img.shields.io/badge/LinkedIn-Connect-0A66C2?style=for-the-badge&logo=linkedin&logoColor=white)](https://www.linkedin.com/in/)
[![GitHub](https://img.shields.io/badge/GitHub-Profile-181717?style=for-the-badge&logo=github&logoColor=white)](https://github.com/bipinmaurya5567-bit)
[![Email](https://img.shields.io/badge/Email-Contact-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:125Bipin6042@sjcem.com)

---

## 🛠️ End-to-End Technical Pipeline
