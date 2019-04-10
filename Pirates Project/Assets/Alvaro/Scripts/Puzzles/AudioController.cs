using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    //Puzzle sounds
    private AudioClip[] successSounds;
    private AudioClip[] simonSounds;   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
