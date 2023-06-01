using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameModel model;
    public GameView view;

    private int currentLevel = 0;
    private int currentRandomSeed;
    [Header("Timers")] [SerializeField] private float checkConditionInterval = 1f;
    private float elapsedTime = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeLevel(currentLevel);
        view.inputHandler.onTilesSwapped.AddListener(OnTilesSwapped);
        view.board.onMatch.AddListener(OnMatch);
        view.board.onMatchStart.AddListener(OnMatchStart);
        view.board.onBoardRefillComplete.AddListener(OnBoardRefillComplete);
        model.OnScoreChanged.AddListener((s) => CheckGameState());
        view.OnTileMatchFound.AddListener(OnTileMatchFound);
        currentRandomSeed = UnityEngine.Random.seed;

        elapsedTime = 0f;
    }

    public void InitializeLevel(int levelIndex)
    {
        currentLevel = levelIndex;
        LevelObjective objective = model.levels[currentLevel];
        model.moves = objective.maxMoves;
        model.totalTime = objective.maxTime;
        model.remainingTime = model.totalTime;
        model.score = 0;
        model.SetComboMultiplier(1);
    }

    void Update()
    {
        if (model.remainingTime > 0)
        {
            model.remainingTime -= Time.deltaTime;
        }
        else
        {
            model.remainingTime = 0;
        }
        CheckGameState();
        CheckLoseCondition();
    }

    public void OnGameOver()
    {
        Debug.Log("You lost");
        // TODO: Display "Game Over" message or other related GUI
    }


    public void IncreaseCombo()
    {
        model.SetComboMultiplier(model.GetComboMultiplier() + 1);
    }

    public void ResetCombo()
    {
        model.SetComboMultiplier(1);
    }

    public void OnTilesSwapped(Tile tileA, Tile tileB)
    {
        StartCoroutine(view.board.SwapTiles(tileA, tileB));
        model.ConsumeMove();
        CheckGameState();
        
    }

    void OnMatchStart()
    {
        // Implement any logic needed on a match start, e.g. play a sound effect or show an animation
    }

    void OnBoardRefillComplete()
    {
        // Implement any logic needed when the board refilling is completed, e.g. update UI or check for win/lose conditions
    }

    private void CheckGameState()
    {
        CheckWinCondition();
        CheckLoseCondition();
    }

    private void CheckWinCondition()
    {
        LevelObjective levelObjective = model.levels[model.currentLevel];

        // Check if all victory conditions are met
        bool allConditionsMet = true;
        foreach (VictoryCondition condition in levelObjective.victoryConditions)
        {
            if (!condition.IsConditionMet(model))
            {
                allConditionsMet = false;
                break;
            }
        }

        if (allConditionsMet)
        {
            OnLevelComplete();
        }
    }

    private void OnLevelComplete()
    {
        // Implement logic for moving to the next level or showing a "Level Complete" message
        if (model.currentLevel < model.levels.Length - 1)
        {
            model.currentLevel++;
            InitializeLevel(model.currentLevel);
        }
        else
        {
            // Game completed (all levels completed)
        }
    }

    private void CheckLoseCondition()
    {
        LevelObjective levelObjective = model.levels[model.currentLevel];
        foreach (LoseCondition loseCondition in levelObjective.loseConditions)
        {
            if (loseCondition.IsConditionMet(model))
            {
                OnGameOver();
                break;
            }
        }
    }

    void OnTileMatchFound(int tileType, int numOfMatchedTiles)
    {
        // Handle the matched tiles in this method.
        // Example: Add score based on the number of matched tiles and tile type
    }
    

    private void OnMatch()
    {
        IncreaseCombo();
    }
}