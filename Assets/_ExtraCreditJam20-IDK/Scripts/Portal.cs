using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
	private int health = 15;

    // Start is called before the first frame update
    void Start()
    {
		gameObject.GetComponent<AudioSource>().pitch += Random.Range(0f, 0.5f);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void TakeHit()
	{
		health--;

		if (health == 0)
		{
			Destroy(this);
		}
	}
}
