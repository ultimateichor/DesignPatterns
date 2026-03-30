# FnafButBad – Prototype README

## Overview

FnafButBad is a 2D survival horror prototype built in Unity. The game combines multiple simultaneous threat systems with fast-paced multitasking gameplay. The player must survive increasingly chaotic encounters while completing burger orders to achieve victory.

This README explains how to open, run, and test the prototype within the Unity Editor.

---

## Requirements

To run this project, you will need:

* Unity Hub installed
* Unity Editor (recommended version: 2021.3 LTS or newer)
* A keyboard and mouse

If the exact Unity version is unknown, Unity Hub will prompt you to install the correct version when opening the project.

---

## Getting Started

1. Download or clone the project files to your local machine.

2. Open Unity Hub.

3. Click the **“Open”** button and select the project folder.

4. Allow Unity to import all assets. This may take a few minutes the first time.

5. Once loaded, open the main scene:
   `Assets/Scenes/SampleScene.unity`

---

## Running the Game

1. In the Unity Editor, ensure **SampleScene** is open.

2. Press the **Play** button at the top of the editor.

3. The game will start from the Main Menu.

4. Click **Start** to begin gameplay.

---

## Controls

* **Mouse Movement / Clicks** – Interact with buttons and UI
* **Spacebar** – Open/close the camera monitor
* **View Rotation** – Move the mouse to rotate between directions (Front, Left, Back, Right)

---

## Core Gameplay Instructions

You are stationed in an office and must survive while completing burger orders.

Rotate between views to access different systems:

Front view contains the gate and Simon Says panel

Left view contains the door control

Back view contains Burtle’s burger station

Use the camera monitor (Spacebar) to track threats before they reach you

Respond correctly to each threat before its timer expires

Complete burger orders to progress toward the win condition

---

## Win Condition

Complete **67 burger orders** to trigger the Win Screen.

---

## Lose Conditions

The game ends immediately if any of the following occur:

* Incorrect input during Simon Says
* Failure to react to N. Sigma in time
* Incorrect response to a 6 or 7 encounter
* Wrong ingredient selected for a burger order
* Burtle’s patience meter reaches zero

A jumpscare will play before transitioning to the Game Over screen.

---

## Scenes

* **Main Menu** – Entry point of the game
* **SampleScene** – Main gameplay scene
* **Game Over** – Displayed on death
* **Win Screen** – Displayed upon victory

Ensure all scenes are added to **Build Settings** if running a build.

---

## Building the Game (Optional)

To create a standalone build:

1. Go to **File > Build Settings**
2. Add all scenes in the correct order:

   * Main Menu
   * SampleScene
   * Game Over
   * Win Screen
3. Select your target platform (PC recommended)
4. Click **Build** and choose an output folder
5. Run the generated executable

---

## Notes

* This is a prototype; visuals and audio are intentionally rough and stylized
* Multiple threats run simultaneously, so overlapping events are expected
* Performance may vary depending on hardware

---

## Troubleshooting

If the game does not run correctly:

* Ensure the correct scene is loaded before pressing Play
* Check the Console for missing references or errors
* Verify all assets imported successfully
* Restart Unity if UI elements fail to appear

---

## Credits

Developed as a Unity prototype project.
All art and audio are custom-made for this game.

---
