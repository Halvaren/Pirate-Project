using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSoundController : GeneralSoundController
{
    public AudioSource mainTheme;

    public override void FadeOutTheme()
    {
        StartCoroutine(FadeOutClip(1.0f, mainTheme));
    }  
}
