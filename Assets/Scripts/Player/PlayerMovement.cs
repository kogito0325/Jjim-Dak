using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerData playerData;
    private Rigidbody2D rigidBody;
    private PlayerStemina playerStemina;
    private PlayerAniManager playerAnim;
    private PlayerCombat playerCombat;
    private PlayerHealth playerHealth;

    private float inputX;
    private float xVelocity;

    public bool isGrounded;
    public bool isDashing;
    public bool isJumping;
    public bool isPossibleToDash;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        playerData = GetComponent<PlayerScript>().playerData;
        playerStemina = GetComponent<PlayerStemina>();
        playerAnim = GetComponent<PlayerAniManager>();
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (inputX > 0) transform.rotation = Quaternion.Euler(Vector2.zero);
        else if (inputX < 0) transform.rotation = Quaternion.Euler(Vector2.up * 180f);
        playerAnim.animator.SetFloat("yVelocity", rigidBody.linearVelocityY);
        playerAnim.animator.SetBool("isGround", isGrounded);
        playerAnim.animator.SetBool("isMoving", inputX != 0);
    }

    private void FixedUpdate()
    {
        if (isDashing) rigidBody.linearVelocityY = 0;
    }

    public void Move(float inputX)
    {
        if (isDashing || playerCombat.isHealing) return;
        this.inputX = inputX;
        xVelocity = inputX * playerData.speed;
        rigidBody.linearVelocityX = xVelocity;
        if (inputX != 0 && isGrounded) playerAnim.Play(PlayerAnimState.MOVE);
    }

    public void Jump()
    {
        if (isGrounded && !isDashing)
        {
            playerAnim.Play(PlayerAnimState.JUMP);
            rigidBody.linearVelocityY = playerData.jumpPower;
            isGrounded = false;
        }
    }

    float tempGravity;

    public void Dash()
    {
        if (!isDashing && isPossibleToDash)
        {
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
            playerStemina.SpendEnergy(playerData.dashEnergy);
            playerAnim.Play(PlayerAnimState.DASH);
            tempGravity = rigidBody.gravityScale;
            rigidBody.gravityScale = 0;
            isDashing = true;
            isPossibleToDash = false;
            xVelocity = (transform.rotation.y == 0 ? 1 : -1) * playerData.dashSpeed;
            rigidBody.linearVelocityX = xVelocity;

            Invoke("EndDash", playerData.dashDurationTime);
        }
    }

    private void EndDash()
    {
        if (playerHealth.blinkTimer <= 0)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
        isDashing = false;
        rigidBody.linearVelocityX = 0;
        rigidBody.gravityScale = tempGravity;

        Invoke("EnableDash", playerData.dashCoolTime);
    }

    private void EnableDash()
    {
        isPossibleToDash = true;
    }
}