# 🏎️ 3D Car Driving Game — Unity
<img width="1592" height="895" alt="image" src="https://github.com/user-attachments/assets/f624a055-1f8a-4ae5-adee-b463ed62d420" />
<img width="1458" height="778" alt="image" src="https://github.com/user-attachments/assets/4e060e3e-edd5-4bf6-ba06-5f65e1064271" />

A stylized **3D car driving and exploration game** developed with Unity. Explore a low-poly city, drive different types of vehicles, and navigate a modular urban environment filled with buildings, roads, traffic lights, and other city props.

## ✨ Features

### 🚗 Multiple Vehicles
Choose from a variety of vehicles, including:
- Police Car 🚓
- Taxi 🚕
- Sedan 🚗
- Hatchback
- Station Wagon

### 🏙️ Modular City Environment
Explore a stylized urban environment featuring:
- Multiple building designs (Buildings A–H)
- Modular roads, including straight roads, corners, T-junctions, and intersections
- Traffic lights and streetlights
- Benches, fire hydrants, dumpsters, and water towers
- Desert and green ground themes

### 🎮 Modern Controls
- Uses Unity's **New Input System**
- Supports configurable input actions
- Provides driving controls for acceleration, braking, steering, and handbraking

### 🎨 Low-Poly Graphics
- Stylized 3D models
- Optimized materials and textures
- Modular environment assets for flexible city construction

## 🛠️ Technologies Used

- **Game Engine:** Unity
- **Programming Language:** C#
- **Input Handling:** Unity New Input System
- **3D Assets:** FBX and GLTF models

## 📋 Requirements

Before running the project, make sure you have:

- [Unity Hub](https://unity.com/download)
- Unity **2021.3 LTS or newer**, subject to project compatibility
- Unity New Input System package
- Git (optional, for cloning the repository)

## 🚀 Getting Started

### 1. Clone the Repository

Open a terminal and run:

```bash
git clone https://github.com/your-username/3D-Car-Game-main.git
```

Replace `your-username` with the actual GitHub username or organization that owns the repository.

### 2. Open the Project

1. Launch Unity Hub.
2. Select **Add** or **Open Project**.
3. Navigate to the cloned project folder.
4. Open the project using a compatible Unity version.

### 3. Configure Dependencies

1. Allow Unity to import the project assets.
2. Open **Window → Package Manager**.
3. Check that the **Input System** package is installed.
4. If prompted to enable the new input backend, follow Unity's instructions and restart the Editor.

### 4. Run the Game

1. Open the main game scene inside the `Assets` folder.
2. Check that the scene contains the required vehicle, camera, and input components.
3. Press the **Play** button in the Unity Editor.
4. Use the controls below to drive and explore.

*Note: The exact main scene name and any additional setup requirements depend on the project files.*

## 🎮 Controls

The following are intended controls. Actual bindings depend on the project's input actions and vehicle scripts.

| Action | Key |
|---|---|
| Accelerate | `W` / `↑` |
| Brake / Reverse | `S` / `↓` |
| Steer Left | `A` / `←` |
| Steer Right | `D` / `→` |
| Handbrake | `Space` |
| Change Camera | `C` (if configured) |

You can inspect and modify the bindings in `InputSystem_Actions.inputactions`.

## 📁 Project Structure

```text
3D-Car-Game-main/
├── Assets/
│   ├── Model/
│   │   └── City Building/
│   │       └── Assets/
│   │           └── fbx/
│   │               ├── car_police.fbx
│   │               ├── car_taxi.fbx
│   │               └── ... other models
│   ├── Materials/
│   └── InputSystem_Actions.inputactions
├── Packages/
├── ProjectSettings/
├── README.md
└── LICENSE
```

*This is an illustrative structure. Folder and file names may differ in your actual repository.*

## 🗺️ Gameplay

Explore the city, drive through streets and intersections, and experience the low-poly environment using different vehicle models.

> **Screenshots and gameplay videos:** Add screenshots or a short gameplay video here to help visitors understand the game before downloading it.

For example:

```markdown
![City Environment](screenshots/city-environment.png)
![Available Vehicles](screenshots/vehicles.png)
```

Create a `screenshots/` folder in the repository and replace these example paths with your actual screenshots.

## 🤝 Contributing

Contributions, bug reports, and feature requests are welcome!

1. Fork the repository.
2. Create a feature branch.
3. Make your changes.
4. Test your changes in Unity.
5. Submit a pull request describing your improvements.

For bugs or suggestions, open an issue in the repository.

## 📄 License

This project is intended to be distributed under the **MIT License**. See the `LICENSE` file for the full terms.

If the repository does not already contain an MIT license, verify that the project assets permit this licensing before adding one.

---

⭐ If you find this project useful, consider giving the repository a star!
