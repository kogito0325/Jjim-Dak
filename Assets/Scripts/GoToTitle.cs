using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToTitle : MonoBehaviour
{
    public void GoTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
