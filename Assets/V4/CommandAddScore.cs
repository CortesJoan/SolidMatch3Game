public class CommandAddScore : Command
{
    // Un campo para guardar la referencia al modelo del juego
    private GameModel gameModel;
    // Un campo para guardar el valor a sumar o restar a la puntuación
    private int scoreValue;

    // El constructor recibe el modelo del juego y el valor a sumar o restar
    public CommandAddScore(GameModel gameModel, int scoreValue)
    {
        // Guarda los valores recibidos en los campos correspondientes
        this.gameModel = gameModel;
        this.scoreValue = scoreValue;
    }

    public override void DoCommand(bool force = false)
    {
        // Llama al método AddScore del modelo del juego pasándole el valor a sumar
        gameModel.AddScore(scoreValue);
    }

    public override void UndoCommand(bool force = false)
    {
        // Llama al método AddScore del modelo del juego pasándole el valor a restar (el opuesto al valor a sumar)
        gameModel.AddScore(-scoreValue);
    }
}