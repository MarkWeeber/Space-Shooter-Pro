using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts._Refactoring
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