using UnityEngine;

public struct SimpleRateLimiter
{
    public float DropTime;
    private float _timeToDrop;
    private bool _ready;
    public bool IsReady(float time)
    {
        _ready = false;
        _timeToDrop = DropTime - time;
        if (_timeToDrop <= 0f)
        {
            _ready = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetNewRate(float time, float newRate)
    {
        if (_ready)
        {
            DropTime = time + newRate;
        }
    }
}