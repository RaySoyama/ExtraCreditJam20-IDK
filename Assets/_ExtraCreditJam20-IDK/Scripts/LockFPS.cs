using UnityEngine;

public class LockFPS : MonoBehaviour
{
    void Update()
    {
        Application.targetFrameRate = 60;
    }
}
