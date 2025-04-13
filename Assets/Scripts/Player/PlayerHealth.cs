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
        UpdateHearts();

        if (hp <= 0)
        {
            Die();
        }
        if (damage >= 0)  // 데미지를 입었을 때, 방어 성공 했을 때
            StartCoroutine(ProtectState());
    }

    private IEnumerator ProtectState()
    {
        Time.timeScale = 0f;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
        yield return new WaitForSecondsRealtime(0.4f);

        Time.timeScale = 1f;

        float blinkTimer = playerData.protectTime;
        while (blinkTimer > 0)
        {
            Blink();
            yield return new WaitForSeconds(0.12f);
            blinkTimer -= 0.12f;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"), false);
    }

    private void Blink()
    {
        SpriteRenderer sprRend = GetComponent<SpriteRenderer>();
        sprRend.color = sprRend.color == Color.white ? Color.gray : Color.white;
    }


    private void Die()
    {
        isAlive = false;
        playerAni.Play(PlayerAnimState.DEAD);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Boss"));
    }
    private void UpdateHearts()
    {
        foreach (Image sprite in hearts) sprite.gameObject.SetActive(false);
        for (int i = 0; i < hp; i++) hearts[i].gameObject.SetActive(true);
    }
}