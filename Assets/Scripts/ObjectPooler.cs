using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Ini adalah class kecil untuk mengatur setiap "kolam" di Inspector
    [System.Serializable]
    public class Pool
    {
        public string tag;      // Nama untuk memanggil pool (misal: "Enemy", "Projectile")
        public GameObject prefab; // Prefab yang akan di-pool
        public int size;      // Jumlah objek yang disiapkan di awal
    }

    // Buat Singleton agar mudah diakses dari script lain
    public static ObjectPooler instance;
    private void Awake()
    {
        instance = this;
    }

    public List<Pool> pools; // Daftar semua kolam yang kita miliki

    // Ini adalah tempat penyimpanan utama kita
    public Dictionary<string, List<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, List<GameObject>>();

        // Isi setiap kolam saat game dimulai
        foreach (Pool pool in pools)
        {
            List<GameObject> objectList = new List<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false); // Nonaktifkan
                objectList.Add(obj);
            }

            poolDictionary.Add(pool.tag, objectList);
        }
    }

    // Fungsi utama untuk "meminjam" objek dari kolam
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool dengan tag " + tag + " tidak ada.");
            return null;
        }

        List<GameObject> objectList = poolDictionary[tag];

        // 1. Cari objek yang non-aktif
        foreach (GameObject obj in objectList)
        {
            if (!obj.activeInHierarchy)
            {
                // Ditemukan! Aktifkan, atur posisinya, dan berikan
                obj.SetActive(true);
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                return obj;
            }
        }

        // 2. Jika semua sibuk, buat yang baru (dinamis)
        // Cari prefab asli dari daftar 'pools'
        Pool poolToGrow = null;
        foreach (Pool p in pools)
        {
            if (p.tag == tag)
            {
                poolToGrow = p;
                break;
            }
        }

        if (poolToGrow != null)
        {
            GameObject newObj = Instantiate(poolToGrow.prefab); // Instansiasi dari prefab asli
            newObj.SetActive(true);
            newObj.transform.position = position;
            newObj.transform.rotation = rotation;
            objectList.Add(newObj); // Tambahkan ke daftar pool agar bisa didaur ulang
            return newObj;
        }

        Debug.LogError("Gagal menemukan prefab untuk menumbuhkan pool tag: " + tag);
        return null;
    }
}