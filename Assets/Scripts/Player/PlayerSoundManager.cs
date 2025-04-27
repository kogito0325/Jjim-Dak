using System.Collections.Generic;
using UnityEngine;
public enum PlayerSoundState
{
    ATTACK,
    ATTACK2,
    Hit,
    Counter
}
public class PlayerSoundManager
{
    private AudioClip[] audioClips;
    private AudioSource audioSource;

    private Dictionary<string, PlayerSoundState> playerSoundNames = new Dictionary<string, PlayerSoundState>()
    {
        { "Attack", PlayerSoundState.ATTACK },
        { "Attack2", PlayerSoundState.ATTACK2 },
        { "Hit", PlayerSoundState.Hit },
        { "Counter", PlayerSoundState.Counter }
    };

    public PlayerSoundManager(AudioClip[] audioClips, AudioSource audioSource)
    {
        this.audioClips = audioClips;
        this.audioSource = audioSource;
    }

    public void Play(string state)
    {
        audioSource.PlayOneShot(audioClips[(int)playerSoundNames[state]]);
    }
}
