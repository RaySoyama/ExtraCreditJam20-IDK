using UnityEngine;

public class AnimationEventCallback : MonoBehaviour
{
    public delegate void AnimationEventCall(string eventName);
    public AnimationEventCall EventCall;

    public void Blast()
    {
        EventCall?.Invoke("Blast");
    }

}
