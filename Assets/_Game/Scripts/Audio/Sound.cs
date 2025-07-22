
using UnityEngine;
using System;

[Serializable]
public class Sound
{
    public enum Name
    {
        BackgroundMusic,
        Sound_Click,
        Sound_Erase,
        Sound_Win,
        Sound_Lose,
        Sound_RightNumber,
        Sound_WrongNumber
    }

    public Name name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1;

    [HideInInspector]
    public AudioSource source;
    public bool loop = false;

}