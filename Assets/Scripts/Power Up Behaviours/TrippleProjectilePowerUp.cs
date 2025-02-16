using UnityEngine;

public class TrippleProjectilePowerUp : PowerupItem
{
    [SerializeField]
    private float _timeInSeconds = 10f;

    protected override void OnPicked()
    {
        player.EnableTrippleProjectile(_timeInSeconds);
        base.OnPicked();
    }
}

