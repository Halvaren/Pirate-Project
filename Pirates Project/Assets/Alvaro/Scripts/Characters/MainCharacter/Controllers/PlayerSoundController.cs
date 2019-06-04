using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundController : CharacterSoundController
{
    public AudioSource gunShot;
    public AudioSource gunDrawn;

    public AudioSource sableDrawn;  

    public void PlayGunShot()
    {
        gunShot.Play();
    }

    public void PlayGunDrawn()
    {
        gunDrawn.Play();
    }

    public void PlaySableDrawn()
    {
        sableDrawn.Play();
    }

}
