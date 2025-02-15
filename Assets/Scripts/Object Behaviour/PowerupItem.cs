using UnityEngine;

public class PowerupItem : MonoBehaviour
{
    [SerializeField] private string _targetTag = "";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == _targetTag)
        {
            
        }
    }
}
