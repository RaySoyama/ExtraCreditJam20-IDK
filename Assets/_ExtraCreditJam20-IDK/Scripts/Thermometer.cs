using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Thermometer : MonoBehaviour
{
	private PylonManager pm;

	public Image img;

	public 

    // Start is called before the first frame update
    void Start()
    {
		pm = FindObjectOfType<PylonManager>();
    }

    // Update is called once per frame
    void Update()
    {
		img.fillAmount = pm.GetHeatPercent();
    }
}
