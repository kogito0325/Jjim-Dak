using System.Collections;
using UnityEngine;

public class ShockScript : MonoBehaviour
{
    PlayerScript target;
    public float direction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = FindAnyObjectByType<PlayerScript>();
        StartCoroutine(ShockWave());
    }

    IEnumerator ShockWave()
    {
        while (transform.localScale.x < 3f)
        {
            transform.localScale += Vector3.right * Time.deltaTime;
            yield return null;
        }
        GetComponent<Rigidbody2D>().linearVelocityX = direction * target.playerData.speed * 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
