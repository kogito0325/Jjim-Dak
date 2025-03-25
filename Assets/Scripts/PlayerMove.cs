using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int hp;
    [SerializeField] private float speed;
    [SerializeField] private int maxEnergy;
    [SerializeField] private float jumpPower;
    [SerializeField] private float dashPower;


    [Header("Utilities")]
    [SerializeField] private int dashEnergy;
    [SerializeField] private float dashTime;
    [SerializeField] private int guardEnergy;
    [SerializeField] private float energyHealAmount;
    
    public Image[] hearts;
    public Image energyBar;
    

    private float energy;
    private float inputX;
    private float xVelocity;

    private bool isJumping;
    private bool isDashing;

    SpriteRenderer spriteRenderer;
    Rigidbody2D rigidBody;
    Collider2D playerCollider;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();

        energy = maxEnergy;

        isJumping = false;
        isDashing = false;

        InitHearts();
    }

    private void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        if (!isDashing) xVelocity = inputX * speed;
        
        if (Input.GetKeyDown(KeyCode.Space)) Jump();
        if (Input.GetKeyDown(KeyCode.Mouse1)) Guard();
        if (Input.GetKeyDown(KeyCode.LeftShift)) StartCoroutine(Dash());
        
        UpdateEnergyBar();

        if (xVelocity > 0) spriteRenderer.flipX = false;
        else if (xVelocity < 0) spriteRenderer.flipX = true;
        rigidBody.linearVelocityX = xVelocity;

        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    private void Jump()
    {
        if (!isJumping)
        {
            rigidBody.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            isJumping = true;
        }
    }

    
    private IEnumerator Dash()
    {
        if (!isDashing && energy >= dashEnergy && xVelocity != 0)
        {
            energy -= dashEnergy;
            isDashing = true;
            xVelocity = inputX * dashPower;
            yield return new WaitForSeconds(dashTime);
            isDashing = false;
        }
    }

    private  void Guard()
    {
        if (energy >= guardEnergy)
        {
            energy -= guardEnergy;
        }
    }

    private void InitHearts()
    {
        foreach (Image sprite in hearts) sprite.gameObject.SetActive(false);
        for (int i = 0; i < hp; i++) hearts[i].gameObject.SetActive(true);
    }

    private void UpdateEnergyBar()
    {
        if (energy <= maxEnergy) energy += energyHealAmount;
        energyBar.fillAmount = energy / maxEnergy;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("floor")) isJumping = false;
    }
}
