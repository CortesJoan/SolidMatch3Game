using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "VictoryCondition/Base")]
public abstract class VictoryCondition : ScriptableObject
{
    public abstract bool IsConditionMet(GameModel model);
}