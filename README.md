# 🍺 Alcolimit

> **Fine-tune how drunk you are, fool the breathalyser and outrun the police all the way home.**

**Alcolimit** is a first-person 3D simulation, exploration and driving game made in **Unity** by **Dual Soul Games** for the *Video Game Programming* university course.

[![Watch the tutorial on YouTube](https://img.shields.io/badge/▶_Watch_tutorial-YouTube-red?style=for-the-badge&logo=youtube)](https://youtu.be/j3t6Ko1aUPo)
![Unity](https://img.shields.io/badge/Unity-3D-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![Platform](https://img.shields.io/badge/Platform-PC-blue?style=for-the-badge&logo=windows)

> ℹ️ The game's text and the code are in Spanish.

---

## 📑 Table of Contents

- [Story](#-story)
- [Game Concept](#-game-concept)
- [Drunkenness System](#-drunkenness-system)
- [Controls](#️-controls)
- [Levels and Game Flow](#️-levels-and-game-flow)
- [HUD](#️-hud)
- [Art and Sound](#-art-and-sound)
- [Programming](#-programming)
- [Project Structure](#-project-structure)
- [Third-Party Assets](#-third-party-assets)
- [Use of AI](#-use-of-ai)
- [Team](#-team)

---

## 📖 Story

**The premise.** A disastrous night, far too many drinks and a huge police checkpoint blocking the only way out. You're trapped.

**The twist.** An urban legend turns out to be true: the patrol's breathalyser is an outdated model with a *firmware* bug. If it detects a massive and **extremely specific** blood alcohol level, it crashes and reads **0.0**.

**The goal.** Head back into the bar, blindly calibrate how drunk you are to "hack" the checkpoint, then survive a frantic car chase all the way to your garage.

### Characters

- **The Driver (protagonist):** an anonymous citizen who has had a terrible night. He has no lines of dialogue, to maximise first-person immersion.
- **The Police Officer:** the authority guarding the checkpoint. Strict, but held back by obsolete equipment. Miss the exact range and it's instant *Game Over*.

---

## 🎮 Game Concept

Each run is split into two timed phases:

1. **🍸 Bar phase (infiltration and drinking).** You have **5 minutes** to explore the bar, drink and adjust your alcohol level to the target range before you leave or get thrown out when time runs out.
2. **🚗 Driving phase (the getaway).** If you get past the checkpoint, you have to drive drunk and dodge the police until you get home. If your level was wrong, you're arrested on the spot.

---

## 🥴 Drunkenness System

The player's state is driven by an internal alcohol value from **0 to 10**:

- **Starting level:** you start at **1** and can never go below it.
- **Faulty range:** at the start of each run, the range where the breathalyser fails is picked at random: **5–6, 6–7, 7–8 or 8–9**. It's shown in an intro comic panel and stays visible in the HUD during the bar phase.
- **Blind interface:** you know the target range but **not your exact level**. You have to read your character's symptoms:
  - **Camera distortion:** constant swaying and bobbing that simulates loss of balance.
  - **Colour filter:** a green tint that gets stronger with each alcohol level.

### Ways to Control Your Alcohol Level

| Action | Where | Effect |
|---|---|---|
| 🍾 **Drink** | Bottles spread around the bar | Raises your level |
| 🚽 **Pee** | Upstairs toilets | Lowers it moderately |
| 💃 **Dance** | Dance floor | Lowers it moderately |
| 🤮 **Throw up** | Bathroom sink (only when your level is critically high) | Lowers it drastically |

---

## ⌨️ Controls

| Input | Bar Phase | Driving Phase | Behaviour |
|---|---|---|---|
| `W` `A` `S` `D` | Move | Steer | Moves the character / turns the steering wheel |
| `Mouse` | Look | Look | First-person camera |
| `E` | Interact | – | Drink, pee, dance or throw up near the object |
| `W` | – | Accelerate | Drive forward |
| `S` | – | Brake / reverse | Slow down or reverse |
| `Space` | – | Handbrake | Emergency brake and drift |

---

## 🗺️ Levels and Game Flow

### 1. The Bar (two-storey interior)
- **Ground floor:** an open room with high tables, a long bar along the back wall and a dance floor with coloured tiles under a disco ball.
- **First floor:** a **VIP room** with sofas, low tables and a record player, plus the **toilets** (a sink area and a separate stall), which are key to lowering your alcohol level.
- The contextual prompt *"Pulsa [E] para..."* ("Press [E] to...") only appears when you get close to a point of interest.

### 2. Transition: the Police Checkpoint
A cutscene where your level is checked. If you're within the range, the breathalyser absurdly reads **0.0** and you can carry on.

### 3. The Road (the chase)
A city at night with building blocks, streetlights, street furniture and traffic. You drive in first person from the cockpit, keeping the green filter and sway you built up in the bar.

**🏁 Win condition:** reach your garage without being caught.

---

## 🖥️ HUD

A minimal interface that changes with each phase:

**Bar phase**
- **Target range** (top left), e.g. *"Rango objetivo: Entre 5 y 6"* ("Target range: between 5 and 6").
- **Alcohol bar** (top centre), from 0 to 10, showing only the numbers at each end.
- **Timer** (top right), counting down from `05:00`.
- **Centre crosshair** for aiming at bottles and interactive objects.

**Driving phase**
- **Police distance** (top centre), e.g. *"Policía a: 28,2 m"* ("Police: 28.2 m away").
- **Dynamic minimap** (bottom left) with the streets and the police car's position.
- **Magenta arrow** next to the minimap, always pointing towards your home.
- **Speedometer and gears** built into the car's dashboard.

---

## 🎨 Art and Sound

- **Style:** *low-poly* 3D with sharp edges, simple geometry and clean textures. Visual reference: *Gamble With Your Friends*.
- **Bar:** even, warm lighting that gives a false sense of calm, in contrast with the green filter and the drunken sway.
- **Road:** darker street lighting, broken up by the red and blue flashes of the police sirens.
- **Sound:** rhythmic electronic music in the bar; your engine and the police sirens during the chase.

---

## 💻 Programming

- **Procedural drunk camera.** The dizziness doesn't use pre-made animations: it's calculated with `Mathf.PerlinNoise` for an organic movement that scales with your alcohol level. The same value drives Unity's *Post-Processing* in real time (lens distortion and colour).
- **NPCs with NavMesh Surface and points of interest.** NPCs check which spots are free (bar, dance floor...) to pick their next destination and compute the best path while avoiding obstacles.
- **Traffic.** Cars follow 3 looping routes so they spread across the whole map. *Raycasts* and traffic lights at critical junctions prevent head-on jams.
- **Police chase AI.** The police car keeps updating its destination towards the player using NavMesh. If it keeps you within catch range long enough, the game stops and the defeat screen appears.

---

## 📁 Project Structure

```
Assets/
├── Scenes/
│   ├── MenuInicio.unity        # Main menu
│   ├── Introduccion.unity      # Intro comic panel with the target range
│   ├── Interior.unity          # Phase 1: the bar
│   ├── ControlPolicia.unity    # Transition: breathalyser checkpoint
│   └── Exterior.unity          # Phase 2: city chase
├── Scripts/                    # Game logic (C#)
│   ├── GameManager.cs, SistemaAlcohol.cs, TemporizadorNivel.cs
│   ├── FirstPersonController.cs, PlayerInteraction.cs, BottleInteractable.cs
│   ├── ZonaAlivio.cs, ZonaBaileJugador.cs, NPCMovimiento.cs, ZonaNPC.cs
│   ├── PoliciaIA.cs, GestorPersecucion.cs, CocheAleatorio.cs, SemaforoCritico.cs
│   └── CamaraMinimapa.cs, IndicadorMinimapa.cs, EfectosAlcoholCoche.cs, ...
├── Prefabs/ · Materials/ · Images/ · Sprites/ · Sounds/
└── PrefabsPackages/            # Third-party assets
```

### ▶️ Opening the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/franra18/Alcolimit.git
   ```
2. Open the folder with **Unity Hub** (or import the project's `.unitypackage` into a new 3D project).
3. Open `Assets/Scenes/MenuInicio.unity` and press **Play**.

---

## 📦 Third-Party Assets

| Asset | Author | Used for |
|---|---|---|
| **SimplePoly City** | VenCreations | Modular city with roads and traffic |
| **Interiors FREE** | Mnostva Art | Minimalist bar interior |
| **Creative Characters FREE** | ithappy | Customer and NPC models |
| **Ezereal Car Controller** | Ezereal | Vehicle controller |
| **Farland Skies – Low Poly** | Borodar | *Low-poly* skyboxes |

---

## 🤖 Use of AI

- **Gemini:** optimising and debugging scripts (traffic, police car pursuit), support with art and audio direction, and creating the start screen.
- **Unity AI:** generating *prefabs* (bottles and bathroom furniture) and organising scenes and placing objects in the bar.

---

## 👥 Team

**Dual Soul Games**

| Member | Roles |
|---|---|
| **Francisco Ramírez Cañadas** | Game design, 3D/2D art, level design and production |
| **Marcos Luque Montiel** | Game design, programming, UI and level design |
| **Luis Fernández Rodríguez** | Game design, programming, UI and sound |

---

> ⚠️ *Alcolimit is a work of fiction with a humorous tone. Don't drink and drive.*
