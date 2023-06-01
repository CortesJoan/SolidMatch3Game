using System;
using Random = UnityEngine.Random;

[Serializable]

public class CommandSaveLoadRandomState : Command
{
    private Random.State previousState;
    private Random.State currentState;

    public int minInclusive, maxExclusive;

    public int result;

    public CommandSaveLoadRandomState(int min, int max)
    {
        previousState = Random.state;
        minInclusive = min;
        maxExclusive = max;
        currentState = previousState;
    }

    public override void DoCommand(bool force = false)
    {
        result = Random.Range(minInclusive, maxExclusive);
        currentState = Random.state;
    }

    public override void UndoCommand(bool force = false)
    {
        Random.state = previousState;
        currentState = previousState;
        result = 0;
    }
}