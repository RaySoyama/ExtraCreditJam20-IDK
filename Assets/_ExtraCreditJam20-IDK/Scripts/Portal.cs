using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
		gameObject.GetComponent<AudioSource>().pitch += Random.Range(0f, 0.5f);
	}

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Death()
	{
		//Called when wave ends.
		Destroy(gameObject);
	}
}
