using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExperienceOrb"))
        {
            other.GetComponent<ExperienceOrb>().SetTarget(transform.parent);
        }
        else if (other.CompareTag("Consumable"))
        {
            ConsumableItem item = other.GetComponent<ConsumableItem>();
            if (item != null)
            {
                item.SetTarget(transform.parent);
            }
        }
    }
}