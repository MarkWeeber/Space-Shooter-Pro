using UnityEngine;

public struct SimpleRateLimiter
{
    public float DropTime;
    private float _timeToDrop;

    public bool IsReady(float time, float newRate)
    {
        _timeToDrop = DropTime - time;
        if (_timeToDrop <= 0f)
        {
            DropTime = time + newRate;
            return true;
        }
        else
        {
            return false;
        }
    }
}