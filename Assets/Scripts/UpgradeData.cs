using UnityEngine;

public enum UpgradeType
{
    WeaponSpeed,
    WeaponDamage,
    PlayerSpeed,
    HP,
    WeaponProjectileCount,
    XP_Gained
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    [Header("Visuals")]
    // The pre-generated image with text/icon included
    public Sprite fullCardImage; 

    [Header("Logic")]
    // We still need these so the code knows what to do!
    public UpgradeType type;
    public float value;
}