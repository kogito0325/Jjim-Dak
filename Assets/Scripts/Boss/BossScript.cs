using System.Collections;
using UnityEngine;


public enum Dir { Right = 1, Left = -1}


public class BossScript : MonoBehaviour
{
    [SerializeField] BossData bossData;

    public Dir lookDir;

    private float actionBreakTime;

    public int hp { get; private set; }
    private int phaseIdx;
    private int atkCount;
    
    private bool isInPattern;
    private bool isJumping;

    public Collider2D attackRange;
    public GameObject shockPrefab;
    public Transform shockSpot;
    public GameObject jumpEffectPrefab;
    public Transform jumpEffectSpot;

    public GameObject groggyEffect;
    public GameObject groundEffect;

    public SpriteRenderer backGround;
    public Material WhiteFlashMaterial;

    Material originMaterial;

    private BossAnimControl animControl;
    Rigidbody2D rigid;
    Transform target;
    SpriteRenderer sprRend;


    private void Awake()
    {
        animControl = new BossAnimControl(GetComponent<Animator>());
        rigid = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();
        target = GameObject.FindWithTag("Player").transform;

        originMaterial = sprRend.material;

        isJumping = false;
        isInPattern = false;
        phaseIdx = 0;
        atkCount = 0;
        lookDir = Dir.Left;
        actionBreakTime = bossData.actionTerm;

        hp = bossData.maxHp;
    }

    private void Update()
    {
        if (hp <= 0) Die();
        
        if (isInPattern) return;
        
        actionBreakTime -= Time.deltaTime;
        animControl.Play(BossAnimState.Idle);
        
        if (actionBreakTime > 0) return;
        
        isInPattern = true;

        LockOn();

        int patternIdx = Random.Range(0, 4);

        switch (patternIdx)
        {
            case 0:
                StartCoroutine(Chase());
                break;
            case 1:
                StartCoroutine(Charge());
                break;
            case 2:
                JumpReady();
                break;
            case 3:
                Smash();
                break;
            default:
                break;
        }
    }


    private IEnumerator Chase()
    {
        LockOn();
        animControl.Play(BossAnimState.Chase);
        while (true)
        {
            float distance2target = Mathf.Abs(target.position.x - transform.position.x);
            if (distance2target <= bossData.swingDistance) break;
            LockOn();
            rigid.linearVelocityX = bossData.moveSpeed * (int)lookDir;
            yield return null;
        }
        Swing();
    }

    private void Swing()
    {
        LockOn();
        rigid.linearVelocity = Vector2.zero;
        animControl.Play(BossAnimState.Swing);
    }


    private IEnumerator Charge()
    {
        LockOn();
        animControl.Play(BossAnimState.Charge);
        yield return new WaitForSeconds(0.5f);
        rigid.linearVelocityX = lookDir == Dir.Left ? -bossData.chargeSpeed : bossData.chargeSpeed;
        yield return new WaitForSeconds(bossData.chargeDurationTime);
        rigid.linearVelocity = Vector2.zero;
    }

    public void MakeInstance()
    {
        Destroy(Instantiate(bossData.afterFx, transform.position, transform.rotation), bossData.chargeDurationTime/2f);
    }

    private void JumpReady()
    {
        LockOn();
        rigid.linearVelocity = Vector2.zero;
        animControl.Play(BossAnimState.JumpReady);
    }

    private void Jump()
    {
        rigid.linearVelocityX = target.position.x - transform.position.x;
        rigid.linearVelocityY = bossData.jumpPower;
        isJumping = true;
    }

    private void JumpSmash()
    {
        rigid.linearVelocityX = 0;
        animControl.Play(BossAnimState.Ground);
    }

    private void JumpEffect()
    {
        Destroy(Instantiate(jumpEffectPrefab, jumpEffectSpot.position, Quaternion.identity), 1f);
    }

    private void Smash()
    {
        LockOn();
        animControl.Play(BossAnimState.Smash);
    }

    private void SmashEffect()
    {
        Instantiate(groundEffect, shockSpot.position, Quaternion.identity);
        FindAnyObjectByType<CameraScript>().ShakeCamera();
    }

    public void ThrowShock()
    {
        ShockScript shockWave = shockPrefab.GetComponent<ShockScript>();
        shockWave.direction = lookDir == Dir.Left ? -1 : 1;
        shockWave.transform.rotation = Quaternion.Euler(Vector2.up * (shockWave.direction + 1) * 90f);
        Instantiate(shockPrefab, shockSpot.position, shockPrefab.transform.rotation);
    }

    private void LockOn()
    {
        if (target.position.x < transform.position.x)
            lookDir = Dir.Left;
        else if (target.position.x > transform.position.x)
            lookDir = Dir.Right;

        transform.rotation = Quaternion.Euler(Vector3.up * ((int)lookDir + 1) * 90);
    }

    private void Die()
    {
        StopPattern();
        StartCoroutine(DeadEffect());

        animControl.Play(BossAnimState.Die);
        gameObject.layer = LayerMask.NameToLayer("Dead");

        enabled = false;
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

        atkCount++;

        sprRend.material = WhiteFlashMaterial;
        Invoke("BlinkOut", 0.1f);

        if (hp <= 0) Die();
        else if (hp <= bossData.phase[phaseIdx])
        {
            phaseIdx++;
            if (phaseIdx == 2)
                bossData.actionTerm = 1f;
            StopPattern();
            StartCoroutine(Groggy());
        }
    }
    private void BlinkOut()
    {
        sprRend.material = originMaterial;
    }

    private IEnumerator Groggy()
    {
        isInPattern = true;
        Instantiate(groggyEffect, transform.position + Vector3.up, Quaternion.identity);
        animControl.Play(BossAnimState.GroggyStart);
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        float groggyTimer = 5f;
        atkCount = 0;
        while (groggyTimer > 0 && atkCount < 5)
        {
            groggyTimer -= Time.deltaTime;

            yield return null;
        }
        animControl.Play(BossAnimState.GroggyEnd);
        if (phaseIdx == 2) StartCoroutine(FadeOut(backGround, 3f));
    }

    private void StopPattern()
    {
        StopAllCoroutines();
        rigid.linearVelocity = Vector2.zero;
        attackRange.enabled = false;
        isJumping = false;
        isInPattern = false;
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
        actionBreakTime = bossData.actionTerm;
        isInPattern = false;
        LockOn();
    }

    private void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }

    private void JumpProduction()
    {
        GetComponent<Rigidbody2D>().linearVelocityX = -15f;
    }

    private void FadeOut()
    {
        SceneControl sceneControl = FindAnyObjectByType<SceneControl>();
        sceneControl.GetComponent<Animator>().Play("FadeOut");
        sceneControl.LoadScene("EndingScene", 5f);
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

            Instantiate(groundEffect, transform.position + Vector3.up, Quaternion.identity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (isJumping && isInPattern)
            {
                isJumping = false;
                JumpSmash();
            }
        }
    }
}
