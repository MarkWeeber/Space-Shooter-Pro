using UnityEngine;
using UnityEngine.Events;

namespace SpaceShooterPro
{
    public class TriggerOnDestroy : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onDestroyEvent;

        private void OnDestroy()
        {
            _onDestroyEvent?.Invoke();
        }
    }
}