using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth
{
    private PlayerMachine playerMachine;
    private PlayerData playerData;
    private PlayerAniManager playerAni;
    private Image[] hearts;
    public int hp { get; private set; }
    public bool isAlive { get; set; }
    public float blinkTimer = 0;


    public PlayerHealth(PlayerMachine player, PlayerAniManager playerAni)
    {
        playerMachine = player;
        playerData = player.playerData;
        this.playerAni = playerAni;
        hearts = player.hearts;

        hp = playerData.maxHp;
        isAlive = true;
    }

    public void TakeDamage(int damage = 1)
    {
        if (damage > 0)  // 데미지를 입었을 때
            Object.FindAnyObjectByType<CameraScript>().ShakeCamera();

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
            playerMachine.StartCoroutine(ProtectState(true));
        else if (damage == 0)  // 방어했을 때
            playerMachine.StartCoroutine(ProtectState(false));
    }

    private IEnumerator ProtectState(bool isDamaged)
    {
        if (blinkTimer > 0) yield break;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        if (isDamaged)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.2f);
            Time.timeScale = 1f;
        }

        InitBlinkTimer();

        while (blinkTimer > 0)
        {
            if(isDamaged) Blink();
            yield return new WaitForSeconds(0.12f);
            blinkTimer -= 0.12f;
        }
        playerMachine.GetComponent<SpriteRenderer>().color = Color.white;

        if (isAlive)
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
    }

    private void Blink()
    {
        SpriteRenderer sprRend = playerMachine.GetComponent<SpriteRenderer>();
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
        playerMachine.GetComponent<Rigidbody2D>().linearVelocityX = 0;
    }
    private void UpdateHearts()
    {
        foreach (Image sprite in hearts) sprite.enabled = false;
        for (int i = 0; i < hp; i++) hearts[i].GetComponent<Animator>().Play("Idle");
    }
}