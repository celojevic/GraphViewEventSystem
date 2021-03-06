using System.Collections.Generic;
using UnityEngine;

namespace EventGraph.Audio
{

    public class AudioManager : MonoBehaviour
    {

        [SerializeField] private List<AudioClip> _audioClips = new List<AudioClip>();
        public static Dictionary<string, AudioClip> audioClips;

        private void Awake()
        {
            InitAudioDict();
        }

        private void InitAudioDict()
        {
            audioClips = new Dictionary<string, AudioClip>();
            foreach (var item in _audioClips)
                audioClips.Add(item.name, item);
        }

        public static AudioClip GetAudioClip(string name)
        {
            if (!audioClips.HasElements()) return null;

            audioClips.TryGetValue(name, out AudioClip clip);
            return clip;
        }

    }

}
