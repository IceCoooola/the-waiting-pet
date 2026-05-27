using System.Collections.Generic;

public static class GameData
{
    // Puzzle States
    public static bool CandlestickPuzzleSolved = false;
    public static bool FootstepPuzzleSolved = false;
    public static bool EyePuzzleSolved = false; // For PuzzleManager.cs

    // Door States (using unique IDs or names)
    private static HashSet<string> unlockedDoors = new HashSet<string>();

    public static void UnlockDoor(string doorId)
    {
        if (!unlockedDoors.Contains(doorId))
        {
            unlockedDoors.Add(doorId);
        }
    }

    public static bool IsDoorUnlocked(string doorId)
    {
        return unlockedDoors.Contains(doorId);
    }
}
