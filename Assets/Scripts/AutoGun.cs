using UnityEngine;
using System.Collections.Generic;

public class AutoGun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float fireRate; // Kita perpanjang sedikit jeda tembaknya
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
                // Buat proyektil baru
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

                // Beri tahu proyektil siapa targetnya
                projectileObj.GetComponent<ProjectileBehaviour>().SetTarget(enemy.transform);
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
}