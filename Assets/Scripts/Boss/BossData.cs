using UnityEngine;

[CreateAssetMenu(fileName = "BossData", menuName = "ScriptableObjects/BossData")]
public class BossData : ScriptableObject
{
    [Header("Health")]
    public int maxHp;
    public int[] phase;

    [Header("Movement")]
    public float jumpPower;
    public float moveSpeed;

    [Header("Combat")]
    public int power;
    public float chargeSpeed;
    public float chargeDurationTime;
    public float swingDistance;

    [Header("Utility")]
    public float actionTerm;
    public GameObject afterFx;
}