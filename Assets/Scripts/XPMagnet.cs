using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Deteksi Orb XP (Logika Lama)
        if (other.CompareTag("ExperienceOrb"))
        {
            other.GetComponent<ExperienceOrb>().SetTarget(transform.parent);
        }
        // 2. Deteksi Item Konsumsi (Logika Baru)
        else if (other.CompareTag("Consumable")) // Pastikan Tag item di Unity diatur ke "Consumable"
        {
            ConsumableItem item = other.GetComponent<ConsumableItem>();
            if (item != null)
            {
                item.SetTarget(transform.parent);
            }
        }
    }
}