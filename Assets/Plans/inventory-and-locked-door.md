# Project Overview
- Game Title: The Waiting Pet (Inferred from project root)
- High-Level Concept: Top-down adventure/exploration with room transitions and item interaction.
- Players: Single player
- Target Platform: WebGL
- Render Pipeline: Built-in

# Game Mechanics
## Core Gameplay Loop
- Players explore rooms (Room1, Room2).
- Players interact with objects (bedside table) to find keys.
- Players use keys to unlock doors and transition between rooms.

## Controls and Input Methods
- Movement: WASD / Arrow Keys.
- Interaction: Space bar.
- Inventory: Visual grid in top-left.

# UI
- **Inventory Grid**: A 1x5 horizontal grid in the top-left corner.
- **Slots**: 5 background boxes (semi-transparent dark squares).
- **Icons**: Item sprites (e.g., Key) appearing inside slots when collected.
- **Dialogue**: Text panel at the bottom (using existing DialogueManager).

# Key Assets & Context
- **Scripts**:
    - `InventoryManager.cs`: Singleton to track items and update UI.
    - `KeyPickup.cs`: Interactable script for the table to give the key.
    - `LockedDoor.cs`: Controller for locked door logic, referencing `DoorTransition`.
- **Sprites**:
    - Key Sprite: `Assets/karsiori/Sprites/Pixel Art Key Pack - Animated/Key 11/GOLD/Key 11 - GOLD - frame0000.png`
- **Objects**:
    - `bedside table 1`: Target for key pickup.
    - `Door_Room1`: Target for locking/unlocking.

# Implementation Steps
## 1. Inventory System & UI
1. **Create `InventoryManager.cs`**:
    - Maintain a `List<string>` of item IDs.
    - Provide `AddItem(string id, Sprite icon)` and `HasItem(string id)` methods.
    - Manage the UI Slot icons.
2. **Create Inventory UI**:
    - In the existing `DialogueCanvas` (or a new one), create a `Panel` named `InventoryGrid`.
    - Set `RectTransform` to Top-Left.
    - Add a `HorizontalLayoutGroup`.
    - Create 5 child `Image` objects (Slots) with a background sprite/color.
    - Each Slot has a child `Image` (Icon) which is initially disabled.
3. **Wire UI to `InventoryManager`**:
    - Assign the Slot Icon images to an array in the `InventoryManager`.

## 2. Key Pickup Logic
1. **Create `KeyPickup.cs`**:
    - When player is in range and presses Space:
        - Call `InventoryManager.Instance.AddItem("Room1Key", keySprite)`.
        - Call `DialogueManager.Instance.ShowDialogue("I found a key")`.
        - Disable the pickup (so it's one-time).
2. **Setup Table**:
    - Add a `BoxCollider2D` (isTrigger) to `bedside table 1` if missing.
    - Attach `KeyPickup` and assign the Key sprite.

## 3. Locked Door Logic
1. **Modify `DoorTransition.cs`**:
    - Make `PerformTransition()` public so it can be called by other scripts.
2. **Create `LockedDoor.cs`**:
    - Properties: `bool isLocked = true`, `string requiredKeyId = "Room1Key"`.
    - In `Update`, if player is in range and presses Space:
        - If `isLocked`:
            - Check `InventoryManager.Instance.HasItem(requiredKeyId)`.
            - If found: 
                - `isLocked = false`.
                - `InventoryManager.Instance.RemoveItem(requiredKeyId)`.
                - `DialogueManager.Instance.ShowDialogue("Door unlocked!")`.
                - Call `GetComponent<DoorTransition>().PerformTransition()`.
            - Else:
                - `DialogueManager.Instance.ShowDialogue("the door is locked.")`.
        - Else:
            - Call `GetComponent<DoorTransition>().PerformTransition()`.
3. **Setup Door**:
    - Attach `LockedDoor` to `Door_Room1`.
    - Disable or adjust the `DoorTransition` component on `Door_Room1` so it doesn't trigger automatically on its own (e.g., set `requiredDirection` to a key that isn't used, or modify its logic to check for a lock). 
    - *Refinement*: It's better to modify `DoorTransition` directly to include a `isLocked` flag to avoid double-triggering or complex wiring.

## 4. Refined Script Changes (Direct Modification)
1. **`InventoryManager.cs`**: New script.
2. **`KeyPickup.cs`**: New script.
3. **`DoorTransition.cs`**:
    - Add `public bool isLocked;`
    - Add `public string requiredKeyId;`
    - Update `Update()` to check `isLocked` before calling `PerformTransition()`.
    - Handle the "locked" dialogue and key consumption.

# Verification & Testing
1. **Test Start**: Verify `Door_Room1` is locked. Press Space at door -> "the door is locked" appears.
2. **Test Pickup**: Go to `bedside table 1`, press Space -> "I found a key" appears, Key icon appears in top-left UI.
3. **Test Unlock**: Return to `Door_Room1`, press Space -> Key disappears from UI, door transitions to `Room2`.
4. **Test Persistence**: Return to `Room1` and try `Door_Room1` again. It should transition immediately without needing a key.
