using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private BossScript boss;

    private bool isUsed = false;

    private void Start()
    {
        playerHealth = FindAnyObjectByType<PlayerMachine>().playerHealth;
        boss = FindAnyObjectByType<BossScript>();
        isUsed = false;
    }

    private void Update()
    {
        if (isUsed) return;

        if (playerHealth.hp <= 0)
        {
            LoadScene("TitleScene", 0f);
            isUsed = true;
        }
        else if(boss.hp <= 0)
        {
            LoadScene("EndingScene", 5f);
            isUsed= true;
        }
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void LoadScene(string scene, float delay)
    {
        StartCoroutine(SceneLoadCoroutine(scene, delay));
    }

    private IEnumerator SceneLoadCoroutine(string scene, float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<Animator>().Play("FadeOut");
        yield return new WaitForSeconds(3f);
        LoadScene(scene);
    }
}
