using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonBehaviour<AudioManager>
{
    [SerializeField] private AudioSource _projectileSound;
    [SerializeField] private AudioSource _powerUpSound;
    [SerializeField] private AudioSource _explosionSound;
    
    public void PlayProjectileShoot(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(_projectileSound.clip, position);
    }
    
    public void PlayPowerUpPickup(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(_powerUpSound.clip, position);
    }

    public void PlayExplosion(Vector3 position)
    {
        AudioSource.PlayClipAtPoint(_explosionSound.clip, position);
    }    
}
