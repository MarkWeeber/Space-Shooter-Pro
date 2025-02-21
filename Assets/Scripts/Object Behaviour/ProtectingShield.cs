using System;
using UnityEngine;

public class ProtectingShield : MonoBehaviour
{
    [SerializeField] private Transform _shieldTransform;
    public Action OnDepleted;
    public Action<float, float> Changed;

    private float _value;
    public float Value { get => _value; set => _value = value; }
    private float _maxValue;
    public float MaxValue { get => _maxValue; set => _maxValue = value; }
    private bool _isActive;
    public bool IsActive { get => _isActive; set => _isActive = value; }

}

