using System.Collections;
using UnityEngine;

public enum ItemType
{
    HealthPotion,
    SpeedPotion,
    AttackPotion
}

[RequireComponent(typeof(Collider2D))]
public class ConsumableItem : MonoBehaviour
{
    [Header("Pengaturan Item")]
    public ItemType type;
    public float value;
    public float duration;

    [Header("Pengaturan Animasi & Magnet")]
    public float moveSpeed = 8f;      // Kecepatan item terbang ke pemain
    public float absorbDuration = 0.3f; // Berapa lama animasi mengecil berlangsung

    private Transform target;         // Target (Player)
    private bool isAbsorbing = false; // Status apakah sedang diserap
    private Vector3 originalScale;    // Ukuran asli untuk reset pooling
    private Collider2D col;

    void Awake()
    {
        originalScale = transform.localScale;
        col = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        // Reset kondisi saat spawn dari Object Pooler
        transform.localScale = originalScale;
        target = null;
        isAbsorbing = false;
        col.enabled = true;
    }

    // Fungsi ini dipanggil oleh XPMagnet saat item masuk radius
    public void SetTarget(Transform newTarget)
    {
        if (isAbsorbing) return;
        target = newTarget;
    }

    void Update()
    {
        // Jika sudah punya target (kena magnet), terbang ke arah pemain
        if (target != null && !isAbsorbing)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        }
    }

    // Saat item akhirnya menyentuh badan Pemain
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAbsorbing)
        {
            // 1. Kunci status biar tidak dipanggil double
            isAbsorbing = true;

            // 2. Terapkan Efek Item
            ApplyEffect(other.gameObject);

            // 3. Matikan collider biar tidak mengganggu
            col.enabled = false;

            // 4. Mulai animasi terserap (mengecil lalu hilang)
            StartCoroutine(AbsorbAnimation());
        }
    }

    void ApplyEffect(GameObject player)
    {
        switch (type)
        {
            case ItemType.HealthPotion:
                HealthSystem health = player.GetComponent<HealthSystem>();
                if (health != null) health.Heal((int)value);
                break;

            case ItemType.SpeedPotion:
                PlayerMovement movement = player.GetComponent<PlayerMovement>();
                if (movement != null) movement.BoostSpeed(value, duration);
                break;

            case ItemType.AttackPotion:
                AutoGun weapon = player.GetComponent<AutoGun>();
                if (weapon != null) weapon.BoostDamage((int)value, duration);
                break;
        }
    }
    IEnumerator AbsorbAnimation()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        while (timer < absorbDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, timer / absorbDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        // Setelah hilang, kembalikan ke Pool
        gameObject.SetActive(false);
    }
}