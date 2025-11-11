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
        float roll = Random.value; // Lempar dadu sekali

        // --- INI LOGIKA BARUNYA ---
        if (currentStress > 15f)
        {
            // 1. PEMAIN KESULITAN
            enemyTagToSpawn = "TimeAnomaly";
        }
        else if (currentStress < -30f) // Kita bikin lebih ekstrim syaratnya
        {
            // 2. PEMAIN JAGO BANGET (Stres sangat negatif): 
            // "GELOMBANG KEJUTAN" (Shock Wave)
            // Kirim si Elite!
            if (roll < 0.1f) // 10% kemungkinan kirim ELITE
            {
                enemyTagToSpawn = "EliteAnomaly";
                Debug.LogWarning("AI Director: Mengirim GELOMBANG KEJUTAN (Elite)!");
            }
            else // 90% sisanya kirim si cepat
            {
                enemyTagToSpawn = "ChronoHound";
            }
        }
        else if (currentStress < -10f)
        {
            // 3. PEMAIN JAGO (Stres negatif):
            // "Gelombang Tekanan" (Pressure Wave)
            if (roll < 0.6f)
                enemyTagToSpawn = "ChronoHound";
            else
                enemyTagToSpawn = "TimeAnomaly";
        }
        else
        {
            // 4. PEMAIN NORMAL: 
            // "Gelombang Pengepungan" (Siege Wave)
            if (roll < 0.15f)
                enemyTagToSpawn = "VoidBehemoth";
            else if (roll < 0.4f)
                enemyTagToSpawn = "TemporalWeaver";
            else
                enemyTagToSpawn = "TimeAnomaly";
        }
        // --- AKHIR LOGIKA BARU ---

        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * 15f;
        ObjectPooler.instance.SpawnFromPool(enemyTagToSpawn, spawnPos, Quaternion.identity);
    }

}