using System;

public enum RemoveReason
{
    Undefined,
    Station,
    Collision,
    LostLocomotive,
    ExitGood,
    ExitBad,
    OffTrack,
}

public static class RemoveReasons {

    public static bool IsGood(RemoveReason reason)
    {
        switch (reason)
        {
            case RemoveReason.Station:
            case RemoveReason.ExitGood:
                return true;
            
            case RemoveReason.Undefined:
            case RemoveReason.Collision:
            case RemoveReason.LostLocomotive:
            case RemoveReason.ExitBad:
            case RemoveReason.OffTrack:
                return false;
            default:
                return false;
        }
    }
}