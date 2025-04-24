using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerAniManager playerAni;
    public Image[] hearts;
    public int hp { get; private set; }
    public bool isAlive { get; set; } = true;
    public float blinkTimer = 0;

    private void Start()
    {
        playerData = GetComponent<PlayerScript>().playerData;
        playerAni = GetComponent<PlayerAniManager>();
        hp = playerData.maxHp;
    }

    public void TakeDamage(int damage = 1)
    {
        if (damage > 0)  // 데미지를 입었을 때
            FindAnyObjectByType<CameraScript>().ShakeCamera();

        hp -= damage;
        if (damage > 0)
            hearts[hp].GetComponent<Animator>().Play($"Disappear{hp}");
        else
            UpdateHearts();

        if (hp <= 0)
        {
            Die();
        }
        if (damage > 0)  // 데미지를 입었을 때
            StartCoroutine(ProtectState(true));
        else if (damage == 0)  // 방어했을 때
            StartCoroutine(ProtectState(false));
    }

    private IEnumerator ProtectState(bool isDamaged)
    {
        if (blinkTimer > 0) yield break;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        if (isDamaged)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.4f);
            Time.timeScale = 1f;
        }

        InitBlinkTimer();

        while (blinkTimer > 0)
        {
            if(isDamaged) Blink();
            yield return new WaitForSeconds(0.12f);
            blinkTimer -= 0.12f;
        }
        GetComponent<SpriteRenderer>().color = Color.white;

        if (isAlive)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
    }

    private void Blink()
    {
        SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
        sprRend.color = sprRend.color == Color.white ? Color.gray : Color.white;
    }

    public void InitBlinkTimer()
    {
        blinkTimer = playerData.protectTime;
    }


    private void Die()
    {
        isAlive = false;
        playerAni.Play(PlayerAnimState.DEAD);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        GetComponent<Rigidbody2D>().linearVelocityX = 0;
    }
    private void UpdateHearts()
    {
        foreach (Image sprite in hearts) sprite.enabled = false;
        for (int i = 0; i < hp; i++) hearts[i].GetComponent<Animator>().Play("Idle");
    }
}