using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PowerupSpawner : MonoBehaviour
{
    public float maxSpawnWait = 60.0f;
    public float minSpawnWait = 25.0f;

    private float spawnRate = 0.0f;
    private float _spawnRate = 0.0f;

    public List<GameObject> powerups = new List<GameObject>();

    void Update()
    {
        _spawnRate += Time.deltaTime;

        if (_spawnRate >= spawnRate)
        {
            spawnRate = Random.Range(minSpawnWait, maxSpawnWait);
            _spawnRate = 0f;
            StartCoroutine(spawnPowerup());
        }
    }

    //Very laggy :(
    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 finalPosition = Vector3.zero;

        while (Vector3.Distance(finalPosition, PlayerController.instance.transform.position) >= 10f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius + transform.position;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
            {
                finalPosition = hit.position;
            }
        }

        return finalPosition;
    }

    IEnumerator spawnPowerup()
    {
        Vector3 finalPosition = Vector3.zero;

        while (finalPosition == Vector3.zero)
        {
            Vector3 randomDirection = Random.insideUnitSphere * 85;
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomDirection, out hit, 85, 1))
            {
                finalPosition = hit.position;
            }
        }

        Debug.Log(finalPosition);
        Instantiate(powerups[Random.Range(0, powerups.Count)], finalPosition + Vector3.up, Quaternion.identity);

        yield return null;
    }
}