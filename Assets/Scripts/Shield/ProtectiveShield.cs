using UnityEngine;

namespace SpaceShooterPro
{
    public class ProtectiveShield : MonoBehaviour
    {
        [SerializeField] private float _maxShieldValue;
        public float MaxShieldVale { get => _maxShieldValue; set => _maxShieldValue = value; }
        [SerializeField] private float _shieldValue;
        [SerializeField] private Transform _shieldTransform;
        public float ShieldValue { get => _shieldValue; }
        private bool _shieldActive;
        public bool IsShieldActive { get => _shieldActive; set => _shieldActive = value; }
        private Material _shieldMaterial;
        private float _shieldScale;

        private void Start()
        {
            _shieldMaterial = _shieldTransform.GetComponent<Renderer>().material;
            CheckShieldValue();
        }

        public void RestoreShield(float restoreAmmount)
        {
            _shieldValue = Mathf.Clamp(_shieldValue + restoreAmmount, 0f, _maxShieldValue);
            CheckShieldValue();
        }

        public void TakeDamage(float damageValue)
        {
            _shieldValue = Mathf.Clamp(_shieldValue - damageValue, 0f, _maxShieldValue);
            CheckShieldValue();
        }

        private void CheckShieldValue()
        {
            _shieldScale = _shieldValue / _maxShieldValue;
            UpdateTransparencyOfMaterial();
            if (_shieldValue > 0)
            {
                _shieldActive = true;
                _shieldTransform.gameObject.SetActive(true);
            }
            else // shield depleted
            {
                _shieldActive = false;
                _shieldTransform.gameObject.SetActive(false);
            }
        }

        private void UpdateTransparencyOfMaterial()
        {
            _shieldMaterial.color = new Color(
                    _shieldMaterial.color.r,
                    _shieldMaterial.color.g,
                    _shieldMaterial.color.b,
                    _shieldScale);
        }
    }
}
