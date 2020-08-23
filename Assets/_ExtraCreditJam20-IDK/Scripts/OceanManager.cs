using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanManager : MonoBehaviour
{

	public List<GameObject> oceanObjectPrefabs;

	private float startZ = 2500f;
	private float startY = -100f;

	private float negXMax = -2500f;
	private float negXMin = -600f;

	private float posXMax = 2500f;
	private float posXMin = 600f;

	public float spawnIntervalMin;
	public float spawnIntervalMax;

	private float nextSpawnInterval = 0f;
	private float lastSpawnTime = 0f;


	private PylonManager pm;

	private void Start()
	{
		pm = FindObjectOfType<PylonManager>();
	}

	// Update is called once per frame
	void Update()
    {

		if (!pm.IsOverheated() && Time.time >= lastSpawnTime + nextSpawnInterval)
		{
			//Spawn
			Vector3 pos;
			if (Random.Range(0f, 1f) < 0.5f)
			{
				pos = new Vector3(Random.Range(negXMin, negXMax), startY, startZ);
			}
			else
			{
				pos = new Vector3(Random.Range(posXMin, posXMax), startY, startZ);
			}

			Instantiate(oceanObjectPrefabs[Random.Range(0, oceanObjectPrefabs.Count)], pos, Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)));

			lastSpawnTime = Time.time;
			nextSpawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
		}

    }
}
