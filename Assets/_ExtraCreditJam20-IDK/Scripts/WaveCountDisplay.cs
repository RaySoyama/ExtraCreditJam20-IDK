using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveCountDisplay : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject textField;
    string text;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<PylonManager>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        text = "Wave " + gameManager.GetComponent<PylonManager>().wave;

        textField.GetComponent<TextMeshProUGUI>().text = text;
    }
}