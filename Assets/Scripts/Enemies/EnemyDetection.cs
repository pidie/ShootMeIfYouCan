using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemyDetection : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private float glowLowOpacity = 0.1f;
        [SerializeField] private float glowHighOpacity = 0.75f;
        [SerializeField] private float fadeTime;

        private bool _passive;
        private bool _pulsing;
        private float _fadeIncrement;
        private Material _mat;
        private Color _emissionColor;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        private Color _lowOpacity;
        private Color _highOpacity;
    
        public Transform Target { get; private set; }
        public Vector3 PlayerDetectedFromPoint { get; private set; }
        public bool IsPlayerDetected => Target != null;

        private void Awake()
        {
            Target = null;
            _passive = true;
            _mat = meshRenderer.material;
            _emissionColor = _mat.GetColor(EmissionColor);
            _lowOpacity = new Color(_emissionColor.r * glowLowOpacity, _emissionColor.g * glowLowOpacity,
                _emissionColor.b * glowLowOpacity);
            _highOpacity = new Color(_emissionColor.r * glowHighOpacity, _emissionColor.g * glowHighOpacity,
                _emissionColor.b * glowHighOpacity);
            _mat.SetColor(EmissionColor, _lowOpacity);
            ResetPlayerDetectedFromPoint();
        }

        private void Update()
        {
            if (_passive && !_pulsing)
            {
                _pulsing = true;
                StartCoroutine(PulsateLight());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _passive = false;
                Target = other.transform;
                TurnUpEmissions();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _passive = true;
                Target = null;
                TurnDownEmissions();
            }
        }

        public void ResetPlayerDetectedFromPoint() => PlayerDetectedFromPoint = Vector3.zero;

        public void PlayerDetected(Vector3 point)
        {
            _passive = false;
            TurnUpEmissions();
            PlayerDetectedFromPoint = point;
        }

        public void PlayerLost()
        {
            _passive = true;
            TurnDownEmissions();
            ResetPlayerDetectedFromPoint();
        }

        public float Distance(Transform origin) => Target == null ? 1000f : Vector3.Distance(origin.position, Target.position);

        private void TurnUpEmissions()
        {
            _mat.SetColor(EmissionColor, _highOpacity);
        }
    
        private void TurnDownEmissions()
        {
            _mat.SetColor(EmissionColor, _lowOpacity);
        }

        private IEnumerator PulsateLight()
        {
            var currentEmissionColor = _emissionColor;
        
            if (_emissionColor.a <= 0.1f)
            {
                for (var t = 0.1f; t < fadeTime; t += _fadeIncrement)
                {
                    currentEmissionColor = Color.Lerp(_lowOpacity, _highOpacity, t / fadeTime);
                    _mat.SetColor(EmissionColor, currentEmissionColor);
                    yield return null;
                }
            }
            else
            {
                for (var t = 0.75f; t > fadeTime; t -= _fadeIncrement)
                {
                    currentEmissionColor = Color.Lerp(_lowOpacity, _highOpacity, t / fadeTime);
                    _mat.SetColor(EmissionColor, currentEmissionColor);
                    yield return null;
                }
            }
        
            yield return new WaitForSeconds(0.1f);
        }
    }
}