using System;

public class PlayerStats
{
    public int Score { get; private set; }

    public event Action<int, int> OnScoreChange;

    public void AddScore(int change)
    {
        Score += change;
        
        OnScoreChange?.Invoke(Score, change);
    }
}
