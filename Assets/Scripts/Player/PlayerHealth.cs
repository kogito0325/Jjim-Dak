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
        hp -= damage;
        
        // 데미지를 입었을 때
        if (damage > 0)
        {
            Object.FindAnyObjectByType<CameraScript>().ShakeCamera();
            hearts[hp].GetComponent<Animator>().Play($"Disappear{hp}");
            playerMachine.StartCoroutine(ProtectState(true));
        }
        else
        {
            UpdateHearts();
            if (damage == 0)
                playerMachine.StartCoroutine(ProtectState(false));
        }

        if (hp <= 0) Die();
    }

    private IEnumerator ProtectState(bool isDamaged)
    {
        if (blinkTimer > 0) yield break;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        if (isDamaged)
        {
            playerMachine.canControl = false;
            playerAni.Play(PlayerAnimState.HIT);
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(0.2f);
            Time.timeScale = 1f;
        }

        InitBlinkTimer();

        while (blinkTimer > 0)
        {
            if (playerData.protectTime - blinkTimer >= playerData.knockBackDurationTime)
                playerMachine.canControl = true;
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