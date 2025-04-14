using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 1)]
public class PlayerData : ScriptableObject
{
    [Header("Player Stats")]
    public int maxHp;
    public float maxEnergy;
    public float energyHealAmount;
    public float speed;
    public float jumpPower;

    [Header("Attack Stats")]
    public float attackDurationTime;
    public float attackHealEnergyAmount;

    [Header("Dash")]
    public float dashSpeed;
    public float dashEnergy;
    public float dashDurationTime;
    public float dashCoolTime;

    [Header("Heal")]
    public int healAmount;
    public float healEnergy;
    public float healTime;

    [Header("Guard")]
    public float guardEnergy;
    public float guardDurationTime;
    public float guardHealEnergyAmount;
    public float guardCoolTime;
    public int counterDamage;

    [Header("Utility")]
    public float protectTime;
}