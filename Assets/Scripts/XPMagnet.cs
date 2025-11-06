using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    // Fungsi ini dipanggil saat ada orb masuk ke jangkauan magnet
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ExperienceOrb"))
        {
            // Beri tahu orb siapa targetnya (yaitu Player, si 'parent' dari magnet ini)
            other.GetComponent<ExperienceOrb>().SetTarget(transform.parent);
        }
    }
}