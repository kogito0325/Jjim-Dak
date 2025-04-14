using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public PlayerData playerData;

    private PlayerMovement playerMovement;
    private PlayerCombat playerCombat;
    private PlayerHealth playerHealth;
    private PlayerStemina playerStemina;

    private float inputX;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerCombat = GetComponent<PlayerCombat>();
        playerHealth = GetComponent<PlayerHealth>();
        playerStemina = GetComponent<PlayerStemina>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            playerHealth.TakeDamage();

        if (!playerHealth.isAlive) return;

        inputX = Input.GetAxisRaw("Horizontal");

        playerMovement.Move(inputX);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStemina.CheckEnergy(playerData.dashEnergy))
        {
            playerMovement.Dash();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            playerCombat.Attack();
        }
        if (Input.GetKeyDown(KeyCode.W) && playerStemina.CheckEnergy(playerData.healEnergy))
        {
            playerCombat.Heal();
        }
        if (Input.GetKeyDown(KeyCode.Mouse1) && playerStemina.CheckEnergy(playerData.guardEnergy))
        {
            playerCombat.Guard();
        }
    }
}