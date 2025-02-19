using TMPro;
using UnityEngine;

public class UITextFlickeringEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _flickerRate = 0.5f;
    private bool _enabled, _hide;
    private string _storedText;
    private float _timer;

    private void OnEnable()
    {
        _enabled = true;
        _storedText = _text.text;
        _hide = false;
        _timer = _flickerRate;
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    private void Update()
    {
        Flicker(Time.deltaTime);
    }

    private void Flicker(float deltaTime)
    {
        if (_enabled)
        {
            if (_timer > 0f)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                _timer = _flickerRate;
                if (_hide)
                {
                    _text.text = "";
                }
                else
                {
                    _text.text = _storedText;
                }
                _hide = !_hide;
            }
        }
    }
}
