using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundData", menuName = "ScriptableObjects/PlayerSoundData")]
public class PlayerSoundData : ScriptableObject
{
    public enum AudioType
    {
        ATTACK,
        ATTACK2,
        Hit,
        Counter,
        PlayerHit,
        Jump,
        FootStep,
        Heal,
        Dash
    }

    public AudioClip[] audioClips;
}