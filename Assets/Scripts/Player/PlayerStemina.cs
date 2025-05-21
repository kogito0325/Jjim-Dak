using UnityEngine;
using UnityEngine.UI;

public class PlayerStemina
{
    PlayerData playerData;
    public Image energySprite;
    private float energy;

    public PlayerStemina(PlayerData playerData, Image image)
    {
        this.playerData = playerData;
        energy = playerData.maxEnergy;
        energySprite = image;
    }


    public void UpdateStemina()
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
        if (energySprite != null)
            energySprite.fillAmount = energy / playerData.maxEnergy;
    }
}
