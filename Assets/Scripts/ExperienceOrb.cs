using System.Collections;
using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public int xpValue = 5;

    // OnEnable dipanggil SETIAP KALI objek di-SetActivate(true)
    void OnEnable()
    {
        // Mulai hitungan mundur 5 detik setiap kali diaktifkan
        StartCoroutine(DeactivateAfterTime(5f));
    }

    private IEnumerator DeactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);

        // Kembalikan ke pool
        gameObject.SetActive(false);
    }
}