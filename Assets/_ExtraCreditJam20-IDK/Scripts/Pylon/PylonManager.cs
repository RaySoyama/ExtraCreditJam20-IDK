﻿using System.Collections.Generic;
using UnityEngine;

public class PylonManager : MonoBehaviour
{
	public float heatGain;
	public float heatLoss;
	public float heatThreshold;
	[SerializeField, ReadOnlyField]
	private float heatLevel;
	[SerializeField, ReadOnlyField]
	private bool overheated = false;

	public float portalLikelyHoodPerPylon;
	public float portalSpawnTick;

	public List<GameObject> portalPrefabs;

	private float lastPortalSpawnTick = 0f;

    public static PylonManager instance = null;

    [SerializeField, ReadOnlyField]
    private List<PylonController> allPylons = new List<PylonController>();
    public List<PylonController> AllPylons
    {
        get
        {
            return allPylons;
        }
    }

    [SerializeField, ReadOnlyField]
    private List<PylonController> allActivePylons = new List<PylonController>();
    public List<PylonController> AllActivePylons
    {
        get
        {
            return allActivePylons;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }
    void Start()
    {
		//For testing
		//foreach (var pylon in AllPylons)
		//{
		//	if (Random.Range(0.0f, 1.0f) >= 0.5f)
		//	{
		//		pylon.ActivatePylon();
		//	}
		//}
	}

    void Update()
    {
		if (Time.time >= lastPortalSpawnTick + portalSpawnTick)
		{
			foreach (PylonController p in allActivePylons)
			{
				float roll = Random.Range(0f, 1f);

				if (roll <= portalLikelyHoodPerPylon)
				{
					GameObject portalToSpawn = portalPrefabs[Random.Range((int)0, portalPrefabs.Count)];
					bool portalIsAGroundEnemySpawner = false;
					if (portalIsAGroundEnemySpawner)
					{
						//Spawn based on nav mesh
					}
					else
					{
						float x = Random.Range(0f, 25f);
						float y = Random.Range(30f, 40f);
						float z = Random.Range(0f, 25f);

						Vector3 spawnPoint = new Vector3(x, y, z) + p.gameObject.transform.position;

						GameObject newPortal = Instantiate(portalToSpawn, spawnPoint, Quaternion.identity);
						newPortal.transform.LookAt(p.gameObject.transform.position);
					}
				}
			}
		}
    }

	private void FixedUpdate()
	{
		if (!overheated && heatLevel >= heatThreshold)
		{
			overheated = true;
			//activate some pylons
			foreach (var pylon in AllPylons)
			{
				if (Random.Range(0.0f, 1.0f) >= 0.5f)
				{
					pylon.ActivatePylon();
				}
			}

			//If no pylons were activated pick a random one to activate.
			if (allActivePylons.Count == 0)
			{
				allPylons[Random.Range((int)0, allPylons.Count)].ActivatePylon();
			}
		}
		else if (overheated && heatLevel <= 0f)
		{
			overheated = false;
			//deactivate all pylons
		}

		if (overheated)
		{
			heatLevel -= heatLoss;
		}
		else
		{
			heatLevel += heatGain;
		}
	}

	public void AddPylonToList(PylonController pylon)
    {
        allPylons.Add(pylon);
    }

    public void ActivatePylon(PylonController pylon)
    {
        allActivePylons.Add(pylon);
    }

    public void DeActivatePylon(PylonController pylon)
    {
        allActivePylons.Remove(pylon);
    }
}
