using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSoundController : MonoBehaviour
{
    public AudioSource selectSound;

    public void PlaySelectSound()
    {
        selectSound.Play();
    }

    public virtual void FadeOutTheme()
    {
        
    }

    protected IEnumerator FadeOutClip(float time, AudioSource audio)
    {
        float initialVolume = audio.volume;
        float finalVolume = 0f;

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            audio.volume = Mathf.Lerp(initialVolume, finalVolume, elapsedTime / time);

            yield return null;
        }
        audio.volume = finalVolume;
    } 
}
