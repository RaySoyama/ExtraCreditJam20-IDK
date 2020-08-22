using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startRandomForce : MonoBehaviour
{
    public float maxForce = 50;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce), Random.Range(-maxForce, maxForce)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}