using System;

public class PlayerStats
{
    public int Score { get; private set; }
    public int Lives { get; private set; }

    public event Action<int, int> OnScoreChange;
    public event Action<int, int> OnLivesChange;

    public PlayerStats()
    {
        Score = 0;
        Lives = 10;
    }
    
    public void AddScore(int change)
    {
        Score += change;
        
        OnScoreChange?.Invoke(Score, change);
    }
    
    public void AddLife(int change)
    {
        Lives += change;
        
        OnLivesChange?.Invoke(Lives, change);
    }
}
