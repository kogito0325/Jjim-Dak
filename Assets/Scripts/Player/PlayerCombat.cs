using System.Collections;
using UnityEngine;

public class PlayerCombat
{
    private PlayerMachine PlayerMachine;
    private PlayerAniManager playerAni;
    private PlayerData playerData;
    private PlayerHealth playerHealth;
    private PlayerStemina playerStemina;
    private PlayerSoundManager playerSound;

    private GameObject attackRange;
    private GameObject healObject;
    private GameObject guardObject;
    private Rigidbody2D rigid;

    public float atkTimer {get; set;}
    private float guardTimer;
    private float guardCoolTimer;
    private float nextAtkTime;

    public bool isHealing { get; private set; }
    public bool isGuarding { get; private set; }

    public PlayerCombat(PlayerMachine playerMachine, Rigidbody2D rigidbody)
    {
        PlayerMachine = playerMachine;
        playerData = playerMachine.playerData;
        playerHealth = playerMachine.playerHealth;
        playerAni = playerMachine.playerAniManager;
        playerStemina= playerMachine.playerStemina;
        playerSound = playerMachine.playerSoundManager;

        rigid = rigidbody;

        attackRange = playerMachine.attackRange;
        healObject = playerMachine.healObject;
        guardObject = playerMachine.guardObject;

        nextAtkTime = 0f;
        guardCoolTimer = 0f;
    }

    public void Update()
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

        if (guardCoolTimer > 0)
            guardCoolTimer -= Time.deltaTime;
    }

    public void Attack()
    {
        if (atkTimer <= 0 && !isHealing)
        {
            rigid.linearVelocityX /= 2f;
            atkTimer = playerData.attackDurationTime;
            attackRange.GetComponent<Collider2D>().enabled = true;

            if (nextAtkTime <= 0)
            {
                playerAni.Play(PlayerAnimState.ATTACK);
                nextAtkTime = atkTimer + 0.3f;
                playerSound.Play(PlayerSoundData.AudioType.ATTACK);
            }
            else
            {
                playerAni.Play(PlayerAnimState.ATTACK2);
                nextAtkTime = 0f;
                playerSound.Play(PlayerSoundData.AudioType.ATTACK2);
            }
        }
    }

    public void Guard()
    {
        if (isHealing || isGuarding) return;
        if (guardCoolTimer > 0) return;

        playerStemina.SpendEnergy(playerData.guardEnergy);
        isGuarding = true;
        guardTimer = playerData.attackDurationTime;
        guardCoolTimer = playerData.guardCoolTime;
        playerAni.Play(PlayerAnimState.GUARD);
        playerAni.Play(GuardAnimState.GUARD, guardObject);
    }

    public void CounterAttack(BossScript boss = null)
    {
        playerStemina.HealEnergy(playerData.guardHealEnergyAmount);
        if (boss)
            boss.TakeDamage(playerData.counterDamage);
        isGuarding = false;
        playerHealth.TakeDamage(0);
        playerAni.Play(PlayerAnimState.COUNTER);
        playerAni.Play(GuardAnimState.COUNTER, guardObject);
        playerSound.Play(PlayerSoundData.AudioType.Counter);
        Object.FindAnyObjectByType<CameraScript>().EffectCamera();
    }

    public void Heal()
    {
        if (playerHealth.hp < playerData.maxHp)
            PlayerMachine.StartCoroutine(HealState());
    }

    private IEnumerator HealState()
    {
        isHealing = true;
        float healTimer = playerData.healTime;

        playerAni.Play(HealAnimState.HEALSTART, healObject);

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
            playerAni.Play(HealAnimState.HEALEND, healObject);
            playerSound.Play(PlayerSoundData.AudioType.Heal);
            Debug.Log("Healing Success");
        }
        else
        {
            playerAni.Play(HealAnimState.IDLE, healObject);
            Debug.Log("Healing Cancled");
        }
        isHealing = false;
    }
}