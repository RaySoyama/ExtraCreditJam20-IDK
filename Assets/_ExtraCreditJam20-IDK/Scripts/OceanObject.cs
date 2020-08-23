﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanObject : MonoBehaviour
{
	private bool isSailing = false;

	private float speed = 1f;
	private float diveSpeed = 0.5f;
	private float zDivePoint = 1000f;
	private float yDespawnPoint = -20f;

    void Start()
    {
		FindObjectOfType<PylonManager>().NewOceanObject(this);
    }

    // Update is called once per frame
    void Update()
    {
		if (isSailing)
		{
			//Move
			transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);

			if (transform.position.z >= zDivePoint)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y - diveSpeed, transform.position.z);
			}

			if (transform.position.y <= yDespawnPoint)
			{
				FindObjectOfType<PylonManager>().RemoveOceanObject(this);
				Destroy(this);
			}

		}
    }

	public void SetSailing(bool val)
	{
		isSailing = val;
	}

}
