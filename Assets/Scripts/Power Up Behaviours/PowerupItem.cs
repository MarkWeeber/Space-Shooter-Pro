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
                OnPicked();
            }
        }
    }

    protected virtual void OnPicked()
    {
        Destroy(this.gameObject);
    }
}
