public abstract class GameState
{
    public abstract void EnterState(GameController gameController);
    public abstract void UpdateState(GameController gameController);
    public abstract void ExitState(GameController gameController);
}