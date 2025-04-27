using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum Dir { Right, Left}


public class BossScript : MonoBehaviour
{
    public int hp;
    public int maxHp;
    public int power;
    public Dir lookDir;
    public float actionTerm;
    public float dashSpeed;
    public float jumpPower;
    public int[] phase;

    float actionBreakTime;
    int patternCount;
    int phaseIdx;
    int atkCount;
    bool isInPattern;
    bool isOnWall;
    bool isJumping;

    int actionSpeed;

    public GameObject attackRange;
    public GameObject jumpAttackRange;
    public GameObject shockPrefab;
    public Transform shockSpot;
    public Image hpBar;
    public SpriteRenderer backGround;
    public Material WhiteFlashMaterial;

    Material originMaterial;

    Animator animator;
    Rigidbody2D rigid;
    Transform target;
    SpriteRenderer sprRend;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
        target = GameObject.FindWithTag("Player").transform;

        actionSpeed = 1;

        originMaterial = sprRend.material;

        isJumping = false;
        isInPattern = false;
        atkCount = 0;
        phaseIdx = 0;
        lookDir = Dir.Left;
        actionBreakTime = actionTerm;
        patternCount = 0;

        hp = maxHp;
        UpdateHpBar();
    }

    private void Update()
    {
        if (hp <= 0) Die();
        actionBreakTime -= Time.deltaTime;
        if (actionBreakTime > 0) return;

        if (isInPattern) return;
        isInPattern = true;

        LockOn();

        int patternIdx = Random.Range(0, 3);

        switch (patternIdx)
        {
            case 0:
                Dash();
                break;
            case 1:
                Jump();
                break;
            case 2:
                Smash();
                break;
            default:
                break;
        }
    }

    private void Dash()
    {
        patternCount++;
        animator.SetInteger("dashState", patternCount);
        switch (patternCount)
        {
            case 2:
                rigid.linearVelocityX = lookDir == Dir.Left ? -dashSpeed * actionSpeed : dashSpeed * actionSpeed;
                attackRange.SetActive(true);
                break;
            case 3:
                rigid.linearVelocityX = 0;
                attackRange.SetActive(false);
                patternCount = 0;
                EndPattern();
                break;
            default:
                break;
        }
    }

    private void Jump()
    {
        patternCount++;
        animator.SetInteger("jumpState", patternCount);
        switch (patternCount)
        {
            case 2:
                rigid.linearVelocityX = (target.position.x - transform.position.x) * actionSpeed;
                rigid.linearVelocityY = jumpPower;
                isJumping = true;
                break;
            case 3:
                rigid.linearVelocityX = 0;
                patternCount = 0;
                EndPattern();
                break;
            default:
                break;
        }
    }

    private void Smash()
    {
        patternCount++;
        animator.SetInteger("smashState", patternCount);
        switch (patternCount)
        {
            case 2:
                LockOn();
                break;
            case 3:
                patternCount = 0;
                EndPattern();
                break;
            default:
                break;
        }
    }

    public void ThrowShock()
    {
        shockPrefab.GetComponent<ShockScript>().direction = lookDir == Dir.Left ? -1 : 1;
        GameObject shock = Instantiate(shockPrefab, shockSpot.position, Quaternion.identity);
    }

    private void RecoveryAttack()
    {
        patternCount++;
        animator.SetInteger("recoveryState", patternCount);
        switch (patternCount)
        {
            case 1:
                LockOn();
                break;
            case 2:
                StartCoroutine(RecoveryDash());
                break;
            case 3:
            case 4:
                LockOn();
                break;
            case 5:
                patternCount = 0;
                break;
            default:
                break;
        }
    }

    private IEnumerator RecoveryDash()
    {
        attackRange.SetActive(true);
        isOnWall = false;

        while (!isOnWall)
        {
            rigid.linearVelocityX = lookDir == Dir.Left ? -dashSpeed * actionSpeed : dashSpeed * actionSpeed;
            yield return null;
        }
        attackRange.SetActive(false);
        RecoveryAttack();
    }

    private void LockOn()
    {
        if (target.position.x < transform.position.x)
        {
            lookDir = Dir.Left;
            UpdateLookDireciton();
        }
        else
        {
            lookDir = Dir.Right;
            UpdateLookDireciton();
        }
    }
    private void UpdateLookDireciton()
    {
        transform.rotation = Quaternion.Euler(Vector3.up * (lookDir == Dir.Left ? 0 : 180));
    }
    private void Die()
    {
        StartCoroutine(DeadEffect());
        actionSpeed = 1;
        animator.speed = actionSpeed;

        StopPattern();

        animator.Play("Die");
        gameObject.layer = LayerMask.NameToLayer("Dead");

        GetComponent<BossScript>().enabled = false;
    }

    private IEnumerator DeadEffect()
    {
        FindAnyObjectByType<CameraScript>().EffectCamera();

        Time.timeScale = 0.01f;
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
    }

    public void TakeDamage(int damage = 1)
    {
        if (hp <= 0) return;
        hp -= damage;
        UpdateHpBar();

        atkCount++;

        sprRend.material = WhiteFlashMaterial;
        Invoke("BlinkOut", 0.1f);

        if (hp <= 0) Die();
        else if (hp <= phase[phaseIdx])
        {
            phaseIdx++;
            isInPattern = true;
            StartCoroutine(Groggy());
        }
    }
    private void BlinkOut()
    {
        sprRend.material = originMaterial;
    }
    private void UpdateHpBar()
    {
        hpBar.fillAmount = (float)hp / maxHp;
        
    }

    private IEnumerator Groggy()
    {
        StopPattern();
        animator.Play("Groggy1");
        atkCount = 0;
        float groggyTimer = 5f;
        while (groggyTimer > 0 && atkCount < 5)
        {
            groggyTimer -= Time.deltaTime;

            yield return null;
        }
        animator.Play("Groggy3");
        if (phaseIdx == 2) StartCoroutine(FadeOut(backGround, 3f));
        //LevelUp();
    }

    private void LevelUp()
    {
        animator.speed = ++actionSpeed;
        actionTerm -= 1;
        rigid.gravityScale = 5 * actionSpeed;
    }

    private void StopPattern()
    {
        rigid.linearVelocity = Vector2.zero;
        patternCount = 0;

        attackRange.SetActive(false);
        jumpAttackRange.SetActive(false);
        
        animator.SetInteger("dashState", patternCount);
        animator.SetInteger("jumpState", patternCount);
        animator.SetInteger("smashState", patternCount);
        animator.SetInteger("recoveryState", patternCount);
    }

    private IEnumerator FadeOut(SpriteRenderer sprRend, float timer)
    {
        float maxTimer = timer;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0, maxTimer);
            sprRend.color = new Color(1f, 1f, 1f, timer / maxTimer);
            yield return null;
        }
    }

    private void EndPattern()
    {
        actionBreakTime = actionTerm;
        isInPattern = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerAtk"))
        {
            TakeDamage();
            collision.GetComponentInParent<PlayerMachine>()
                .playerStemina.HealEnergy(
                collision.GetComponentInParent<PlayerMachine>().playerData.attackHealEnergyAmount);
            collision.GetComponent<Collider2D>().enabled = false;
            FindAnyObjectByType<CameraScript>().ShakeLittleCamera(0.2f);
            collision.GetComponentInParent<PlayerMachine>().playerSoundManager.Play("Hit");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            isOnWall = true;
        }

        if (collision.collider.CompareTag("Ground"))
        {
            if (isJumping)
            {
                isJumping = false;
                if (isInPattern && patternCount == 2)
                {
                    Jump();
                }
            }
        }
    }
}
