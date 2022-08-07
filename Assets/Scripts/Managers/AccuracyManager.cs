using System;
using TMPro;
using UnityEngine;

public class AccuracyManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI accuracyText;
    [SerializeField] private TextMeshProUGUI comboText;

    private float _accuracy;
    private float _hits;
    private float _misses;
    private float _combo;

    public static Action<bool> onUpdateAccuracy;
    public static Action onIncreaseCombo;

    private void Awake()
    {
        onUpdateAccuracy += UpdateAccuracy;
        onIncreaseCombo += IncreaseCombo;

        comboText.text = "";
    }

    private void UpdateAccuracy(bool hit)
    {
        if (hit)
            _hits += 1;
        else
        {
            _misses += 1;
            _combo = 0;
            comboText.text = "";
        }

        _accuracy = _hits / (_hits + _misses) * 100;

        // todo : Localize
        accuracyText.text = string.Format($"{_accuracy:F3}%");
    }

    private void IncreaseCombo()
    {
        _combo++;
        if (_combo > 1)
            comboText.text = $"{_combo}x";
    }
}