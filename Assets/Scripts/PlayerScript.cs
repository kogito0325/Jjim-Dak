using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHp;
    [SerializeField] private float maxEnergy;
    [SerializeField] private float energyHealAmount;
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private int hp;
    private float energy;

    [Header("Attack Stats")]
    [SerializeField] private float attackDurationTime;
    [SerializeField] private float attackHealEnergyAmount;

    [Header("Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashEnergy;
    [SerializeField] private float dashDurationTime;
    [SerializeField] private float dashCoolTime;

    [Header("Heal")]
    [SerializeField] private int healAmount;
    [SerializeField] private float healEnergy;
    [SerializeField] private float healTime;

    [Header("Guard")]
    [SerializeField] private float guardEnergy;
    [SerializeField] private float guardDurationTime;

    public Image[] hearts;
    public Image[] energySprites;
    public GameObject guard;
    public GameObject counterPrefab;
    public GameObject attackRange;


    private float inputX;
    private float xVelocity;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isPossibleToDash;
    [SerializeField] private bool isHealing;
    [SerializeField] private bool isGuarding;
    [SerializeField] private bool isAlive;
    [SerializeField] private bool isAttacking;
    [SerializeField] private bool isDashing;

    Rigidbody2D rigidBody;
    Animator animator;
    SpriteRenderer sprRend;


    private void Start()
    {
        //Time.timeScale = 0.1f;
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprRend = GetComponent<SpriteRenderer>();

        energy = maxEnergy;
        hp = maxHp;

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

        if (isDashing) rigidBody.linearVelocityY = 0;
    }

    private void InitializeFlags()
    {
        isPossibleToDash = true;
        isGrounded = true;
        isHealing = false;
        isGuarding = false;
        isAlive = true;
        isDashing = false;
    }

    private IEnumerator IdleState()
    {
        animator.SetBool("isMoving", false);
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttacking)
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
            if (Input.GetKeyDown(KeyCode.LeftShift) && isPossibleToDash && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.W) && !isHealing && hp < maxHp && energy >= healEnergy)
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
        Debug.Log("Enter Move");
        while (true)
        {
            if(isGrounded) animator.SetBool("isMoving", true);
            inputX = Input.GetAxisRaw("Horizontal");
            xVelocity = inputX * speed;

            if (inputX < 0) transform.rotation = Quaternion.Euler(Vector3.up * 180f);
            else if (inputX > 0) transform.rotation = Quaternion.Euler(Vector3.zero);
            MoveX();

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                StartCoroutine(JumpState());
                break;
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && isPossibleToDash && energy >= dashEnergy)
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
            if (Input.GetKeyDown(KeyCode.LeftShift) && isPossibleToDash && energy >= dashEnergy)
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
        attackRange.SetActive(true);
        isAttacking = true;
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackDurationTime);
        attackRange.SetActive(false);
        isAttacking = false;
        StartCoroutine(IdleState());
    }


    private IEnumerator DashState()
    {
        Debug.Log("Enter Dash");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        animator.SetTrigger("isDashing");
        isDashing = true;
        isPossibleToDash = false;
        if (inputX == 0) inputX = transform.rotation.y == 0 ? 1 : -1;
        energy -= dashEnergy;
        xVelocity = inputX * dashSpeed;
        MoveX();
        rigidBody.linearVelocityY = 0f;
        float tempGravity = rigidBody.gravityScale;
        rigidBody.gravityScale = 0f;

        yield return new WaitForSeconds(dashDurationTime);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
        rigidBody.linearVelocityX = 0f;
        isDashing = false;
        rigidBody.gravityScale = tempGravity;
        StartCoroutine(IdleState());

        yield return new WaitForSeconds(dashCoolTime);
        isPossibleToDash = true;
    }

    private IEnumerator HealState()
    {
        isHealing = true;
        float healTimer = healTime;

        animator.SetBool("isHealing", true);

        while (isHealing && healTimer > 0)
        {
            if (Input.GetKeyUp(KeyCode.W)) isHealing = false;
            else if(Input.GetKeyDown(KeyCode.LeftShift) && isPossibleToDash && energy >= dashEnergy)
            {
                StartCoroutine(DashState());
                break;
            }

            healTimer -= Time.deltaTime;
            yield return null;
        }

        if (healTimer <= 0)
        {
            energy -= healEnergy;
            hp += healAmount;
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

    private void DeadState()
    {
        Debug.Log("Dead");
        isAlive = false;
        animator.SetTrigger("isDead");
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));

    }

    private void UpdateHearts()
    {
        foreach (Image sprite in hearts) sprite.gameObject.SetActive(false);
        for (int i = 0; i < hp; i++) hearts[i].gameObject.SetActive(true);
    }

    private void UpdateEnergyBar()
    {
        if (energy <= maxEnergy) energy += energyHealAmount * Time.deltaTime;
        float energyUnit = maxEnergy / energySprites.Length;

        for (int i = 0; i < energySprites.Length; i++)
        {
            energySprites[i].fillAmount = Mathf.Clamp((energy - energyUnit * i) / energyUnit, 0, 1);
            if (energySprites[i].fillAmount < 1f) energySprites[i].color = new Color(1, 1, 1, 0.1f);
            else energySprites[i].color = Color.white;
        }
    }


    public void TakeDamage(int damage = 1)
    {
        FindAnyObjectByType<CameraScript>().ShakeCamera();
        
        hp -= damage;
        UpdateHearts();

        if (hp <= 0 && isAlive)
        {
            StopAllCoroutines();
            DeadState();
            return;
        }

        StartCoroutine(ProtectState());
    }

    public void HealEnergy(float amount = 0)
    {
        if (amount == 0) amount = attackHealEnergyAmount;
        energy += amount;
    }


    private IEnumerator ProtectState()
    {
        Time.timeScale = 0f;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        yield return new WaitForSecondsRealtime(0.4f);

        Time.timeScale = 1f;

        StartCoroutine(IdleState());
        float blinkTimer = 1f;
        while (blinkTimer > 0)
        {
            Blink();
            yield return new WaitForSeconds(0.12f);
            blinkTimer -= 0.12f;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
    }

    private void Blink()
    {
        sprRend.color = sprRend.color == Color.white ? Color.gray : Color.white;
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
