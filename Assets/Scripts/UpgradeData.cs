using UnityEngine;

public enum UpgradeType
{
    WeaponSpeed,
    WeaponDamage,
    PlayerSpeed,
    WeaponProjectileCount
}

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Game/Upgrade")]
public class UpgradeData : ScriptableObject
{
    public UpgradeType type;
    public string upgradeName;
    [TextArea]
    public string description;
}