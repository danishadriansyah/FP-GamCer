using UnityEngine;
using System.Collections;

public class AutoGun : MonoBehaviour
{
    [Header("Pengaturan Senjata")]
    public GameObject projectilePrefab;
    public float fireRate;
    public int projectileDamage = 1;
    public int projectileCount = 1;

    private float nextFireTime;
    private Camera mainCamera;

    // Variabel untuk menghandle reset buff
    private Coroutine damageBuffCoroutine;
    private int currentBuffAmount = 0; // Simpan berapa damage yang ditambahkan terakhir kali

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Time.time > nextFireTime)
        {
            FireAtVisibleEnemies();
            nextFireTime = Time.time + (1 / fireRate);
        }
    }

    void FireAtVisibleEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            if (IsObjectVisible(enemy))
            {
                StartCoroutine(FireBurstAtEnemy(enemy));
            }
        }
    }
    // FIXED LOGIC: Spawn a NEW bullet for every loop iteration
    IEnumerator FireBurstAtEnemy(GameObject enemy)
    {
        for (int i = 0; i < projectileCount; i++)
        {
            // Safety check: Don't keep shooting if the enemy is already dead/null
            if (!enemy.GetComponent<HealthSystem>().GetStatus())
            {
                // 1. Spawn a NEW bullet from the pool
                GameObject projectileObj = ObjectPooler.instance.SpawnFromPool("Projectile", transform.position, Quaternion.identity);

                // 2. Configure the bullet
                ProjectileBehaviour projectile = projectileObj.GetComponent<ProjectileBehaviour>();
                if (projectile != null)
                {
                    projectile.SetTarget(enemy.transform);
                    projectile.SetDamage(projectileDamage);
                }

                // 3. Wait slightly before firing the next bullet in the burst
                // Only wait if we actually have more bullets to fire
                if (i < projectileCount - 1)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
    }

    private bool IsObjectVisible(GameObject obj)
    {
        if (obj == null) return false;
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    // --- LOGIKA BARU BUFF DAMAGE ---
    public void BoostDamage(int amount, float duration)
    {
        // 1. Jika buff sedang jalan, reset dulu
        if (damageBuffCoroutine != null)
        {
            StopCoroutine(damageBuffCoroutine);
            projectileDamage -= currentBuffAmount; // Kurangi damage yang sebelumnya ditambah
            Debug.Log("Buff lama di-reset!");
        }

        // 2. Simpan nilai buff ini untuk keperluan reset nanti
        currentBuffAmount = amount;

        // 3. Jalankan buff baru
        damageBuffCoroutine = StartCoroutine(DamageBuffRoutine(amount, duration));
    }

    IEnumerator DamageBuffRoutine(int amount, float duration)
    {
        projectileDamage += amount;
        Debug.Log("POWER UP! Damage naik jadi: " + projectileDamage);

        yield return new WaitForSeconds(duration);

        projectileDamage -= amount;
        damageBuffCoroutine = null;
        Debug.Log("Power Up Habis. Damage kembali ke: " + projectileDamage);
    }
}