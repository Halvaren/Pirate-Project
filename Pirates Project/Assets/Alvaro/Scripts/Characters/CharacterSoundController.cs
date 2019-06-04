using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundController : MonoBehaviour
{
    public AudioClip[] hurtSounds;
    public AudioClip[] stepSounds;
    public AudioClip[] swordHits;
    public AudioClip[] swordClashes;
    
    protected int hurtSoundIndex;
    protected int stepSoundIndex;
    protected int swordHitIndex;
    protected int swordClashIndex;

    public AudioSource hurtSound;
    public AudioSource deathSound;

    public AudioSource sableSlash;
    public AudioSource sableHitOnBody;
    public AudioSource sableHitOnSable;

    public AudioSource steps;

    public void PlayHurt()
    {
        hurtSound.clip = hurtSounds[hurtSoundIndex];
        hurtSound.Play();

        hurtSoundIndex++;
        if(hurtSoundIndex == hurtSounds.Length) hurtSoundIndex = 0;
    }

    public void PlayDeath()
    {
        deathSound.Play();
    }

    public void PlayStep()
    {
        steps.clip = stepSounds[stepSoundIndex];
        steps.Play();

        stepSoundIndex++;
        if(stepSoundIndex == stepSounds.Length) stepSoundIndex = 0;
    }

    public void PlaySableSlash()
    {
        sableSlash.Play();
    }

    public void PlaySableHitOnBody()
    {
        sableHitOnBody.clip = swordHits[swordHitIndex];
        sableHitOnBody.Play();

        swordHitIndex++;
        if(swordHitIndex == swordHits.Length) swordHitIndex = 0;
    }

    public void PlaySableHitOnSable()
    {
        sableHitOnSable.clip = swordClashes[swordClashIndex];
        sableHitOnSable.Play();

        swordClashIndex++;
        if(swordClashIndex == swordClashes.Length) swordClashIndex = 0;
    }
}
