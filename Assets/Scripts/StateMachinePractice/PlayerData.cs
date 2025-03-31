using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Player Stats")]
    [field: SerializeField] public int maxHp { get; private set; }
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public float speed { get; private set; }
    [field: SerializeField] public float maxEnergy { get; private set; }
    [field: SerializeField] public float jumpPower { get; private set; }
    [field: SerializeField] public float dashPower {get; private set; }


    [Header("Utilities")]
    [field: SerializeField] public float dashEnergy { get; private set; }
    [field: SerializeField] public float dashCoolTime { get; private set; }
    [field: SerializeField] public float dashDurationTime { get; private set; }
    [field: SerializeField] public float guardEnergy { get; private set; }
    [field: SerializeField] public float guardDurationTime { get; private set; }
    [field: SerializeField] public float energyHealAmount { get; private set; }
    [field: SerializeField] public float healTime { get; private set; }
    [field: SerializeField] public float attackDurationTime { get; private set; }


    public GameObject guard;
    public GameObject counterPrefab;
    public GameObject attackRange;


    public float energy { get; set; }
    public float inputX { get; private set; }
    public float xVelocity { get; private set; }

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isDashingPossible;
    [SerializeField] private bool isHealing;
    [SerializeField] private bool isGuarding;

    public void InitializeFlags()
    {
        isDashingPossible = true;
        isGrounded = true;
        isHealing = false;
        isGuarding = false;
    }
}
