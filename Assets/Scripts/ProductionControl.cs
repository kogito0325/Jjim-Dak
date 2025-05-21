using System.Collections;
using UnityEngine;

public class ProductionControl : MonoBehaviour
{
    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject movieCam;
    [SerializeField] private GameObject uis;
    [SerializeField] private PlayerMachine player;
    [SerializeField] private BossScript boss;

    private void Start()
    {
        StartCoroutine(Produce());
    }

    private IEnumerator Produce()
    {
        yield return new WaitForSeconds(4.5f);
        movieCam.SetActive(false);
        mainCam.SetActive(true);
        uis.SetActive(true);

        player.enabled = true;
        boss.enabled = true;
    }
}
