using UnityEngine;

public abstract class PowerupItem : MonoBehaviour
{
    protected Player player;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == GlobalVariables.PLAYER_TAG)
        {
            if (collision.TryGetComponent<Player>(out player))
            {
                PlayPickUpSound();
                OnPicked();
                Destroy(this.gameObject);
            }
        }
    }

    protected virtual void OnPicked()
    {
    }

    private void PlayPickUpSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayPowerUpPickup(transform.position);
        }
    }
}
