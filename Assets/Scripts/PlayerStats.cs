using System;

public class PlayerStats
{
    public int Score { get; private set; }

    public event Action<int> OnScoreChange;

    public void AddScore(int value)
    {
        Score += value;
        
        OnScoreChange?.Invoke(Score);
    }
}
