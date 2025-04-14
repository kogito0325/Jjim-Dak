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
    public int[] phase;

    float actionBreakTime;
    int dashCount;
    int phaseIdx;
    int atkCount;
    bool isInPattern;

    public GameObject attackRange;
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

        originMaterial = sprRend.material;

        isInPattern = false;
        atkCount = 0;
        phaseIdx = 0;
        lookDir = Dir.Left;
        actionBreakTime = actionTerm;
        dashCount = 0;
        hp = maxHp;
        UpdateHpBar();
    }

    private void Update()
    {

        Debug.Log(sprRend.color.a);
        if (hp <= 0) Die();

        actionBreakTime -= Time.deltaTime;
        if (actionBreakTime > 0) return;

        if (isInPattern) return;
        isInPattern = true;

        if (dashCount == 0)
        {
            LockOn();
            Dash();
        }

    }

    private void Dash()
    {
        dashCount++;
        animator.SetInteger("dashState", dashCount);
        switch (dashCount)
        {
            case 2:
                rigid.linearVelocityX = lookDir == Dir.Left ? -dashSpeed : dashSpeed;
                attackRange.SetActive(true);
                break;
            case 3:
                rigid.linearVelocityX = 0;
                attackRange.SetActive(false);
                dashCount = 0;
                EndPattern();
                break;
            default:
                break;
        }
    }
    private void LockOn()
    {
        target = GameObject.FindWithTag("Player").transform;
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
        rigid.linearVelocity = Vector2.zero;
        animator.Play("Die");
        gameObject.layer = LayerMask.NameToLayer("Dead");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Dead"));
        attackRange.SetActive(false);
        GetComponent<BossScript>().enabled = false;
    }
    public void TakeDamage(int damage = 1)
    {
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
        rigid.linearVelocity = Vector2.zero;
        dashCount = 0;
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
            collision.GetComponentInParent<PlayerStemina>()
                .HealEnergy(
                collision.GetComponentInParent<PlayerScript>().playerData
                .attackHealEnergyAmount);
            collision.GetComponent<Collider2D>().enabled = false;
        }
    }
}
