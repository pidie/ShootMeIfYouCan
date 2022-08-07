using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class BatteryLevel : MonoBehaviour
{
    private Image _image;
    private bool _flashColor;
    private bool _colorFlop;
    private Color _initColor = GameManager.GetColor();
    private bool _depleted;

    public static Action<Color> onUpdateColor;
    public static Action<bool> onIsDepleted;

    private void Awake()
    {
        _image = GetComponent<Image>();
        StartCoroutine(FlashImage());
        onUpdateColor += UpdateColor;
        onIsDepleted += IsDepleted;
        _flashColor = true;
    }

    private void Update()
    {
        if (_depleted && _flashColor)
        {
            _flashColor = false;
            _image.color = _colorFlop ? _initColor : Color.white;
            StartCoroutine(FlashImage());
        }
        else
        {
            _image.color = _initColor;
        }
    }

    private void IsDepleted(bool value) => _depleted = value;

    private void UpdateColor(Color newColor) => _initColor = newColor;

    private IEnumerator FlashImage()
    {
        yield return new WaitForSeconds(0.5f);
        _colorFlop = !_colorFlop;
        _flashColor = true;
    }
}