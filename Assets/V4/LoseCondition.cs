using UnityEngine;

public abstract class LoseCondition :ScriptableObject
{
    public abstract bool IsConditionMet(GameModel model);
}