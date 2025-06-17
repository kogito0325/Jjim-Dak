using UnityEngine;

[CreateAssetMenu(fileName = "BossSoundData", menuName = "ScriptableObjects/BossSoundData")]
public class BossSoundData : ScriptableObject
{
    public enum AudioType
    {
        Appear,
        Swing,
        Move,
        Smash,
        Jump
    }

    public AudioClip[] audioClips;
}