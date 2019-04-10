using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {

    public class AudioController : MonoBehaviour
    {
        private AudioSource m_BackgroundMusicSource;
        public AudioSource BackgroundMusicSource
        {
            get {
                return m_BackgroundMusicSource;
            }
            set {
                m_BackgroundMusicSource = value;
            }
        }

        private AudioSource m_SoundEffectSource;
        public AudioSource SoundEffectSource {
            get {
                return m_SoundEffectSource;
            }
            set {
                m_SoundEffectSource = value;
            }
        }

        private int onRequest;

        //Puzzle sounds
        private AudioClip[] successSounds;
        private AudioClip[] simonSounds;

        private BundleController m_BundleController;
        public BundleController BundleController
        {
            get {
                if(m_BundleController == null) m_BundleController = GameManager.Instance.BundleController;
                return m_BundleController;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            onRequest = 0;
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void LoadAudioAssetBundle(string bundleName)
        {
            onRequest++;
            BundleController.SendAudioRequest(bundleName, this);
        }

        public void ResolveAudioRequest(string bundleName, AudioClip[] audioClips)
        {
            switch(bundleName)
            {
                case "successsounds":
                    successSounds = audioClips;
                    break;
                case "simonsounds":
                    simonSounds = audioClips;
                    break;
            }

            onRequest--;
        }

        public bool GetOnRequest()
        {
            return onRequest > 0; //Si está con alguna request, el valor será mayor que 0. Si está desocupado, no.
        }

        public void PlayBackgroundMusic() {
            //Clip (?)
            //BackgroundMusicSource.clip = clip;
            //BackgroundMusicSource.Play();
        }

        public void StopBackgroundMusic() {
            BackgroundMusicSource.Stop();
        }

        public void PlaySoundEffect(string genre, string name)
        {
            AudioClip clip;
            switch(genre)
            {
                case "successsounds":
                    clip = FoundSoundEffect(successSounds, name);
                    break;
                case "simonsounds":
                    clip = FoundSoundEffect(simonSounds, name);
                    break;
                default:
                    clip = null;
                    break;
            }

            if(clip != null)
            {
                SoundEffectSource.clip = clip;
                SoundEffectSource.Play();
            }
            else print("There is no audio with that name");
        }

        public void PlayRandomSoundEffectFromGenre(string genre)
        {
            AudioClip[] audioClips;
            switch(genre)
            {
                case "successsounds":
                    audioClips = successSounds;
                    break;
                case "simonsounds":
                    audioClips = simonSounds;
                    break;
                default:
                    audioClips = null;
                    break;
            }

            if(audioClips != null)
            {
                AudioClip clip = audioClips[Random.Range(0, audioClips.Length)];
                SoundEffectSource.clip = clip;
                SoundEffectSource.Play();
            }
            else print("There is no genre with that name");
        }

        public void StopSoundEffect() { SoundEffectSource.Stop(); }

        public AudioClip FoundSoundEffect(AudioClip[] audioClips, string name)
        {
            for(int i = 0; i < audioClips.Length; i++)
            {
                if(audioClips[i].name == name) return audioClips[i];
            }
            return null;
        }
    }
}