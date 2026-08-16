# Amelia's Potty Time

A Unity-based educational game about creating a sustainable potty schedule for a puppy named Amelia. Designed with accessibility and usability as core principles.

## Controls

- **Mouse/Touch only** — all interactions use click/tap on buttons and dropdowns
- No keyboard shortcuts required
- No time-pressure inputs

## How to Play

1. **Main Menu** — Press "New Game" to begin
2. **Story Screen** — Read the scenario, then press "Play"
3. **Setup Screen** — Choose how many potty trips (1–8) using the dropdown, then press "Next"
4. **Schedule Screen** — Set the hour, minute, and AM/PM for each trip using dropdowns. Press "Set" when done
5. **Results Screen** — Watch the clock animate through the day. Your scheduled times appear in green. If you missed a needed time, it appears in red
6. **Outcome Popup** — See whether you succeeded, failed, or over-scheduled. Press "Menu" to return

## Accessibility Features

### 1. Multi-Modal Feedback (Auditory + Visual)

- Button clicks produce audio feedback confirming the action was registered
- Each scheduled time that appears on the results screen plays a ding sound
- Win/lose outcomes have distinct audio cues in addition to visual popups
- Color-coded results (green/red) are reinforced by context: green times appear sequentially during normal playback, while the red time appears last and triggers the failure popup — meaning is not conveyed by color alone

### 2. Cognitive Accessibility

- Step-by-step flow with clear instructions at each stage
- Progress is shown via an animated clock (spatial/temporal indicator)
- Simple, low-distraction layout with one task per screen
- Immediate, descriptive feedback on outcomes (popup explains what happened and when)

### 3. Motor Accessibility

- Large click targets (oversized buttons and dropdowns)
- No time-pressure interactions — all inputs are untimed
- No drag operations or precision movements required
- Mouse-only control scheme (no keyboard required)

### 4. General Usability

- "Back" buttons on every screen allow undo/navigation correction
- Validation prevents invalid schedules (times must be sequential, within waking hours)
- Disabled states on buttons prevent premature actions (e.g., "Set" is disabled until schedule is valid)

## Project Structure

- `Assets/Scripts/` — Game logic (MainMenuController, ScheduleController, ResultsController, etc.)
- `Assets/Scenes/Game.unity` — Single scene containing all UI panels
- `Assets/Sprites/` — Visual assets
- `Assets/Sounds/` — Audio clips (button click, ding, win, lose)
- `Assets/Fonts/` — Custom typography
- `Assets/Prefab/` — Reusable UI components

## Built With

- Unity 6 (6000.5.7f1)
- TextMesh Pro
- Universal Render Pipeline (URP)
