using System.Collections;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField] float flipSpeed;
    [SerializeField, Range(0.5f, 1f)] float flipDuration;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(BookFlip());
    }

    private IEnumerator BookFlip()
    {
        while (flipSpeed  > 1f)
        {
            flipSpeed = Mathf.Lerp(flipSpeed, 0f, flipDuration * Time.deltaTime);
            animator.speed = flipSpeed;
            yield return null;
        }
        animator.speed = 1f;
        animator.SetTrigger("Open");
    }
}
