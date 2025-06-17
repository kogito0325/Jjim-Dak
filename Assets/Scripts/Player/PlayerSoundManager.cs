using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager
{
    private AudioSource audioSource;
    private PlayerSoundData soundData;

    public PlayerSoundManager(PlayerSoundData soundData, AudioSource audioSource)
    {
        this.soundData = soundData;
        this.audioSource = audioSource;
    }

    public void Play(PlayerSoundData.AudioType audio)
    {
        audioSource.PlayOneShot(soundData.audioClips[(int)audio]);
    }
}
