using System.Collections;
using UnityEngine;

public class PlayerMovement
{
    private Transform playerTransform;
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

    public PlayerMovement(PlayerMachine player, Rigidbody2D rigid)
    {
        playerData = player.playerData;

        rigidBody = rigid;
        playerTransform = player.transform;
        playerStemina = player.playerStemina;
        playerAnim = player.playerAniManager;
        playerCombat = player.playerCombat;
        playerHealth = player.playerHealth;

        isGrounded = true;
        isDashing = false;
        isJumping = false;
        isPossibleToDash = true;
    }

    public void UpdateDir(Transform transform, float inputX)
    {
        if (inputX > 0) transform.rotation = Quaternion.Euler(Vector2.zero);
        else if (inputX < 0) transform.rotation = Quaternion.Euler(Vector2.up * 180f);
        if (isDashing) rigidBody.linearVelocityY = 0;
        UpdateAnim();
    }

    public void UpdateAnim()
    {
        playerAnim.animator.SetFloat("yVelocity", rigidBody.linearVelocityY);
        playerAnim.animator.SetBool("isGround", isGrounded);
        playerAnim.animator.SetBool("isMoving", inputX != 0);
    }


    public void Move(float inputX)
    {
        if (isDashing || playerCombat.isHealing) return;
        this.inputX = inputX;
        xVelocity = inputX * playerData.speed;
        rigidBody.linearVelocityX = xVelocity;
        if (inputX != 0 && isGrounded) playerAnim.Play(PlayerAnimState.MOVE);

        UpdateDir(playerTransform, inputX);
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

    public void Dash(Transform transform)
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

            transform.GetComponent<PlayerMachine>().StartCoroutine(EndDash(transform));
        }
    }

    private IEnumerator EndDash(Transform transform)
    {
        yield return new WaitForSeconds(playerData.dashDurationTime);

        if (playerHealth.blinkTimer <= 0)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
        isDashing = false;
        rigidBody.linearVelocityX = 0;
        rigidBody.gravityScale = tempGravity;

        yield return new WaitForSeconds(playerData.dashCoolTime);
        EnableDash();
    }

    private void EnableDash()
    {
        isPossibleToDash = true;
    }
}