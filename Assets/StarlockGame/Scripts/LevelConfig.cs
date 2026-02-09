using UnityEngine;

[System.Serializable]
public class LevelConfig
{
    public int levelNumber;
    public int pairsToSpawn;
    public float rotationSpeed;
    public bool reverseRotation;
    public int maxShapesInside;

    private static LevelConfig[] defaultConfigs = new LevelConfig[]
    {
        new LevelConfig { levelNumber = 1, pairsToSpawn = 3, rotationSpeed = 20f, reverseRotation = false, maxShapesInside = 10 },
        new LevelConfig { levelNumber = 2, pairsToSpawn = 4, rotationSpeed = 25f, reverseRotation = false, maxShapesInside = 10 },
        new LevelConfig { levelNumber = 3, pairsToSpawn = 5, rotationSpeed = 30f, reverseRotation = false, maxShapesInside = 10 },
        new LevelConfig { levelNumber = 4, pairsToSpawn = 5, rotationSpeed = 35f, reverseRotation = true, maxShapesInside = 9 },
        new LevelConfig { levelNumber = 5, pairsToSpawn = 6, rotationSpeed = 40f, reverseRotation = false, maxShapesInside = 9 },
        new LevelConfig { levelNumber = 6, pairsToSpawn = 6, rotationSpeed = 45f, reverseRotation = true, maxShapesInside = 8 },
        new LevelConfig { levelNumber = 7, pairsToSpawn = 7, rotationSpeed = 50f, reverseRotation = false, maxShapesInside = 8 },
        new LevelConfig { levelNumber = 8, pairsToSpawn = 7, rotationSpeed = 55f, reverseRotation = true, maxShapesInside = 7 },
        new LevelConfig { levelNumber = 9, pairsToSpawn = 8, rotationSpeed = 60f, reverseRotation = false, maxShapesInside = 7 },
        new LevelConfig { levelNumber = 10, pairsToSpawn = 10, rotationSpeed = 70f, reverseRotation = true, maxShapesInside = 6 },
    };

    public static LevelConfig GetConfig(int level)
    {
        if (level < 1) level = 1;
        if (level > defaultConfigs.Length) level = defaultConfigs.Length;

        return defaultConfigs[level - 1];
    }

    public static int GetTotalLevels()
    {
        return defaultConfigs.Length;
    }
}
