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
        CreateAudioSourceBackground();
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

    #region Sound Effect Functions
    public void PlayClickSound()
    {
        PlayEffect(Sound.Name.Sound_Click);
    }
    public void PlayEraseSound()
    {
        PlayEffect(Sound.Name.Sound_Erase);
    }
    public void PlayUndoSound()
    {
        PlayEffect(Sound.Name.Sound_Erase); // Sử dụng sound erase cho undo
    }
    public void PlayWinSound()
    {
        PlayEffect(Sound.Name.Sound_Win);
    }

    public void PlayLoseSound()
    {
        PlayEffect(Sound.Name.Sound_Lose);
    }

    public void PlayRightNumberSound()
    {
        PlayEffect(Sound.Name.Sound_RightNumber);
    }

    public void PlayWrongNumberSound()
    {
        PlayEffect(Sound.Name.Sound_WrongNumber);
    }

    #endregion

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
        if (backgroundMusic.source == null)
        {
            backgroundMusic.source = gameObject.AddComponent<AudioSource>();
        }
        backgroundMusic.source.clip = backgroundMusic.clip;
        backgroundMusic.source.loop = backgroundMusic.loop;
        backgroundMusic.source.volume = backgroundMusic.volume;
        backgroundMusic.source.playOnAwake = false;
    }

    private void CreateAudioSource(Sound[] sounds)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.source == null)
            {
                GameObject soundObject = new GameObject($"AudioSource_{sound.name}");
                soundObject.transform.SetParent(transform);
                sound.source = soundObject.AddComponent<AudioSource>();
            }

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
            sound.source.playOnAwake = false;
        }
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
    }
}