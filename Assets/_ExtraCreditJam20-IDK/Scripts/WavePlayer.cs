using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePlayer : MonoBehaviour
{
	private PylonManager pm;
	private AudioSource audio;

	private float startVolume;

	public float fadeSpeed;

	void Start()
	{
		pm = FindObjectOfType<PylonManager>();
		audio = GetComponent<AudioSource>();

		startVolume = audio.volume;
		audio.volume = 0f;
	}

	void Update()
	{
		if (!pm.IsOverheated())
		{
			//Fade waves in
			audio.volume = Mathf.Clamp(audio.volume + fadeSpeed, 0f, startVolume);
		}
		else
		{
			//fade waves out
			audio.volume = Mathf.Clamp(audio.volume - fadeSpeed, 0f, startVolume);
		}

	}
}
