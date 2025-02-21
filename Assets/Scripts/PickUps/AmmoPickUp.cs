using UnityEngine;

public class AmmoPickUp : PickUpItem
{
    [SerializeField] private int _ammoCount = 10;
    protected override void OnPicked()
    {
        player.AmmouCount += _ammoCount;
    }
}