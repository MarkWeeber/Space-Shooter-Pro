using UnityEngine;

public class TrippleProjectilePowerUp : PickUpItem
{
    [SerializeField]
    private float _timeInSeconds = 10f;

    protected override void OnPicked()
    {
        player.EnableTrippleProjectile(_timeInSeconds);
        base.OnPicked();
    }
}

