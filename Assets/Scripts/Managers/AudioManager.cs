using UnityEngine;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _audioSource;
    
        public static AudioManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
                Destroy(this);
            else
                instance = this;

            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayOneShot(AudioClip sound)
        {
            if (sound != null)
                _audioSource.PlayOneShot(sound);
        }
    }
}