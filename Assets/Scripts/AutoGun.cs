using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoGun : MonoBehaviour
{
    [Header("Pengaturan Senjata")]
    public GameObject projectilePrefab;
    public float fireRate;
    public int projectileDamage = 1;
    private float nextFireTime;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Time.time > nextFireTime)
        {
            FireAtVisibleEnemies();
            nextFireTime = Time.time + fireRate;
        }
    }

    void FireAtVisibleEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in allEnemies)
        {
            // Cek apakah musuh terlihat di kamera
            if (IsObjectVisible(enemy))
            {
                // Buat proyektil baru dari Pool
                GameObject projectileObj = ObjectPooler.instance.SpawnFromPool("Projectile", transform.position, Quaternion.identity);

                // Beri tahu proyektil siapa targetnya dan berapa damagenya
                ProjectileBehaviour projectile = projectileObj.GetComponent<ProjectileBehaviour>();
                if (projectile != null)
                {
                    projectile.SetTarget(enemy.transform);

                    // Kirim nilai damage saat ini (termasuk buff jika ada)
                    projectile.SetDamage(projectileDamage);
                }
            }
        }
    }

    // Fungsi untuk mengecek apakah sebuah objek ada di dalam layar
    private bool IsObjectVisible(GameObject obj)
    {
        if (obj == null) return false;
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(obj.transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }

    public void BoostDamage(int amount, float duration)
    {
        StartCoroutine(DamageBuffRoutine(amount, duration));
    }

    // Coroutine untuk mengatur durasi buff
    IEnumerator DamageBuffRoutine(int amount, float duration)
    {
        // 1. Tambah damage sementara
        projectileDamage += amount;
        Debug.Log("POWER UP! Damage naik jadi: " + projectileDamage);

        // 2. Tunggu selama durasi item (misal 10 detik)
        yield return new WaitForSeconds(duration);

        // 3. Kembalikan damage ke semula
        projectileDamage -= amount;
        Debug.Log("Power Up Habis. Damage kembali ke: " + projectileDamage);
    }
}