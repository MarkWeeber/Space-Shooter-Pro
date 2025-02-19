using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField] private TMP_Text _scoreValueText;
    [SerializeField] private Image _healthBarHorizontalFillImage;
    [SerializeField] private Color _fullHealthBarColor = Color.green;
    [SerializeField] private Color _emptyHealthBarColor = Color.red;
    [SerializeField] private Image[] _healthBarImages;
    public UnityEvent OnGameOver;
    private Color _healthBarColor;
    private float _healthBarFillRatio;
    private int _scoreValue;
    
    private void Start()
    {
        _scoreValueText.text = _scoreValue.ToString();
    }

    public void UpdateHealth(float startingHealth, float currentHealth)
    {
        if (_healthBarHorizontalFillImage != null)
        {
            _healthBarFillRatio = currentHealth / startingHealth;
            _healthBarHorizontalFillImage.fillAmount = _healthBarFillRatio;
            if (_healthBarImages != null)
            {
                _healthBarColor = Color.Lerp(_emptyHealthBarColor, _fullHealthBarColor, _healthBarFillRatio);
                foreach (var image in _healthBarImages)
                {
                    image.color = _healthBarColor;
                }
            }
        }
    }

    public void AddScore(int score)
    {
        _scoreValue += score;
        _scoreValueText.text = _scoreValue.ToString();
    }

    public void SetGameOver()
    {
        OnGameOver?.Invoke();
    }

}