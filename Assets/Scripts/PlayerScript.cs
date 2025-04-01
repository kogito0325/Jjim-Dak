using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int hp;
    [SerializeField] private float speed;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float jumpPower;
    [SerializeField] private float dashPower;


    [Header("Utilities")]
    [SerializeField] private float dashEnergy;
    [SerializeField] private float dashCoolTime;
    [SerializeField] private float dashDurationTime;
    [SerializeField] private float guardEnergy;
    [SerializeField] private float guardDurationTime;
    [SerializeField] private float energyHealAmount;
    [SerializeField] private float healTime;
    [SerializeField] private float attackDurationTime;

    public Image[] hearts;
    public Image energyBar;
    public GameObject guard;
    public GameObject counterPrefab;
    public GameObject attackRangeRight;
    public GameObject attackRangeLeft;
    public SpriteRenderer healEffect;


    private float energy;
    private float inputX;
    private float xVelocity;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isDashingPossible;
    [SerializeField] private bool isHealing;
    [SerializeField] private bool isGuarding;
    [SerializeField] private bool isAlive;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;
    Animator animator;


    private void Start()
    {
        //Time.timeScale = 0.1f;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        energy = maxEnergy;

        InitializeFlags();

        UpdateHearts();

        StartCoroutine(IdleState());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
        
        if (Input.GetKeyDown(KeyCode.Mouse1) && isAlive) Guard();

        UpdateEnergyBar();

        if (Input.GetKeyDown(KeyCode.Alpha4) && isAlive) TakeDamage();
        animator.SetFloat("yVelocity", rigidBody.linearVelocityY);
        healEffect.flipX = spriteRenderer.flipX;
    }

    private void InitializeFlags()
    {
        isDashingPossible = true;
        isGrounded = true;
        isHealing = false;
        isGuarding = false;
        isAlive = true;
    }

    private IEnumerator IdleState()
    {
        animator.SetBool("isMoving", false);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartCoroutine(AttackState());
                break;
            }
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                StartCoroutine(MoveState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                StartCoroutine(JumpState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && isDashingPossible && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.W) && !isHealing && hp < 5)
            {
                StartCoroutine(HealState());
                break;
            }
            yield return null;
        }
    }

    private void MoveX()
    {
        rigidBody.linearVelocityX = xVelocity;
    }

    private IEnumerator MoveState()
    {
        while (true)
        {
            if(isGrounded) animator.SetBool("isMoving", true);
            inputX = Input.GetAxisRaw("Horizontal");
            xVelocity = inputX * speed;

            if (xVelocity > 0) spriteRenderer.flipX = false;
            else if (xVelocity < 0) spriteRenderer.flipX = true;
            MoveX();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                StartCoroutine(JumpState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && isDashingPossible && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
                break;
            }
            if (inputX == 0)
            {
                StartCoroutine(IdleState());
                break;
            }
            yield return null;
        }
    }

    private IEnumerator JumpState()
    {
        isGrounded = false;
        rigidBody.linearVelocityY = jumpPower;
        animator.SetBool("isJumping", true);
        animator.SetBool("isMoving", false);
        while (!isGrounded)
        {
            if (Input.GetAxisRaw("Horizontal") != 0)
            {
                StartCoroutine(MoveState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && isDashingPossible && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
            }
            yield return null;
        }
        if (isGrounded)
        {
            StartCoroutine(IdleState());
        }
    }

    private IEnumerator AttackState()
    {
        //animator.SetBool("isAttackStarted", true);
        GameObject tempObj = spriteRenderer.flipX ? attackRangeLeft : attackRangeRight;
        tempObj.SetActive(true);

        float attackTimer = attackDurationTime;
        bool isAttackContinuing = false;
        yield return null;
        while (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                isAttackContinuing = true;
            }
            yield return null;
        }
        if (isAttackContinuing)
        {
            //animator.SetBool("isAttacking", true);
            tempObj.SetActive(true);
        }
        yield return new WaitForSeconds(attackDurationTime);
        //animator.SetBool("isAttackStarted", false);
        //animator.SetBool("isAttacking", false);
        tempObj.SetActive(false);
        StartCoroutine(IdleState());
    }


    private IEnumerator DashState()
    {
        isDashingPossible = false;
        if (inputX == 0) inputX = spriteRenderer.flipX ? -1 : 1;
        energy -= dashEnergy;
        xVelocity = inputX * dashPower;
        MoveX();
        yield return new WaitForSeconds(dashDurationTime);
        StartCoroutine(IdleState());
        yield return new WaitForSeconds(dashCoolTime);
        isDashingPossible = true;
    }

    private IEnumerator HealState()
    {
        isHealing = true;
        float healTimer = healTime;

        animator.SetBool("isHealing", true);

        while (isHealing && healTimer > 0)
        {
            if (Input.GetKeyUp(KeyCode.W)) isHealing = false;
            else if(Input.GetKeyDown(KeyCode.LeftShift) && isDashingPossible && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
                break;
            }

            healTimer -= Time.deltaTime;
            yield return null;
        }

        if (healTimer <= 0)
        {
            hp += 1;
            UpdateHearts();
            Debug.Log("Healing Success");
        }
        else
        {
            Debug.Log("Healing Cancled");
        }
        StartCoroutine(IdleState());
        animator.SetBool("isHealing", false);
        isHealing = false;
    }

    private void Guard()
    {
        if (energy >= guardEnergy && !isGuarding)
        {
            energy -= guardEnergy;
            StopAllCoroutines();
            StartCoroutine(GuardSequence());
        }
    }

    private IEnumerator GuardSequence()
    {
        isGuarding = true;
        guard.SetActive(true);
        int oldHp = hp;
        float guardTimer = guardDurationTime;
        while (guardTimer > 0)
        {
            guardTimer -= Time.deltaTime;
            if (hp < oldHp)
            {
                CounterAttack();
                hp += 1;
                energy += 10f;
                break;
            }
            yield return null;
        }
        if (guardTimer <= 0)
        {
            StartCoroutine(IdleState());
        }
        isGuarding = false;
        guard.SetActive(false);
    }

    private void CounterAttack()
    {
        Instantiate(counterPrefab, transform.position, Quaternion.identity);
        InitializeFlags();
        StartCoroutine(IdleState());
    }

    private IEnumerator DeadState()
    {
        Debug.Log("Dead");
        isAlive = false;
        animator.SetBool("isDead", true);
        yield return null;
        animator.SetBool("isDead", false);
    }

    private void UpdateHearts()
    {
        foreach (Image sprite in hearts) sprite.gameObject.SetActive(false);
        for (int i = 0; i < hp; i++) hearts[i].gameObject.SetActive(true);
    }

    private void UpdateEnergyBar()
    {
        if (energy <= maxEnergy) energy += energyHealAmount * Time.deltaTime;
        energyBar.fillAmount = energy / maxEnergy;
    }


    public void TakeDamage(int damage = 1)
    {
        hp -= damage;
        UpdateHearts();
        if (hp <= 0 && isAlive)
        {
            StopAllCoroutines();
            StartCoroutine(DeadState());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
        else if (collision.gameObject.CompareTag("Boss"))
        {
            TakeDamage();
        }
        else if (collision.gameObject.CompareTag("BossAtk"))
        {
            collision.gameObject.SetActive(false);
            TakeDamage();
        }
        
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boss")) return;
    }
}
