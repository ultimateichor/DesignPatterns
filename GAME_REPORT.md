# FnafButBad — Game Report

## Overview

**FnafButBad** is a 2D survival horror game built in Unity, inspired by *Five Nights at Freddy's*. The player sits in a virtual office and must survive a full night by defending against multiple animatronic threats simultaneously. The game uses deliberately hand-drawn, janky art and custom audio, giving it a distinct amateur-charm aesthetic. Each threat uses a completely different mechanic, making the game a multi-tasking challenge where the player must constantly switch between minigames and defensive actions.

---

## How to Play

The player views their office from a first-person perspective and can rotate to face four directions: **Front**, **Right**, **Back**, and **Left**. Pressing **Spacebar** opens the security camera monitor, which shows live feeds from four cameras around the building.

Threats spawn independently on random timers. Multiple threats can be active at the same time, forcing the player to prioritize and multitask. Failing to deal with any threat in time triggers a jumpscare and sends the player to the Game Over screen.

The only win condition is completing **67 burger orders** for Burtle (see below). All other threats loop indefinitely until the player dies or wins.

---

## Characters & Threats

### 1. Sam the Simon
- **Direction:** Front view
- **Appearance:** A spider-crab creature carrying a taco
- **Mechanic:** Simon Says — Sam appears and generates a random color sequence (4–6 colors). The player must repeat the sequence by clicking the correct buttons (Red, Blue, Green, Yellow) in order.
- **Time Limit:** 1.5 seconds between each button press.
- **Spawn Rate:** Every 10–20 seconds.
- **Failure:** Pressing the wrong color or running out of time triggers a jumpscare and Game Over.

### 2. N. Sigma
- **Directions:** Front view (gate) or Left view (door) — chosen randomly each spawn
- **Appearance:** A large skull-faced threat
- **Mechanic:** N. Sigma approaches through the cameras before reaching the office. The player must check cameras to see where it is, then react:
  - **Front path:** Hold the gate **down** for 3 continuous seconds within a 6-second window.
  - **Left path:** Close the left door before the 6-second timer expires.
- **Spawn Rate:** Every 20–40 seconds.
- **Failure:** Not completing the correct defensive action in time triggers a jumpscare and Game Over.

### 3. 6 and 7
- **Direction:** Front camera (check via monitor)
- **Appearance:** Two distinct sprite variants that appear at the gate camera
- **Mechanic:** An audio cue plays when one of them spawns. The player must open the camera monitor, identify whether it is **6** or **7**, and react accordingly:
  - **If it's 6:** Do **NOT** pull the gate down. Pulling the gate kills you.
  - **If it's 7:** **Pull** the gate down. Leaving the gate up kills you.
- **Window:** 7 seconds to respond.
- **Spawn Rate:** Every 15–35 seconds.
- **Failure:** Taking the wrong action instantly triggers a jumpscare and Game Over.

### 4. Burtle
- **Direction:** Back view
- **Appearance:** A burger shop mascot
- **Mechanic:** Burtle demands burger orders. Each order requires clicking a specific sequence of ingredients (bread → ingredient → ingredient → bread). The player has a **patience meter** that drains over approximately 125 seconds. Completing an order correctly refills the meter by 30%.
- **Win Condition:** Complete **67 orders** to win the game.
- **Failure:** Clicking the wrong ingredient at any point instantly triggers Game Over. The patience meter reaching zero also ends the game.

---

## Planned Characters (Not Yet Implemented)

- **N. Bababooey:** Watch Camera 1 — slam the door the instant it leaves.
- **Barrel Roll / Gun Hand:** Place a "no fly" sign in front of Barrel Roll, but NOT in front of Gun Hand.

---

## Game Mechanics

### Office Navigation
The player can rotate their view to face four directions. The camera monitor blocks rotation while open.

| Direction | What's There |
|-----------|-------------|
| Front | Gate (drag mechanic), Simon Says panel, Sam's window |
| Right | Decorative area |
| Back | Burtle's burger station |
| Left | Door toggle button |

### Gate (Front)
Drag the gate **downward** to close it. It automatically returns to the open position when released. Used to defend against N. Sigma (front path) and the 6/7 threat.

### Door (Left)
Click to toggle the left door open or closed. Used to defend against N. Sigma (left path).

### Camera Monitor (Spacebar)
Slides up from the bottom of the screen. Shows four camera feeds:
- **Cam 0:** Front gate area (6/7 encounters, N. Sigma front approach)
- **Cam 1:** Front hallway
- **Cam 2:** Left hallway entrance
- **Cam 3:** Left hallway corner

### Simon Says Panel (Front)
A four-button panel (Red, Blue, Green, Yellow) that activates when Sam appears. Buttons light up and play sounds to show the sequence, then the player must repeat it.

### Burger Station (Back)
A row of ingredient buttons. Burtle issues a random order; the player clicks the correct ingredients in sequence to complete it.

---

## Win / Lose Conditions

### Win
- Complete **67 burger orders** for Burtle → Victory screen.

### Lose (any of the following)
- Press wrong Simon button or time out during Sam's sequence.
- Fail to hold the gate or close the door in time for N. Sigma.
- Take the wrong action during a 6 or 7 encounter.
- Press the wrong ingredient button during a Burtle order.
- Let Burtle's patience meter drain to zero.

All losses play a jumpscare animation and death sound, then load the Game Over screen after a short delay.

---

## Scenes

| Scene | Name | Description |
|-------|------|-------------|
| 0 | Main Menu | Title screen, Start button loads the game |
| 1 | SampleScene | Main game — all threats and mechanics |
| 2 | Game Over | Death screen, returns to main menu |
| 3 | Win Screen | Victory screen, returns to main menu |

---

## Audio

| Sound | Used For |
|-------|---------|
| 67spawn | 6/7 encounter start |
| 67Jumscaresound | 6/7 death |
| Bonk | 7 leaves safely |
| Footstepsound | 6 leaves safely |
| appearsimon | Sam appears |
| deathsimon | Sam kills player |
| Red1, Blue1, Green1, Yellow1 | Sam's Simon color voice cues |
| Player click sounds (x4) | Player Simon button presses |
| BurtleBell | New burger order |
| ButtonBurtleClick | Ingredient button press |
| HappyBurtle | Order completed |
| N. Sigma approach sound | N. Sigma nearing office |

---

## Technical Notes

- Built in **Unity** (2D)
- All threat systems run independently via coroutines and Update loops — multiple threats can be active at once
- Modular design: adding a new animatronic only requires a new manager script
- CanvasGroup alpha + raycasting used for clean show/hide of directional UI elements
- Callbacks connect SimonController to SamManager without tight coupling
