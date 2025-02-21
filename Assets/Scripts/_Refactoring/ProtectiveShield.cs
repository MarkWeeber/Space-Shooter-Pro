using UnityEngine;

namespace Assets.Scripts._Refactoring
{
    public class ProtectiveShield : MonoBehaviour
    {
        private float _maxShieldValue;
        public float MaxShieldValue { get { return _maxShieldValue; } set { _maxShieldValue = value; ShieldValueChanged(); } }
        private float _shieldValue;
        public float ShieldValue { get { return _shieldValue; } set { _shieldValue = value; ShieldValueChanged(); } }

        private Material _shieldMaterial;
        private float _shieldScale;

        private void Start()
        {
            _shieldMaterial = GetComponent<Renderer>().material;
        }

        private void ShieldValueChanged()
        {
            _shieldScale = _shieldValue / _maxShieldValue;
            UpdateTransparencyOfMaterial();
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
