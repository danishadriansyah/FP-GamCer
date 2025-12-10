using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Collider2D playerCollider = transform.parent.GetComponent<Collider2D>();
        if (other.CompareTag("ExperienceOrb"))
        {
            other.GetComponent<ExperienceOrb>().SetTarget(playerCollider);
        }
        else if (other.CompareTag("Consumable"))
        {
            ConsumableItem item = other.GetComponent<ConsumableItem>();
            if (item != null)
            {
                item.SetTarget(playerCollider);
            }
        }
    }
}