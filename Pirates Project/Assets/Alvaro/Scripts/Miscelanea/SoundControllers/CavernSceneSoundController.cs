using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CavernSceneSoundController : GeneralSoundController
{
    public AudioSource cavernTheme;
    public AudioSource actionTheme;

    public void PlayCavernTheme()
    {
        StartCoroutine(FadeOutClip(1.0f, actionTheme));
        StartCoroutine(FadeInClip(1.0f, cavernTheme));
    }

    public void PlayActionTheme()
    {
        if(!actionTheme.isPlaying) actionTheme.Play();
        StartCoroutine(FadeOutClip(1.0f, cavernTheme));
        StartCoroutine(FadeInClip(1.0f, actionTheme));
    }

    public override void FadeOutTheme()
    {
        StartCoroutine(FadeOutClip(1.0f, cavernTheme));
        StartCoroutine(FadeOutClip(1.0f, actionTheme));
    } 

    private IEnumerator FadeInClip(float time, AudioSource audio)
    {
        float initialVolume = audio.volume;
        float finalVolume = 1f;

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
