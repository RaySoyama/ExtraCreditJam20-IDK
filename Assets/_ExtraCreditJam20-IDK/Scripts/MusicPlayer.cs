using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
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
		if (pm.GetActivePortalCount() > 0)
		{
			//Fade music in
			audio.volume = Mathf.Clamp(audio.volume + fadeSpeed, 0f, startVolume);
		}
		else
		{
			//fade music out
			audio.volume = Mathf.Clamp(audio.volume - fadeSpeed, 0f, startVolume);
		}
    }
}
