using System.Collections;
using UnityEngine;

public class ShockScript : MonoBehaviour
{
    public float direction;
    public float speed;
    private Rigidbody2D rigid;
    private float curSpeed;


    void Start()
    {
        curSpeed = 0.001f;
        rigid = GetComponent<Rigidbody2D>();
        StartCoroutine(ShockWave());
    }

    IEnumerator ShockWave()
    {
        GetComponent<Animator>().Play("Wave");
        while (curSpeed < speed)
        {
            curSpeed *= 1 + speed * Time.deltaTime;
            rigid.linearVelocityX = curSpeed * direction;
            yield return null;
        }
        rigid.linearVelocityX = speed * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall") || collision.CompareTag("Player"))
            Destroy(gameObject, 5f);
    }
}
