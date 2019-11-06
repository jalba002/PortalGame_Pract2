using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioManager
{
    public static void PlayClip(AudioSource l_AudioSource, AudioClip l_Clip)
    {
        if (l_AudioSource == null) return;
        try
        {
            l_AudioSource.clip = l_Clip;
            l_AudioSource.Play();
        }
        catch
        {
            Debug.LogWarning("Error when playing sound!");
        }
    }
}
