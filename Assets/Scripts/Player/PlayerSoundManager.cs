using System.Collections.Generic;
using UnityEngine;

public enum PlayerSoundState
{
    ATTACK,
    ATTACK2,
    Hit,
    Counter
}

public class PlayerSoundManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    private Dictionary<string, PlayerSoundState> playerSoundNames = new Dictionary<string, PlayerSoundState>()
    {
        { "Attack", PlayerSoundState.ATTACK },
        { "Attack2", PlayerSoundState.ATTACK2 },
        { "Hit", PlayerSoundState.Hit },
        { "Counter", PlayerSoundState.Counter }
    };


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Play(string state)
    {
        audioSource.PlayOneShot(audioClips[(int)playerSoundNames[state]]);
    }
}
