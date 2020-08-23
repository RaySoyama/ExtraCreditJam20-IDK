using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleController : MonoBehaviour
{
	private AudioSource audio;

	public AudioClip introduction;
	private bool hasPlayedIntro = false;


    void Start()
    {
		audio = GetComponent<AudioSource>();
    }

    
    void Update()
    {
		if (Time.time > 1f && !hasPlayedIntro)
		{
			audio.PlayOneShot(introduction);
			hasPlayedIntro = true;
		}
    }
}
