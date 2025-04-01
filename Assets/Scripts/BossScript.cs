using System.Collections;
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

    float actionBreakTime;
    int dashCount;

    public GameObject attackRangeLeft;
    public GameObject attackRangeRight;
    public Image hpBar;

    Animator animator;
    Rigidbody2D rigid;
    Transform target;
    SpriteRenderer sprRend;


    private void Start()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprRend = GetComponent<SpriteRenderer>();

        lookDir = Dir.Left;
        actionBreakTime = actionTerm;
        dashCount = 0;
        hp = maxHp;
        UpdateHpBar();
    }

    private void Update()
    {
        if (hp <= 0) Die();

        actionBreakTime -= Time.deltaTime;
        if (actionBreakTime > 0) return;

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
                if (lookDir == Dir.Left)
                {
                    rigid.linearVelocityX = -dashSpeed;
                    attackRangeLeft.SetActive(true);
                }
                else
                {
                    rigid.linearVelocityX = dashSpeed;
                    attackRangeRight.SetActive(true);
                }
                break;
            case 3:
                rigid.linearVelocityX = 0;
                attackRangeLeft.SetActive(false);
                attackRangeRight.SetActive(false);
                dashCount = 0;
                LockOn();
                actionBreakTime = actionTerm;
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
        sprRend.flipX = lookDir == Dir.Right;
    }
    private void Die()
    {
        sprRend.color = Color.gray;
        GetComponent<BossScript>().enabled = false;
    }
    public void TakeDamage(int damage = 1)
    {
        hp -= damage;
        UpdateHpBar();
        if (hp <= 0) Die();
    }
    private void UpdateHpBar()
    {
        hpBar.fillAmount = (float)hp / maxHp;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("inTrigger");
        if (collision.gameObject.CompareTag("PlayerAtk"))
        {
            Debug.Log("BossHit");
            TakeDamage();
            collision.gameObject.SetActive(false);
        }
    }
}
