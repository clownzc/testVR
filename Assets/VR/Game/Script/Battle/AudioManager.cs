
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class AudioManager : MonoBehaviour
    {
        [Range(0, 1)]
        public float _volumn = 1;

        public GameObject _audioSourcePrefab;
        public AudioClip[] _audios;

        private GameUtil.AudioParamDict _audioDict;

        private void Awake()
        {
            _audioDict = new GameUtil.AudioParamDict(_audios);
        }

        private void OnEnable()
        {
            Messenger.AddListener<string, Vector3>("PlaySound", OnPlaySound);
            Messenger.AddListener<string[], Vector3>("PlaySoundRandom", OnPlaySoundRandom);
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<string, Vector3>("PlaySound", OnPlaySound);
            Messenger.RemoveListener<string[], Vector3>("PlaySoundRandom", OnPlaySoundRandom);
        }

        private void OnPlaySound(string name, Vector3 pos)
        {
            AudioClip clip = null;
            var exists = _audioDict.TryGetValue(name, out clip);
            if (!exists) return;

            if (_audioSourcePrefab != null)
            {
                var go = Instantiate(_audioSourcePrefab, pos, Quaternion.identity);
                go.transform.parent = transform;

                var audioSource = go.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                    audioSource.volume *= _volumn;
                    StartCoroutine(AudioPlayCallback(audioSource, clip.length));
                }
            }
        }

        private void OnPlaySoundRandom(string[] names, Vector3 pos)
        {
            var index = Mathf.RoundToInt(Random.Range(0, names.Length - 1));
            var name = names[index];
            OnPlaySound(name, pos);
        }

        private IEnumerator AudioPlayCallback(AudioSource audioSource, float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(audioSource.gameObject);
        }
    }

}