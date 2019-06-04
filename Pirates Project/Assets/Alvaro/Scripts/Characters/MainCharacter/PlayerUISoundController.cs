using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUISoundController : MonoBehaviour
{
    public AudioSource coinSound;
    public AudioSource recoverHealth;
    public AudioSource pickKeySound;

    public void PlayCoinSound(bool value)
    {  
        if(value && !coinSound.isPlaying) coinSound.Play();
        else if(!value && coinSound.isPlaying) coinSound.Stop();
    }

    public void PlayRecoverHealth()
    {
        recoverHealth.Play();
    }

    public void PlayPickKey()
    {
        pickKeySound.Play();
    }
}
