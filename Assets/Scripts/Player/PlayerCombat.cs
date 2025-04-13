using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject attackRange;

    private PlayerAniManager playerAni;
    private PlayerData playerData;
    private PlayerHealth playerHealth;
    private PlayerStemina playerStemina;
    private Rigidbody2D rigid;

    private float atkTimer;
    private float guardTimer;
    private float nextAtkTime;

    public bool isHealing { get; private set; }
    public bool isGuarding { get; private set; }

    private void Start()
    {
        playerData = GetComponent<PlayerScript>().playerData;
        playerHealth = GetComponent<PlayerHealth>();
        playerAni = GetComponent<PlayerAniManager>();
        playerStemina = GetComponent<PlayerStemina>();
        rigid = GetComponent<Rigidbody2D>();
        nextAtkTime = 0f;
    }

    private void Update()
    {
        if (atkTimer > 0)
            atkTimer -= Time.deltaTime;
        else if (attackRange.activeSelf)
            attackRange.GetComponent<Collider2D>().enabled = false;

        if (nextAtkTime > 0)
            nextAtkTime -= Time.deltaTime;

        if (guardTimer > 0)
            guardTimer -= Time.deltaTime;
        else
            isGuarding = false;

        Debug.Log(nextAtkTime);
    }

    public void Attack()
    {
        if (atkTimer <= 0 && !isHealing && rigid.linearVelocity == Vector2.zero)
        {
            atkTimer = playerData.attackDurationTime;
            attackRange.GetComponent<Collider2D>().enabled = true;

            if (nextAtkTime <= 0)
            {
                playerAni.Play(PlayerAnimState.ATTACK);
                nextAtkTime = atkTimer + 0.3f;
            }
            else
            {
                playerAni.Play(PlayerAnimState.ATTACK2);
                nextAtkTime = 0f;
            }
        }
    }

    public void Guard()
    {
        if (isHealing || isGuarding || rigid.linearVelocity != Vector2.zero) return;
        Debug.Log("Guard");
        playerStemina.SpendEnergy(playerData.guardEnergy);
        isGuarding = true;   
        guardTimer = playerData.attackDurationTime;
        playerAni.Play(PlayerAnimState.GUARD);
    }

    public void CounterAttack()
    {
        Debug.Log("Counter");
        playerStemina.HealEnergy(playerData.guardHealEnergyAmount);
        FindAnyObjectByType<BossScript>().TakeDamage(playerData.counterDamage);
        isGuarding = false;
        playerHealth.TakeDamage(0);
        playerAni.Play(PlayerAnimState.COUNTER);
    }

    public void Heal()
    {
        if (playerHealth.hp < playerData.maxHp)
            StartCoroutine(HealState());
    }

    private IEnumerator HealState()
    {
        isHealing = true;
        float healTimer = playerData.healTime;

        playerAni.animator.SetBool("isHealing", isHealing);
        playerAni.Play(PlayerAnimState.HEAL);

        while (isHealing && healTimer > 0)
        {
            if (Input.GetKeyUp(KeyCode.W) || rigid.linearVelocity != Vector2.zero)
            {
                isHealing = false;
                break;
            }
            healTimer -= Time.deltaTime;
            yield return null;
        }

        if (healTimer <= 0)
        {
            playerStemina.SpendEnergy(playerData.healEnergy);
            playerHealth.TakeDamage(-playerData.healAmount);
            Debug.Log("Healing Success");
        }
        else
        {
            Debug.Log("Healing Cancled");
        }
        isHealing = false;
        playerAni.animator.SetBool("isHealing", isHealing);
    }
}