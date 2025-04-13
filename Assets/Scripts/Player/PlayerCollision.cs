using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    PlayerData playerData;
    PlayerCombat playerCombat;
    PlayerHealth playerHealth;
    PlayerStemina playerStemina;
    PlayerMovement playerMovement;

    void Start()
    {
        playerData = GetComponent<PlayerScript>().playerData;
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStemina = GetComponent<PlayerStemina>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!playerHealth.isAlive) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            playerMovement.isGrounded = true;
        }

        if (collision.collider.CompareTag("Boss") || collision.collider.CompareTag("BossAtk"))
        {
            if (playerCombat.isGuarding) playerCombat.CounterAttack();
            else playerHealth.TakeDamage();
        }
    }
}
