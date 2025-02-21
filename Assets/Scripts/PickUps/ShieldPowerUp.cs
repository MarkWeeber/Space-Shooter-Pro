using UnityEngine;

public class ShieldPowerUp : PickUpItem
{
    [SerializeField] private float _shieldHitPointsValue = 25;
    protected override void OnPicked()
    {
        player.EnableShield(_shieldHitPointsValue);
        base.OnPicked();
    }
}
