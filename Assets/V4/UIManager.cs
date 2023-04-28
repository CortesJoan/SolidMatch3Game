using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI targetScoreText;

    private GameModel _gameModel;

    void Start()
    {
        _gameModel = FindObjectOfType<GameModel>();
        _gameModel.OnScoreChanged.AddListener(UpdateScoreText);    }

    void Update()
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        movesText.text = $"Moves: {_gameModel.moves}";
        timeText.text = $"Time: {_gameModel.remainingTime:F0}s";
        levelText.text = $"Level: {_gameModel.CurrentLevel + 1}";
        targetScoreText.text = $"Target: {_gameModel.levels[_gameModel.CurrentLevel].targetScore}";
    }
    
    public void UpdateScoreText(int newScore)
    {
        scoreText.text = $"Score: {newScore}";
    }
}