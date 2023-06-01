using System;

[Serializable]
public class CommandAddScore : Command
{
    private GameModel gameModel;
    private int scoreValue;

    public CommandAddScore(GameModel gameModel, int scoreValue)
    {
        this.gameModel = gameModel;
        this.scoreValue = scoreValue;
    }

    public override void DoCommand(bool force = false)
    {
        gameModel.AddScore(scoreValue);
    }

    public override void UndoCommand(bool force = false)
    {
        gameModel.AddScore(-scoreValue);
    }
}