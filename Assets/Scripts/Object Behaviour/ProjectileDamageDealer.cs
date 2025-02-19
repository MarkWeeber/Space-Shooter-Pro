using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileDamageDealer : DamageDealer
{
    private void OnEnable()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayProjectileShoot(transform.position);
        }
    }
}
