using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    // --- PENGATURAN INI AKAN MUNCUL DI INSPECTOR ---
    [Header("Pengaturan Intensitas")]
    [Tooltip("Berapa 'poin stres' yang didapat pemain saat kena damage.")]
    public float damageStressAmount = 10f;
    [Tooltip("Berapa 'poin stres' yang pulih saat pemain berhasil membunuh musuh.")]
    public float killReliefAmount = 2f;
    [Tooltip("Berapa 'poin stres' yang pulih secara alami setiap detik.")]
    public float stressDecayRate = 1.5f;

    [Header("Pengaturan Spawn")]
    [Tooltip("Interval spawn dasar (saat stres normal).")]
    public float baseSpawnInterval = 1f;
    [Tooltip("Batas spawn tercepat (saat pemain jago / stres 0).")]
    public float minSpawnInterval = 0.2f;
    [Tooltip("Batas spawn terlambat (saat pemain kesulitan / stres tinggi).")]
    public float maxSpawnInterval = 3f;

    [Header("Pengaturan Ritme")]
    [Tooltip("Durasi dasar fase 'Spawning'.")]
    public float baseWaveDuration = 20f;
    [Tooltip("Durasi dasar fase 'Resting'.")]
    public float baseRestDuration = 8f;
    // --- AKHIR DARI PENGATURAN INSPECTOR ---


    // --- Variabel Status Internal (Ini tidak akan muncul di Inspector) ---
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

    // --- Berlangganan Event ---
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

    // --- Event Handlers (Ini adalah "Mata" & "Telinga" Director) ---
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

    // --- Logika Utama (Update) ---
    void Update()
    {
        // 1. Pulihkan Stres secara alami
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

        // 3. Logika Spawning (jika sedang di fase Spawning)
        if (currentState == DirectorState.Spawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy();

                float spawnInterval = baseSpawnInterval + (currentStress * 0.05f);
                spawnTimer = Mathf.Clamp(spawnInterval, minSpawnInterval, maxSpawnInterval);
            }
        }
    }

    void SpawnEnemy()
    {
        if (playerTransform == null) return;

        Vector2 spawnPos = (Vector2)playerTransform.position + Random.insideUnitCircle.normalized * 15f;
        ObjectPooler.instance.SpawnFromPool("TimeAnomaly", spawnPos, Quaternion.identity);
    }
}