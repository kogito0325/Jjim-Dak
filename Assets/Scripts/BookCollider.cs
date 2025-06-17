using System.Collections;
using UnityEngine;

public class BookCollider : MonoBehaviour
{
    [SerializeField] SpriteRenderer tutotial;
    [SerializeField] float fadeSpeed;

    float goal = 0;

    private void Update()
    {
        tutotial.color = new Color(1f, 1f, 1f, Mathf.Lerp(tutotial.color.a, goal, Time.deltaTime * fadeSpeed));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMachine _))
            goal = 1;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerMachine _))
            goal = 0;
    }
}