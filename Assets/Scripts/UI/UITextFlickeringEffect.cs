using System.Collections;
using TMPro;
using UnityEngine;

public class UITextFlickeringEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _flickerRate = 0.5f;
    private bool _enabled, _hide;
    private string _storedText;

    private void OnEnable()
    {
        _enabled = true;
        _storedText = _text.text;
        _hide = false;
        StartCoroutine(Flicker());
    }

    private void OnDisable()
    {
        _enabled = false;
    }

    IEnumerator Flicker()
    {
        while (_enabled)
        {
            yield return new WaitForSeconds(_flickerRate);
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
