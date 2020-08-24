using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWin : MonoBehaviour
{
    public PylonManager gameManager;

    public int wavesToWin = 20;

    public string winSceneName;

    int wave;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindObjectOfType<PylonManager>();
    }

    // Update is called once per frame
    void Update()
    {
        wave = gameManager.wave;

        if(wave > wavesToWin)
        {
            //win

            GameObject.FindObjectOfType<SceneFunctions>().ChangeScene(winSceneName);
        }
    }
}
