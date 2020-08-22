using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    public static MinimapManager instance = null;

    private struct TrackerData
    {
        public MinimapTracker tracker;
        public Image pin;
    }

    public enum TrackType
    {
        PLAYER,
        PYLON,
        ENEMY
    }

    [SerializeField]
    private float scale = 100;
    [SerializeField]
    private Vector2 offset;

    [SerializeField]
    private Transform canvas;

    [SerializeField]
    private List<MinimapTracker> AllTrackers = new List<MinimapTracker>();

    [SerializeField]
    private GameObject defaultPin = null;

    [SerializeField]
    private Sprite pylonImg = null;

    [SerializeField]
    private Sprite pylonInactiveImg = null;

    [SerializeField]
    private Sprite enemyImg = null;

    [SerializeField]
    private Sprite playerImg = null;

    [SerializeField]
    private List<TrackerData> AllTrackerData = new List<TrackerData>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Multiple instance of Minimap Manager");
            Destroy(this);
        }
    }

    void Start()
    {
        AllTrackerData = new List<TrackerData>();

        UpdateMinimap();


        for (int i = 0; i < 10; i++)
        {
            AllTrackerData.Add(NewTrackerData());
        }
    }

    void Update()
    {
        UpdateMinimap();
    }

    void UpdateMinimap()
    {
        foreach (TrackerData item in AllTrackerData)
        {
            if (item.tracker != null && item.pin.gameObject.activeSelf == true)
            {
                item.pin.transform.localPosition = new Vector3(item.tracker.transform.position.x * scale - offset.x, item.tracker.transform.position.z * scale - offset.y, 0);

                if (item.tracker.Type == TrackType.PYLON)
                {
                    if (item.tracker.PylonManager.IsEnabled == false)
                    {
                        item.pin.sprite = pylonInactiveImg;
                    }
                }

            }
        }
    }


    private TrackerData NewTrackerData()
    {
        TrackerData data = new TrackerData();
        data.pin = Instantiate(defaultPin, canvas, false).GetComponent<Image>();
        data.tracker = null;
        data.pin.gameObject.SetActive(false);
        return data;
    }

    private TrackerData GetAvailableTrackerData()
    {
        foreach (TrackerData item in AllTrackerData)
        {
            if (item.pin.gameObject.activeSelf == false)
            {
                item.pin.gameObject.SetActive(true);
                return item;
            }
        }
        TrackerData data = NewTrackerData();
        data.pin.gameObject.SetActive(true);
        AllTrackerData.Add(data);
        return data;
    }

    public void AddTracker(MinimapTracker tracker)
    {
        TrackerData data = GetAvailableTrackerData();
        data.tracker = tracker;

        switch (tracker.Type)
        {
            case TrackType.PYLON:
                data.pin.sprite = pylonImg;
                data.pin.transform.localScale = Vector3.one;
                break;
            case TrackType.ENEMY:
                data.pin.sprite = enemyImg;
                data.pin.transform.localScale = Vector3.one * 0.5f;
                break;
            case TrackType.PLAYER:
                data.pin.sprite = playerImg;
                data.pin.transform.localScale = Vector3.one;
                break;
        }

        AllTrackerData.Add(data);
    }
    public void RemoveTracker(MinimapTracker tracker)
    {
        foreach (TrackerData data in AllTrackerData)
        {
            if (data.tracker == tracker)
            {
                AllTrackerData.Remove(data);
                return;
            }
        }
    }
}
