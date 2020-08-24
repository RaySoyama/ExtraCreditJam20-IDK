using System.Collections.Generic;
using UnityEngine;

public class PylonManager : MonoBehaviour
{
	public float heatGain;
	public float heatLoss;
	public float heatThreshold;
	[SerializeField, ReadOnlyField]
	private float heatLevel = 200;
	[SerializeField, ReadOnlyField]
	private bool overheated = false;
	[SerializeField, ReadOnlyField]
	public int wave = 0;
	private int portalsSpawmnedThisWave = 0;

	public float portalLikelyHoodPerPylon;
	public float portalSpawnTick;

	public List<GameObject> portalPrefabs;

	private float lastPortalSpawnTick = 0f;

    public static PylonManager instance = null;

	private List<GameObject> portals = new List<GameObject>();

	public Transform groundPortalPoints;

	private List<OceanObject> oceanObjects = new List<OceanObject>();

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
		
	}

    void Update()
    {
		if (Time.time >= lastPortalSpawnTick + portalSpawnTick)
		{
			lastPortalSpawnTick = Time.time;

			foreach (PylonController p in allActivePylons)
			{
				float mod = 0f;

				if (portalsSpawmnedThisWave < 1)
				{
					mod = 0.5f;
				}

				float roll = Random.Range(0f, 1f);
				if (roll <= portalLikelyHoodPerPylon + mod)
				{
					GameObject portalToSpawn = portalPrefabs[Random.Range((int)0, portalPrefabs.Count)];
					if (portalToSpawn.GetComponent<Portal>().groundPortal)
					{
						//Spawn on ground
						Vector3 spawnPoint = groundPortalPoints.GetChild(Random.Range(0, groundPortalPoints.childCount)).position;

						GameObject newPortal = Instantiate(portalToSpawn, spawnPoint, Quaternion.identity);
						newPortal.transform.LookAt(p.gameObject.transform.position);

						//Save new portal.
						portals.Add(newPortal);
					}
					else
					{
						float x = Random.Range(0f, 25f);
						float y = Random.Range(40f, 60f);
						float z = Random.Range(0f, 25f);

						Vector3 spawnPoint = new Vector3(x, y, z) + p.gameObject.transform.position;

						GameObject newPortal = Instantiate(portalToSpawn, spawnPoint, Quaternion.identity);
						newPortal.transform.LookAt(p.gameObject.transform.position);

						//Save new portal.
						portals.Add(newPortal);
					}

					portalsSpawmnedThisWave++;
				}
			}
		}

		//Debuging, add 50 heat.
		if (Input.GetKeyDown(KeyCode.Q))
		{
			heatLevel += 50;
		}
		else if (Input.GetKeyDown(KeyCode.E))
		{
			heatLevel -= 50;
		}
	}

	private void FixedUpdate()
	{
		if (!overheated && heatLevel >= heatThreshold)
		{
			wave++;
			overheated = true;
			portalsSpawmnedThisWave = 0;

			//activate some pylons
			int pylonsToActivate = Random.Range(Mathf.Clamp(1 + (int)(wave / 6f), 1, allPylons.Count + 1), Mathf.Clamp((int)(wave / 4f) + 2, 1, allPylons.Count + 1));
				
			if (wave == 1)
			{
				pylonsToActivate = 1;
			}

			Debug.Log("Pylons to activate: " + pylonsToActivate + " Wave: " + wave);

			int alivePylons = 0;
			foreach (var pylon in AllPylons)
			{
				if (!pylon.GetComponent<PylonController>().PylonIsDestroyed)
				{
					alivePylons++;
				}
			}

			if (alivePylons < pylonsToActivate)
			{
				pylonsToActivate = alivePylons;
			}

			int tryStart = Random.Range((int)0, (int)allPylons.Count);
			while (pylonsToActivate > 0)
			{
				if (!allPylons[tryStart].IsEnabled && !allPylons[tryStart].PylonIsDestroyed)
				{
					allPylons[tryStart].ActivatePylon();
					pylonsToActivate--;
				}

				tryStart++;
				if (tryStart == allPylons.Count)
				{
					tryStart = 0;
				}
			}

			//Tell all ocean objects we overheated
			foreach (OceanObject oo in oceanObjects)
			{
				oo.SetSailing(false);
			}

		}
		else if (overheated && heatLevel <= 0f)
		{
			overheated = false;
			//deactivate all pylons
			while (allActivePylons.Count > 0)
			{
				allActivePylons[0].DeActivatePylon();
			}

			//Destroy portals
			foreach (GameObject p in portals)
			{
				p.GetComponent<Portal>().Death();
			}

			portals = new List<GameObject>();

			//Tell all ocean objects to sail again
			foreach (OceanObject oo in oceanObjects)
			{
				oo.SetSailing(true);
			}
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

	public void NewOceanObject(OceanObject oo)
	{
		oceanObjects.Add(oo);
		oo.SetSailing(!overheated);
	}

	public void RemoveOceanObject(OceanObject oo)
	{
		if (oceanObjects.Contains(oo))
		{
			oceanObjects.Remove(oo);
		}
	}

	public bool IsOverheated()
	{
		return overheated;
	}

	public int GetActivePortalCount()
	{
		return portals.Count;
	}

	public void HealLowestPylon(float amount)
	{
		PylonController lowestHealthPlyon = null;
		float lowestHealth = 100000f;
		foreach (PylonController p in allPylons)
		{
			if (p.PylonCurrentHealth > 0f)
			{
				if (lowestHealthPlyon == null)
				{
					lowestHealthPlyon = p;
					lowestHealth = p.PylonCurrentHealth;
				}
				else if (p.PylonCurrentHealth < lowestHealth)
				{
					lowestHealthPlyon = p;
					lowestHealth = p.PylonCurrentHealth;
				}
			}
		}

		if (lowestHealthPlyon != null)
		{
			lowestHealthPlyon.AddHealth(amount);
		}

	}

	public float GetHeatPercent()
	{
		return heatLevel / heatThreshold;
	}

	public void Cool(float amount)
	{
		heatLevel -= amount;

		if (heatLevel < 0f)
		{
			heatLevel = 0f;
		}
	}

}
