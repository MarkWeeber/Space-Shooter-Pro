using UnityEngine;

public class SpeedPowerUp : PowerupItem
{
    [SerializeField] private float _boostDuration = 5f;
    [SerializeField] private float _boostFactor = 1.5f;

    protected override void OnPicked()
    {
        player.EnableSpeedBoost(_boostDuration, _boostFactor);
        base.OnPicked();
    }
}
