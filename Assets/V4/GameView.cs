using UnityEngine;
using UnityEngine.Events;

public class GameView : MonoBehaviour
{
    public Board board;
    public UIManager uiManager;
    public InputHandler inputHandler;

    // The rest of the GameView methods will use board, uiManager and inputHandler
    // to manage visuals, animations, UI-events as well as any other UI-related tasks
    
    
    public UnityEvent<int,int> OnTileMatchFound
    {
        get { return board.onTileMatchFound; }
    }
}