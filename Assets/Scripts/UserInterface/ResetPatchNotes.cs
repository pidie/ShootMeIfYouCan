using UnityEngine;

namespace UserInterface
{
    public class ResetPatchNotes : MonoBehaviour
    {
        private RectTransform _rectTransform;

        private void Awake() => _rectTransform = GetComponent<RectTransform>();

        private void OnEnable() => _rectTransform.anchoredPosition = Vector2.zero;
    }
}