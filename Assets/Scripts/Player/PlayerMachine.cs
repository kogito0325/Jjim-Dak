using UnityEngine;
using UnityEngine.UI;

public class PlayerMachine : MonoBehaviour
{
    public PlayerData playerData;

    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;
    public PlayerStemina playerStemina;
    public PlayerAniManager playerAniManager;
    public PlayerSoundManager playerSoundManager;

    private Rigidbody2D rigid;
    private Animator animator;
    private AudioSource audioSource;

    public Image energySprite;
    public Image[] hearts;
    public AudioClip[] audioClips;
    public GameObject attackRange;
    public GameObject healObject;
    public GameObject guardObject;

    private float inputX;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerAniManager = new PlayerAniManager(animator);
        playerSoundManager = new PlayerSoundManager(audioClips, audioSource);
        playerStemina = new PlayerStemina(playerData, energySprite);
        playerHealth = new PlayerHealth(this, playerAniManager);
        playerCombat = new PlayerCombat(this, rigid);
        playerMovement = new PlayerMovement(this, rigid);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            playerHealth.TakeDamage();

        if (!playerHealth.isAlive) return;

        inputX = Input.GetAxisRaw("Horizontal");

        playerMovement.UpdateDir(transform, inputX);
        playerMovement.Move(inputX);
        playerStemina.UpdateStemina();
        playerCombat.Update();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStemina.CheckEnergy(playerData.dashEnergy))
        {
            playerMovement.Dash(transform);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!playerHealth.isAlive) return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            playerMovement.isGrounded = true;
        }

        if (collision.collider.CompareTag("Boss"))
        {
            if (playerCombat.isGuarding) playerCombat.CounterAttack(collision.gameObject.GetComponent<BossScript>());
            else playerHealth.TakeDamage();
        }

        if (collision.collider.CompareTag("BossAtk"))
        {
            if (playerCombat.isGuarding) playerCombat.CounterAttack(collision.gameObject.GetComponentInParent<BossScript>());
            else playerHealth.TakeDamage();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossAtk"))
        {
            if (collision.GetComponent<ShockScript>() != null)
            {
                collision.enabled = false;
                Destroy(collision.gameObject, 5f);
            }
            else collision.gameObject.SetActive(false);
            if (playerCombat.isGuarding) playerCombat.CounterAttack(collision.gameObject.GetComponentInParent<BossScript>());
            else playerHealth.TakeDamage();
        }
    }
}