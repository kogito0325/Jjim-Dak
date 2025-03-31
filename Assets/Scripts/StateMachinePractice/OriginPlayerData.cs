/*using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Origin : MonoBehaviour
{



    private void Start()
    {
        StartCoroutine(IdleState());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1)) Guard();

        UpdateEnergyBar();

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }



    private IEnumerator IdleState()
    {
        Debug.Log("Idle");
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
            if (Input.GetKeyDown(KeyCode.W) && !isHealing)
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
        Debug.Log("Move");
        animator.SetBool("isMoving", true);
        while (true)
        {
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
        Debug.Log("Jump");
        isGrounded = false;
        rigidBody.linearVelocityY = jumpPower;
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
        Debug.Log("Attack");
        animator.SetBool("isAttackStarted", true);
        attackRange.SetActive(true);
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
            Debug.Log("Attack2");
            animator.SetBool("isAttacking", true);
            attackRange.SetActive(true);
        }
        yield return new WaitForSeconds(attackDurationTime);
        animator.SetBool("isAttackStarted", false);
        animator.SetBool("isAttacking", false);
        attackRange.SetActive(false);
        StartCoroutine(IdleState());
    }


    private IEnumerator DashState()
    {
        Debug.Log("Dash");
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
        Debug.Log("Heal");
        isHealing = true;
        float healTimer = healTime;

        while (isHealing && healTimer > 0)
        {
            if (Input.GetKeyUp(KeyCode.W)) isHealing = false;
            else if (Input.GetKeyDown(KeyCode.LeftShift) && isDashingPossible && energy >= dashEnergy)
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
            Debug.Log("Healing Success");
        }
        else
        {
            Debug.Log("Healing Cancled");
        }
        StartCoroutine(IdleState());
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
        Debug.Log("Guard");
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
        Debug.Log("Counter");
        Instantiate(counterPrefab, transform.position, Quaternion.identity);
        InitializeFlags();
        StartCoroutine(IdleState());
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) isGrounded = false;
    }
}

*/