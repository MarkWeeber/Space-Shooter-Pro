using UnityEngine;

public class HealthPickUp : PickUpItem
{
    [SerializeField] private float _healthReplenishAmount = 15f;
    protected override void OnPicked()
    {
        player.ReplenishHealth(_healthReplenishAmount);
    }
}