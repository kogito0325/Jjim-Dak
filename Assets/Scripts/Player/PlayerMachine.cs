using UnityEngine;
using UnityEngine.UI;

public class PlayerMachine : MonoBehaviour
{
    [Header("Player Datas")]
    public PlayerData playerData;
    public PlayerSoundData PlayerSoundData;

    public PlayerMovement playerMovement;
    public PlayerCombat playerCombat;
    public PlayerHealth playerHealth;
    public PlayerStemina playerStemina;
    public PlayerAniManager playerAniManager;
    public PlayerSoundManager playerSoundManager;

    private Rigidbody2D rigid;
    private Animator animator;
    private AudioSource audioSource;

    [Header("\nDynamic Allocation")]
    public Image energySprite;
    public Image[] hearts;
    public GameObject attackRange;
    public GameObject healObject;
    public GameObject guardObject;

    public bool canControl;
    private float inputX;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        playerAniManager = new PlayerAniManager(animator);
        playerSoundManager = new PlayerSoundManager(PlayerSoundData, audioSource);
        playerStemina = new PlayerStemina(playerData, energySprite);
        playerHealth = new PlayerHealth(this, playerAniManager);
        playerCombat = new PlayerCombat(this, rigid);
        playerMovement = new PlayerMovement(this, rigid);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
        canControl = true;
    }

    void Update()
    {
        playerStemina.UpdateStemina();
        playerCombat.Update();

        if (!canControl || !playerHealth.isAlive || playerCombat.isGuarding || Time.timeScale == 0) return;

        inputX = Input.GetAxisRaw("Horizontal");

        playerMovement.UpdateDir(transform, inputX);
        if (playerCombat.atkTimer <= 0)
        {
            playerMovement.Move(inputX);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovement.Jump();
            playerCombat.atkTimer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && playerStemina.CheckEnergy(playerData.dashEnergy))
        {
            playerMovement.Dash(transform);
        }
        if (Input.GetKeyDown(KeyCode.Mouse0) && !playerMovement.isDashing)
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

    private void KnockBack(Vector3 rootPos)
    {
        rigid.linearVelocity = Vector2.zero;
        float knockBack_x = transform.position.x - rootPos.x > 0 ? 1 : -1;
        float knockBack_y = Vector2.up.y*2;

        transform.rotation = Quaternion.Euler(Vector3.up * (knockBack_x == -1 ? 0 : -180));
        Vector2 knockBackDir = new Vector2(knockBack_x, knockBack_y);
        rigid.AddForce(knockBackDir.normalized * playerData.knockBackPower, ForceMode2D.Impulse);
    }

    private void PlaySound(PlayerSoundData.AudioType audioType)
    {
        playerSoundManager.Play(audioType);
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
            else
            {
                KnockBack(collision.transform.position);
                playerHealth.TakeDamage();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BossAtk"))
        {
            collision.enabled = false;
            if (playerCombat.isGuarding) playerCombat.CounterAttack(collision.gameObject.GetComponentInParent<BossScript>());
            else
            {
                KnockBack(FindAnyObjectByType<BossScript>().transform.position);
                playerHealth.TakeDamage();
            }
        }
    }
}