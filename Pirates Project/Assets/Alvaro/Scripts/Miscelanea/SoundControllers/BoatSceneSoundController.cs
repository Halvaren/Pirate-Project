using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSceneSoundController : GeneralSoundController
{
    public AudioSource boatTheme;
    public AudioSource waterIdle;

    public override void FadeOutTheme()
    {
        StartCoroutine(FadeOutClip(1.0f, boatTheme));
        StartCoroutine(FadeOutClip(1.0f, waterIdle));
    }
}
