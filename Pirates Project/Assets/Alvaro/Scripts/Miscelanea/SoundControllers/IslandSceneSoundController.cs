using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandSceneSoundController : GeneralSoundController
{
    public AudioSource islandTheme;

    public override void FadeOutTheme()
    {
        StartCoroutine(FadeOutClip(1.0f, islandTheme));
    }
}
