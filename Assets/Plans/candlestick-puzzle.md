# Project Overview
- Game Title: The Waiting Pet
- High-Level Concept: A 2D platformer/interiors puzzle game where the player interacts with props and solves room-based puzzles.
- Players: Single player
- Inspiration / Reference Games: Pixel Art Platformer
- Tone / Art Direction: 2D Pixel Art
- Target Platform: WebGL
- Render Pipeline: Built-in

# Game Mechanics
## Core Gameplay Loop
- Exploration: Move through the hallway and interact with objects.
- Puzzle Solving: Pick up candlesticks and place them in specific patterns on holders.
- Progression: Solve the puzzle to obtain a key and unlock the next area.

## Controls and Input Methods
- Movement: WASD / Arrow Keys.
- Interact: Space.
- Placement Choice: After interacting with a holder, a dialogue will prompt the player: "1. Left, 2. Middle, 3. Right". Press 1, 2, or 3 to select the spot.

# UI
- Inventory: 5 slots in the upper left corner (already implemented).
- Interaction Feedback: Dialogue boxes for item discovery, locked doors, and placement instructions.

# Key Asset & Context
- `CandlestickItem.cs`: Handles picking up individual candlesticks.
- `CandlestickHolder.cs`: Handles placing and removing candlesticks on the 3 spots. Shows instruction dialogue.
- `CandlestickPuzzleManager.cs`: Monitors the 4 holders and triggers the reward.
- `HallwayKey`: The key object that appears when the puzzle is solved.
- `Door_NoGround_main`: The door that requires the HallwayKey.

# Implementation Steps
## 1. Create Candlestick Scripts
### 1.1 Implement CandlestickItem in `Assets/Scripts/CandlestickItem.cs`
- Detect player trigger.
- On `Space`, add "Candlestick" to `InventoryManager` and deactivate self.
- **Dependency**: `InventoryManager.cs`

### 1.2 Implement CandlestickHolder in `Assets/Scripts/CandlestickHolder.cs`
- Define 3 slots (Left, Middle, Right).
- When `Space` is pressed, show dialogue: "Press 1: Left, 2: Middle, 3: Right".
- Use a small "interaction window" (e.g., 2 seconds) or a state flag to detect the subsequent `1/2/3` key press.
- If `1/2/3` is pressed:
    - If spot is empty and player has candlestick: Place candle (activate child sprite).
    - If spot is full and player has room in inventory: Pick back candle (deactivate child sprite, add to inventory).
- Call `CheckPuzzle()` on `CandlestickPuzzleManager`.
- **Dependency**: `DialogueManager.cs`, `InventoryManager.cs`

### 1.3 Implement CandlestickPuzzleManager in `Assets/Scripts/CandlestickPuzzleManager.cs`
- List of 4 `CandlestickHolder` references.
- `CheckPuzzle()` method:
    - If holder counts match `3, 1, 2, 0`:
        - Show dialogue: "Something appeared in the hallway..."
        - Activate `HallwayKey`.
- **Dependency**: `CandlestickHolder.cs`

## 2. Scene Setup
### 2.1 Prepare Holders
- Copy `candlesticks_empty` 3 times to create a total of 4 holders.
- Arrange them in a line in the Hallway.
- Attach `CandlestickHolder` script to each.
- Create 3 child Sprite objects for each holder (for the candles) and assign them to the holder's slots. Set them to inactive initially.

### 2.2 Prepare Candlestick Items
- Create/Configure 6 candlestick items in the room.
- Attach `CandlestickItem` script to each.
- Use a candlestick sprite (likely from `TX Village Props.png`).

### 2.3 Configure Puzzle Manager
- Create an empty GameObject `PuzzleManager`.
- Attach `CandlestickPuzzleManager` and assign the 4 holders in order.
- Assign the `HallwayKey` reference.

### 2.4 Configure Key and Door
- Set `HallwayKey` to inactive by default.
- Ensure `Door_NoGround_main` has `DoorTransition` with `isLocked = true` and `requiredKeyId = "HallwayKey"`.

# Verification & Testing
- Pick up all 6 candlesticks.
- Interact with Holder 1. Verify dialogue "Press 1: Left, 2: Middle, 3: Right" appears.
- Press 1, 2, and 3 to place candles. Verify the sprites appear and inventory updates.
- Place candles to match `3 1 2 0`.
- Verify `HallwayKey` appears.
- Unlock the door with the key.
