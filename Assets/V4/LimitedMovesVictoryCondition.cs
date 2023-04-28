using UnityEngine;

[CreateAssetMenu(menuName = "VictoryCondition/LimitedMoves")]
public class LimitedMovesVictoryCondition : VictoryCondition
{
    public int maxMoves;

    public override bool IsConditionMet(GameModel model)
    {
        return model.score >= model.levels[model.currentLevel].targetScore && model.moves >= 0;
    }
}