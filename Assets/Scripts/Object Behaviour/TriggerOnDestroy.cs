using UnityEngine;
using UnityEngine.Events;

public class TriggerOnDestroy : MonoBehaviour
{
    [SerializeField] private UnityEvent _onDestroyEvent;

    private void OnDestroy()
    {
        _onDestroyEvent?.Invoke();
    }
}
