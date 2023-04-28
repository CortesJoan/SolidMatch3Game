using UnityEngine;

[CreateAssetMenu(menuName = "VictoryCondition/Score")]
public class ScoreVictoryCondition : VictoryCondition
{
    public int targetScore;

    public override bool IsConditionMet(GameModel model)
    {
        return model.score >= targetScore;
    }
}