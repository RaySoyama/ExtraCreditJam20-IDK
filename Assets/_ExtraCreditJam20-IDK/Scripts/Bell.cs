using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bell : MonoBehaviour
{
	private AudioSource audio;

	public AudioClip ring;

    void Start()
    {
		audio = GetComponent<AudioSource>();
    }

	public void TakeHit()
	{
		audio.Stop();
		//audio.PlayOneShot(rings[Random.Range(0, rings.Count)]);
		audio.PlayOneShot(ring);
	}
}
