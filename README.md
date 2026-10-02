# LanAndroidTPSTemplate

A Unity-based third-person shooter (TPS) template for Android with LAN multiplayer support using Mirror.

LanAndroidTPSTemplate was created as a technical project for experimenting with Android multiplayer gameplay over a local network. It provides a basic foundation for character movement, aiming, LAN connectivity, and Host/Client communication.

## Features

- Android TPS gameplay foundation
- LAN multiplayer
- Host / Client connection system
- Mirror networking
- Basic character movement
- Third-person aiming
- IK Bone-based aiming
- Local network testing without requiring an internet connection
- Simple structure that can be extended with additional gameplay systems

## How the LAN Connection Works

The project uses Mirror for the multiplayer connection.

When a Host is created, the server starts listening on **port 8888**. Another device on the same local network can connect to the Host through the Hub.

### Connection Steps

1. Open the project on the first machine.
2. Create a Host.
3. The Host starts listening on port `8888`.
4. On the second machine, activate the Hub.
5. Start listening for the Host.
6. The client connects to the Host.
7. Continue to the sample gameplay scene.

> **Note:** If you want to create and test a client, activate the Hub in the Unity Editor. After creating a Host on the first machine, continue on the other machine and connect from the Hub.

## Main Menu / Hub

The Main Menu provides the basic interface for creating and connecting to a LAN multiplayer session.

![Main Menu](https://github.com/user-attachments/assets/26a62dad-93a6-4d22-b8e4-46a4a0948042)

When the Host is created, it starts listening on port `8888`. A client can then listen for the Host through the Hub and connect to the session.

## Sample Scene

The sample scene demonstrates the basic TPS gameplay systems included in the project.

It currently includes:

- Character movement
- Third-person gameplay
- Aiming
- Multiplayer character setup

![Sample Scene](https://github.com/user-attachments/assets/b9e8d940-843c-43cf-8b3b-b0a67edc5a45)

![Sample Scene - Gameplay](https://github.com/user-attachments/assets/d95cd366-9334-48b9-9a07-c3b9fbf31809)

## Aiming System

The project includes a third-person aiming system.

**IK Bones** are used to adjust the character's bones during aiming, allowing the character to visually align with the aiming direction.

![IK Bones Aiming System](https://github.com/user-attachments/assets/d8b850c8-0173-44d6-a5d9-ac1354193906)

## Technologies

| Technology | Purpose |
|---|---|
| Unity | Game engine |
| C# | Gameplay programming |
| Mirror | Multiplayer networking |
| Android | Target platform |
| IK Bones | Character aiming |
| Git / GitHub | Version control |

## Requirements

- Unity
- Android Build Support
- Android device(s)
- Devices connected to the same local network
- Mirror networking package

## Getting Started

Clone the repository:

```bash
git clone https://github.com/2gur1kan/LanAndroidTPSTamplate.git
```

Open the project with Unity and run the Main Menu / Hub scene.

For LAN testing, make sure the devices are connected to the same local network.

## Project Status

**Prototype / Development Template**

This project is a technical foundation for experimenting with Android TPS multiplayer over LAN.

The current implementation focuses on:

- LAN connectivity
- Host / Client architecture
- Character movement
- Third-person aiming
- IK Bone aiming
- Android multiplayer testing

The project can be expanded with additional weapons, maps, game modes, UI systems, and other multiplayer gameplay features.

## Developer

**Gürkan Gür**

Unity / C# Game Developer

- GitHub: https://github.com/2gur1kan
- itch.io: https://systembug.itch.io

## License

This project is provided for portfolio, educational, and development purposes.
