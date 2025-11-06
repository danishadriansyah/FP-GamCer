using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    // --- Pengaturan (Sama seperti sebelumnya) ---
    [Header("Pengaturan Intensitas")]
    public float damageStressAmount = 10f;
    public float killReliefAmount = 2f;
    public float stressDecayRate = 1.5f;

    [Header("Pengaturan Spawn")]
    public float baseSpawnInterval = 1f;
    public float minSpawnInterval = 0.2f;
    public float maxSpawnInterval = 3f;

    [Header("Pengaturan Ritme")]
    public float baseWaveDuration = 20f;
    public float baseRestDuration = 8f;

    // --- Variabel Internal (Sama seperti sebelumnya) ---
    private float currentStress = 0f;
    private float stateTimer;
    private float spawnTimer;
    private DirectorState currentState;
    private Transform playerTransform;

    private enum DirectorState
    {
        Spawning,
        Resting
    }

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = DirectorState.Resting;
        stateTimer = baseRestDuration;
    }

    // --- Event Handlers (Sama seperti sebelumnya) ---
    void OnEnable()
    {
        HealthSystem.OnPlayerDamaged += HandlePlayerDamaged;
        HealthSystem.OnEnemyKilled += HandleEnemyKilled;
    }
    void OnDisable()
    {
        HealthSystem.OnPlayerDamaged -= HandlePlayerDamaged;
        HealthSystem.OnEnemyKilled -= HandleEnemyKilled;
    }
    void HandlePlayerDamaged(int damageAmount)
    {
        currentStress += damageStressAmount;
        Debug.Log("Director: Pemain kena damage! Stres naik ke: " + currentStress);
    }
    void HandleEnemyKilled()
    {
        currentStress -= killReliefAmount;
        Debug.Log("Director: Pemain membunuh musuh! Stres turun ke: " + currentStress);
    }

    // --- Logika Update (Sama seperti sebelumnya) ---
    void Update()
    {
        // 1. Pulihkan Stres
        if (currentStress > 0)
        {
            currentStress -= stressDecayRate * Time.deltaTime;
            currentStress = Mathf.Max(0, currentStress);
        }
        else if (currentStress < 0)
        {
            currentStress += stressDecayRate * Time.deltaTime;
            currentStress = Mathf.Min(0, currentStress);
        }

        // 2. Logika State Machine (Ritme)
        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0)
        {
            if (currentState == DirectorState.Spawning)
            {
                currentState = DirectorState.Resting;
                stateTimer = baseRestDuration + (currentStress * 0.1f);
                stateTimer = Mathf.Clamp(stateTimer, baseRestDuration * 0.5f, baseRestDuration * 2f);
                Debug.Log("Director: Masuk fase RESTING selama " + stateTimer + " detik.");
            }
            else if (currentState == DirectorState.Resting)
            {
                currentState = DirectorState.Spawning;
                stateTimer = baseWaveDuration - (currentStress * 0.2f);
                stateTimer = Mathf.Clamp(stateTimer, baseWaveDuration * 0.5f, baseWaveDuration * 2f);
                Debug.Log("Director: Masuk fase SPAWNING selama " + stateTimer + " detik.");
            }
        }

        // 3. Logika Spawning
        if (currentState == DirectorState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy(); // Panggil fungsi spawn yang baru

                float spawnInterval = baseSpawnInterval + (currentStress * 0.05f);
                spawnTimer = Mathf.Clamp(spawnInterval, minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    // --- FUNGSI SpawnEnemy() TELAH DI-UPGRADE TOTAL ---
    void SpawnEnemy()
    {
        if (playerTransform == null) return;

        string enemyTagToSpawn;

        // --- INI LOGIKA BARUNYA ---
        if (currentStress > 15f)
        {
            // 1. PEMAIN KESULITAN (Stres tinggi): 
            // Kasih ampun, spawn musuh dasar saja
            enemyTagToSpawn = "TimeAnomaly";
        }
        else if (currentStress < -20f)
        {
            // 2. PEMAIN JAGO (Stres sangat negatif): 
            // Hukum dengan "Gelombang Tekanan" (Pressure Wave)
            // Kirim musuh cepat!
            enemyTagToSpawn = "ChronoHound";
        }
        else
        {
            // 3. PEMAIN NORMAL (Stres di antara -20 dan 15): 
            // Ini adalah "Gelombang Pengepungan" (Siege Wave)
            // Kita campur musuh dasar (melee) dengan musuh penembak (ranged).
            if (Random.value < 0.3f) // 30% kemungkinan spawn penembak
            {
                enemyTagToSpawn = "TemporalWeaver";
            }
            else // 70% kemungkinan spawn melee
            {
                enemyTagToSpawn = "TimeAnomaly";
            }
        }
        // --- AKHIR LOGIKA BARU ---

        // Tentukan posisi spawn
        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * 15f;

        // Panggil dari Object Pooler menggunakan tag yang sudah kita tentukan
        ObjectPooler.instance.SpawnFromPool(enemyTagToSpawn, spawnPos, Quaternion.identity);
    }
}