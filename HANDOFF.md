# FnafButBad - Session Handoff Document

## Project Overview
A FNAF-style 2D office defense game in Unity with a deliberately bad/janky art style.
Project is uploaded to: https://github.com/ultimateichor/DesignPatterns (group project repo)
Local path: C:\Users\Tmcqu\FnafButBad

---

## Current Scene Structure (SampleScene)

### Hierarchy Layout
```
SampleScene
    GameManager          (empty, unused so far)
    Main Camera
    Global Light 2D
    Canvas
        FrontView        (SetActive based on direction, VISUAL ONLY)
            WindowArea
                WindowBackground
        BackView         (SetActive based on direction, VISUAL ONLY)
        RightView        (SetActive based on direction, VISUAL ONLY)
        LeftView         (SetActive based on direction, VISUAL ONLY)
            Door         (passive RectTransform, moved by DoorButton script)
        Gate             (UI Image, DragGate script, CanvasGroup for visibility)
        DoorButton       (UI Image, ToggleDoor script, CanvasGroup for visibility)
        SimonPanel       (UI parent, CanvasGroup for visibility)
            GreenButton  (SimonButton script, ButtonColor = Green)
            RedButton    (SimonButton script, ButtonColor = Red)
            BlueButton   (SimonButton script, ButtonColor = Blue)
            YellowButton (SimonButton script, ButtonColor = Yellow)
        SamVisual        (UI Image, Sam sprite, Image.enabled controlled by SamManager)
        SamJumpscare     (UI Image, fullscreen, enabled on death, MUST be last in Canvas)
        CameraMonitor    (slides up/down with Space)
            MonitorBackground
            FeedImage
            CameraFeedSystem
            CameraButtons
                Cam1Button
                Cam2Button
                Cam3Button
                Cam4Button
    EventSystem
    OfficeController     (OfficeTurner script)
    CameraSystem         (CameraMonitorController script)
    SamManager           (SamManager script)
    SimonController      (SimonController script, AudioSource component)
```

---

## Scripts Summary

### OfficeTurner.cs
- On: OfficeController
- Controls 4 directional views with SetActive
- Controls CanvasGroup visibility for: Gate (front=0), DoorButton (left=3), SimonPanel (front=0), SamVisual (front=0)
- Blocks turning input when monitor is open
- Direction: 0=Front, 1=Right, 2=Back, 3=Left

### DragGate.cs
- On: Gate
- Click and drag downward to lower gate, releases back up
- Uses offset-based drag (not delta) for no desync
- upY = 460, downY = 142

### ToggleDoor (HoldDoor.cs)
- On: DoorButton
- Click to toggle door open/closed
- Won't toggle while door is mid-movement (snap threshold)
- Swaps button sprite between open (red) and closed (green)
- Door references: openX = -695, closedX = -215

### CameraMonitorController.cs
- On: CameraSystem
- Space to toggle monitor sliding up/down
- openY and closedY control slide positions

### CameraFeedController.cs
- On: CameraFeedSystem (inside CameraMonitor)
- 4 camera feeds, switched by clicking cam buttons
- ShowCamera(int index) called by button OnClick events

### SimonButton.cs
- On: Each of the 4 Simon buttons
- Has ButtonColor enum (Red, Blue, Green, Yellow)
- Calls simonController.ReceivePlayerInput(color) on click
- IMPORTANT: SimonController reference must be assigned in Inspector

### SimonController.cs
- On: SimonController object
- Has AudioSource component on same object
- Manages pattern playback with Sam's voice clips
- Manages player input with separate player button sound clips
- Flash uses sprite swap (normal vs lit sprites)
- Has OnSuccess and OnFailure callbacks used by SamManager
- inputTimeout = 1.5f (time between each button press)

### SamManager.cs
- On: SamManager object
- Has AudioSource component on same object
- Controls SamVisual Image.enabled (true when Sam is attacking)
- Controls SamJumpscare Image.enabled (true on death)
- On failure: shows jumpscare → waits for deathClip.length → loads scene 2 (GameOver)
- On success: hides Sam, loop continues
- Difficulty: minPatternLength=4, maxPatternLength=6, minSpawnDelay=10, maxSpawnDelay=20

---

## Scenes
- Scene 0: MainMenu (IN PROGRESS - layout done, needs MainMenuController script)
- Scene 1: SampleScene (main game, mostly working)
- Scene 2: GameOver (done, has GameOverController script, button loads scene 0)

### MainMenuController.cs (NOT YET CREATED)
- Needs to be created and put on a MainMenuManager object in MainMenu scene
- Start button should call StartGame() which loads scene 1

---

## Sprites (Assets/Sprites)
- FrontView, FrontView2, FrontView3 — front office backgrounds
- BackView — back wall
- LeftView, LeftView2 — left wall with door
- RightView — right wall with fishbowl
- FrontGrate — gate/window sprite
- Door, DoorButton, DoorButton_1 (green/closed), DoorButton_2 (red/open)
- simonsayssegments_0 through _3 — 4 Simon wheel quadrants (Unity auto-sliced)
- Sam sprite — the Sam the Simon animatronic (spider-crab body, holding taco)
- Sam jumpscare sprite — shown on death
- CameraTablet, CameraBG, CamLabel — camera monitor UI pieces

## Audio
- Red1, Blue1, Green1, Yellow1 — Sam's voice clips saying each color
- Player button sounds — separate clips for when player clicks Simon buttons
- Sam appear clip — plays when Sam spawns
- Death clip — plays on game over

---

## Animatronic Designs (NOT YET IMPLEMENTED)
1. **Burtle** — make burger meals to progress the night, too slow = death
2. **6/7** — audio cue → check cam 3 → close door for 6, leave open for 7, wrong = death
3. **N. Bababooey** — watch cam 1, instant door slam the moment it leaves cam 1
4. **N. Sigma (WIP)** — slowly approaches front gate or left door
5. **Sam the Simon** ✅ IMPLEMENTED — Simon Says in the office, appears front view
6. **Barrel Roll / Gun Hand** — put no fly sign in front of Barrel Roll, NOT Gun Hand

---

## Important Architecture Notes
- View GameObjects (FrontView etc) are VISUAL ONLY — no gameplay scripts on them
- All gameplay logic lives on always-active objects outside the view hierarchy
- Gate and DoorButton use CanvasGroup alpha/blocksRaycasts for show/hide (not SetActive)
- SimonPanel and SamVisual use CanvasGroup for direction-based visibility
- SamVisual also uses Image.enabled for presence-based visibility (controlled by SamManager)
- Door inside LeftView is fine staying there — it's a passive object moved by DoorButton script
- The freeze bug is SOLVED by this architecture

---

## What To Do Next (in order)
1. Finish MainMenu scene — create MainMenuController.cs, hook up Start button
2. Test full loop: Menu → Game → Die to Sam → Game Over → Back to Menu
3. Push everything to GitHub (ultimateichor/DesignPatterns)
4. Start next animatronic (suggested: N. Sigma since it teaches enemy movement foundation)
