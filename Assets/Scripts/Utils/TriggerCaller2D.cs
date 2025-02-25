using System;
using UnityEngine;

namespace SpaceShooterPro
{
    public class TriggerCaller2D : MonoBehaviour
    {
        [SerializeField] private string _targetTag = "";
        [SerializeField] private bool _runOnlyOnce = true;
        [SerializeField] private Collider2D _collider2D;
        public Action<Collider2D> OnTriggerEnter2DEvent; 
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(_targetTag))
            {
                OnTriggerEnter2DEvent?.Invoke(collision);
                if (_runOnlyOnce)
                {
                    _collider2D.enabled = false;
                }
            }
        }
    }
}