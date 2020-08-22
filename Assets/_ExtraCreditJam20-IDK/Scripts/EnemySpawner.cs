using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static float gameTime = 0f;

    [System.Serializable]
    public class Enemy
    {
        public GameObject enemyPrefab;
        public int spawnCost = 1;
        public Vector3 startRotation = Vector3.zero;
    }

    public float difficulty = 1f;
    public AnimationCurve difficultyProgression;
    public float waveFrequency = 5.0f;
    public float spawnRadius = 0.5f;
    public float stackTimer = 0.1f;

    public float currentDifficulty = 0f;
    
    public List<Enemy> spawnables = new List<Enemy>();

    List<Enemy> toSpawn = new List<Enemy>();
    float waveProgress = 0f;
    float stackProgress = 0f;

    void Update()
    {
        stackProgress += Time.deltaTime;
        gameTime += Time.deltaTime;
        waveProgress += Time.deltaTime;

        if (waveProgress >= waveFrequency)
        {
            waveProgress = 0f;
            
            currentDifficulty = difficultyProgression.Evaluate(gameTime * 0.001f) * difficulty;
            
            int spawnAmount = Mathf.RoundToInt(currentDifficulty * 100);

            if (spawnAmount < 1) spawnAmount = 1;

            int failSafe = 0;

            while (spawnAmount > 0)
            {
                failSafe += 1;

                for (int i = 0; i < spawnables.Count; i++)
                {
                    if (spawnables[i].spawnCost <= spawnAmount)
                    {
                        toSpawn.Add(spawnables[i]);
                        spawnAmount -= spawnables[i].spawnCost;
                    }
                }

                if(failSafe >= 1000)
                    break;
            }
        }
        
        if(toSpawn.Count > 0 && stackProgress >= stackTimer)
        {
            stackProgress = 0f;
            Instantiate(toSpawn[0].enemyPrefab, transform.position + (Random.insideUnitSphere * spawnRadius), Quaternion.Euler(toSpawn[0].startRotation));
            toSpawn.Remove(toSpawn[0]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}