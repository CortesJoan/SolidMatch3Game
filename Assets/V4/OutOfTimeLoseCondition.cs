using UnityEngine;

[CreateAssetMenu(menuName = "LoseCondition/OutOfTime")]
public class OutOfTimeLoseCondition : LoseCondition
{
    public override bool IsConditionMet(GameModel model)
    {
        return model.remainingTime <= 0;
    }
}