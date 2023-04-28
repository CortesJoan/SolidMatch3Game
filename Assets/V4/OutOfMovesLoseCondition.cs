using UnityEngine;

[CreateAssetMenu(menuName = "LoseCondition/OutOfMoves")]
public class OutOfMovesLoseCondition : LoseCondition
{
    public override bool IsConditionMet(GameModel model)
    {
        return model.moves <= 0;
    }
}