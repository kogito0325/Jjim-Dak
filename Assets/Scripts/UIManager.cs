using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UIManager : MonoBehaviour
{
    public PlayerData playerData;
    public Image[] hearts;
    public Image energyBar;

    private void Update()
    {
        UpdateEnergyBar();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < playerData.hp; i++) hearts[i].gameObject.SetActive(true);
        for (int i = playerData.hp; i < playerData.maxHp; i++) hearts[i].gameObject.SetActive(false);
    }

    private void UpdateEnergyBar()
    {
        energyBar.fillAmount = playerData.energy / playerData.maxEnergy;
    }
}
