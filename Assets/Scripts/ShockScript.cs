using System.Collections;
using UnityEngine;

public class ShockScript : MonoBehaviour
{
    PlayerMachine target;
    public float direction;
    public float speed;


    void Start()
    {
        target = FindAnyObjectByType<PlayerMachine>();
        speed = direction * target.playerData.speed / 2;
        StartCoroutine(ShockWave());
    }

    IEnumerator ShockWave()
    {
        while (transform.localScale.x < 3f)
        {
            transform.localScale += Vector3.right * Time.deltaTime;
            speed *= 1 + Time.deltaTime;
            GetComponent<Rigidbody2D>().linearVelocityX = speed * (transform.localScale.x / transform.localScale.y);

            yield return null;
        }
        GetComponent<Rigidbody2D>().linearVelocityX = speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
