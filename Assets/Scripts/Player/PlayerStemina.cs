using UnityEngine;
using UnityEngine.UI;

public class PlayerStemina : MonoBehaviour
{
    PlayerData playerData;
    public Image[] energySprites;
    private float energy;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerData = GetComponent<PlayerScript>().playerData;
        energy = playerData.maxEnergy;
    }

    // Update is called once per frame
    void Update()
    {
        HealEnergy();
        UpdateEnergy();
    }

    private void HealEnergy()
    {
        if (energy < playerData.maxEnergy)
            energy += playerData.energyHealAmount * Time.deltaTime;
    }

    public void HealEnergy(float amount)
    {
        if (energy < playerData.maxEnergy)
            energy += amount;
    }

    public bool CheckEnergy(float amount)
    {
        return amount <= energy;
    }

    public void SpendEnergy(float amount)
    {
        energy -= amount;
    }

    private void UpdateEnergy()
    {
        float energyUnit = playerData.maxEnergy / energySprites.Length;

        for (int i = 0; i < energySprites.Length; i++)
        {
            energySprites[i].fillAmount = Mathf.Clamp((energy - energyUnit * i) / energyUnit, 0, 1);
            if (energySprites[i].fillAmount < 1f) energySprites[i].color = new Color(1, 1, 1, 0.1f);
            else energySprites[i].color = Color.white;
        }
    }
}
