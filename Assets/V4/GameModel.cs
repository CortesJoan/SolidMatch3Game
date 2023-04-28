using UnityEngine;
using UnityEngine.Events;


public class GameModel : MonoBehaviour
{
    public int score;
    public int moves;
   [SerializeField] private int comboMultiplier;
    public int currentLevel;
    
    public LevelObjective[] levels;
    public float totalTime;
    public float remainingTime;
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<int,int> OnTileMatchFound;
    
    public int CurrentLevel => currentLevel;

    public void ResetComboMultiplier()
    {
        comboMultiplier = 1;
    }

    public void IncreaseComboMultiplier()
    {
        comboMultiplier++;
    }
    public void ConsumeMove()
    {
        moves--;
        comboMultiplier = 1;
    }
    public void AddScore(int value)
    {
        score += value * comboMultiplier;
        OnScoreChanged.Invoke(score);
    }
   
    public int GetScore()
    {
        return score;
    }

    public void SetScore(int value)
    {
        score = value;
    }

    public int GetMoves()
    {
        return moves;
    }

    public void SetMoves(int value)
    {
        moves = value;
    }

    public int GetComboMultiplier()
    {
        return comboMultiplier;
    }

    public void SetComboMultiplier(int value)
    {
        comboMultiplier = value;
    }
}