using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerCombat playerCombat;
    PlayerHealth playerHealth;
    PlayerMovement playerMovement;

    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!playerHealth.isAlive) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            playerMovement.isGrounded = true;
        }

        if (collision.collider.CompareTag("Boss"))
        {
            if (playerCombat.isGuarding) playerCombat.CounterAttack();
            else playerHealth.TakeDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossAtk"))
        {
            collision.gameObject.SetActive(false);
            if (playerCombat.isGuarding) playerCombat.CounterAttack();
            else playerHealth.TakeDamage();
        }
    }
}
