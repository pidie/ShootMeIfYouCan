using System;
using TMPro;
using UnityEngine;

public class KillCounter : MonoBehaviour
{
    [SerializeField] private TMP_Text killCountText;
    
    private int _killCount;

    public static Action onKillCountUpdate;

    private void Awake() => onKillCountUpdate += KillCountUpdate;

    // todo : Localize
    private void KillCountUpdate()
    {
        _killCount += 1;
        killCountText.text = _killCount.ToString();
    }
}
