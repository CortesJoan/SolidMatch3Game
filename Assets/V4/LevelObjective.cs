[System.Serializable]
public class LevelObjective
{
    public VictoryCondition[] victoryConditions;
    public LoseCondition[] loseConditions;

    public int targetScore;
    public int maxMoves;
    public float maxTime;
    public int refillType; // 1: Spawn new tiles, 2: Falling tiles from above

}