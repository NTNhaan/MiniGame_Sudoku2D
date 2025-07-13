using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;
public class AudioController : Singleton<AudioController>
{
    public Sound backgroundMusic;
    public Sound[] lstSoundEffect;

    private void Start()
    {
        CreateAudioSource(lstSoundEffect);
        // SoundCheck(); có DBController để set giá trị gốc của các biến sử dụng (sound and music)
        PlayBackgroundMusic();
    }
    private void PlayBackgroundMusic()
    {
        backgroundMusic.source.Play();
    }
    public void PlaySoundEffect()
    {
        PlayEffect(Sound.Name.Sound_Click);
    }

    public void SetVolumeMusic(bool status)
    {
        backgroundMusic.volume = status ? 1 : 0;
        backgroundMusic.source.volume = backgroundMusic.volume;
    }
    public void SetVolumeSound(bool status)
    {
        foreach (Sound sound in lstSoundEffect)
        {
            sound.volume = status ? 1 : 0;
            sound.source.volume = sound.volume;
        }
    }

    public void SoundCheck(bool music, bool effect)
    {
        SetVolumeMusic(music);
        SetVolumeSound(effect);
    }
    private void CreateAudioSourceBackground()
    {
        backgroundMusic.source.clip = backgroundMusic.clip;
        backgroundMusic.source.loop = backgroundMusic.loop;
    }

    private void CreateAudioSource(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
        }
        CreateAudioSourceBackground();
    }
    public void PlayEffect(Sound.Name name)
    {
        foreach (var effect in lstSoundEffect)
        {
            if (effect.name == name)
            {
                effect.source.Play();
                return;
            }
        }
        Debug.LogError("Unable to play effect " + name);
    }
    public void StopEffect(Sound.Name name)
    {
        foreach (var effect in lstSoundEffect)
        {
            if (effect.name == name)
            {
                effect.source.Stop();
                return;
            }
        }
        Debug.LogError("Unable to play effect " + name);
    }
}