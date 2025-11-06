using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] // Pastikan ada collider
public class ExperienceOrb : MonoBehaviour
{
    public int xpValue = 5;
    public float moveSpeed = 5f; // Kecepatan orb ditarik
    public float absorbDuration = 0.5f; // Durasi animasi terserap (detik)

    private Transform target; // Target (Player)
    private bool isAbsorbing = false;
    private Vector3 originalScale;
    private Collider2D col;
    private Coroutine deactivateCoroutine; // Untuk menyimpan timer 5 detik

    void Awake()
    {
        // Simpan ukuran asli dan collider
        originalScale = transform.localScale;
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        // Reset state setiap kali 'bangun' dari pool
        transform.localScale = originalScale; // Kembalikan ke ukuran normal
        target = null;
        isAbsorbing = false;
        col.enabled = true; // Nyalakan lagi collidernya

        // Mulai timer 5 detik
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime(5f));
    }

    // Fungsi yang dipanggil oleh XPMagnet
    public void SetTarget(Transform newTarget)
    {
        if (isAbsorbing) return; // Jangan ditarik kalo lagi diserap
        target = newTarget;
    }

    void Update()
    {
        // Jika target sudah ada (Player masuk jangkauan) & belum diserap
        if (target != null && !isAbsorbing)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    // --- INI LOGIKA BARUNYA ---
    // Saat Orb menyentuh sesuatu
    void OnTriggerEnter2D(Collider2D other)
    {
        // Jika itu Player dan belum dalam proses diserap
        if (other.CompareTag("Player") && !isAbsorbing)
        {
            isAbsorbing = true; // Mulai proses penyerapan

            // 1. Matikan timer 5 detik (karena udah diambil)
            if (deactivateCoroutine != null)
            {
                StopCoroutine(deactivateCoroutine);
            }

            // 2. Beri XP ke Player
            PlayerExperience playerExp = other.GetComponent<PlayerExperience>();
            if (playerExp != null)
            {
                playerExp.AddXP(xpValue);
            }

            // 3. Matikan magnet & collider
            target = null;
            col.enabled = false;

            // 4. Mulai animasi terserap
            StartCoroutine(AbsorbAnimation());
        }
    }

    // --- INI ANIMASINYA ---
    IEnumerator AbsorbAnimation()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero; // Menyusut sampai hilang

        while (timer < absorbDuration)
        {
            // Menyusutkan ukuran orb seiring waktu
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / absorbDuration);
            timer += Time.deltaTime;
            yield return null; // Tunggu frame berikutnya
        }

        // Setelah animasi selesai, kembalikan ke pool
        gameObject.SetActive(false);
    }

    private IEnumerator DeactivateAfterTime(float time)
    {
        // Timer 5 detik (sama seperti sebelumnya)
        float timer = 0f;
        while (timer < time)
        {
            if (target != null)
            {
                yield break; // Batalkan jika orb sudah ditarik
            }
            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }
}