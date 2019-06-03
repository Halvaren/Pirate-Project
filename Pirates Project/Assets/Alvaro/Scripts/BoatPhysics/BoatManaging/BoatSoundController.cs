using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSoundController : MonoBehaviour
{
    public AudioSource sailingSound;
    public AudioSource turningSailSound;

    // Start is called before the first frame update
    void Start()
    {
        sailingSound.volume = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSailingSoundVolume(float volume)
    {
        sailingSound.volume = volume;
    }

    public void PlayTurningSailSound()
    {
        turningSailSound.Play();
        StartCoroutine(VolumeChange(0.5f, 0f, 1f, turningSailSound, false));
    }

    public void StopTurningSailSound()
    {
        StartCoroutine(VolumeChange(0.5f, turningSailSound.volume, 0f, turningSailSound, true));
    }

    private IEnumerator VolumeChange(float time, float initialVolume, float finalVolume, AudioSource audioSource, bool stop)
    {
        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            audioSource.volume = Mathf.Lerp(initialVolume, finalVolume, elapsedTime / time);

            yield return null;
        }
        audioSource.volume = finalVolume;

        if(stop) audioSource.Stop();
    }
}
