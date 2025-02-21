using UnityEngine;

public class PowerfullProjectilePickUp : PickUpItem
{
    [SerializeField] private float _duration = 10f;
    protected override void OnPicked()
    {
        player.EnablePowerfullProjectile(_duration);
    }
}
