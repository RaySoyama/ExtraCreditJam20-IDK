using UnityEngine;

public class MinimapTracker : MonoBehaviour
{
    [SerializeField]
    private MinimapManager.TrackType type = MinimapManager.TrackType.ENEMY;
    public MinimapManager.TrackType Type
    {
        get
        {
            return type;
        }
    }

    [SerializeField]
    private PylonController pylonManager = null;
    public PylonController PylonManager
    {
        get
        {
            return pylonManager;
        }
    }

    private void Start()
    {
        MinimapManager.instance.AddTracker(this);
    }

    private void OnDestroy()
    {
        MinimapManager.instance.RemoveTracker(this);
    }

}
