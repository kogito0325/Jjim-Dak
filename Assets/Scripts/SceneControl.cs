using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerMachine>().playerHealth;
    }

    private void Update()
    {
        if (playerHealth.hp <= 0)
        {
            GetComponent<Animator>().Play("FadeOut");
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadScene(string scene, float delay)
    {
        FindAnyObjectByType<PlayerMachine>().StartCoroutine(SceneLoadCoroutine(scene, delay));
    }

    private IEnumerator SceneLoadCoroutine(string scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(scene);
    }
}
