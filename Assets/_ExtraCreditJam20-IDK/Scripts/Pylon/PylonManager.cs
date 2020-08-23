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
	private int wave = 0;

	public float portalLikelyHoodPerPylon;
	public float portalSpawnTick;

	public List<GameObject> portalPrefabs;

	private float lastPortalSpawnTick = 0f;

    public static PylonManager instance = null;

	private List<GameObject> portals = new List<GameObject>();

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
						float y = Random.Range(40f, 60f);
						float z = Random.Range(0f, 25f);

						Vector3 spawnPoint = new Vector3(x, y, z) + p.gameObject.transform.position;

						GameObject newPortal = Instantiate(portalToSpawn, spawnPoint, Quaternion.identity);
						newPortal.transform.LookAt(p.gameObject.transform.position);

						//Save new portal.
						portals.Add(newPortal);
					}
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

			//activate some pylons
			int pylonsToActivate = Random.Range(Mathf.Clamp(1 + (int)(wave / 6f), 1, allPylons.Count + 1), Mathf.Clamp((int)(wave / 4f) + 2, 1, allPylons.Count + 1));
				
			if (wave == 1)
			{
				pylonsToActivate = 1;
			}

			Debug.Log("Pylons to activate: " + pylonsToActivate);

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
