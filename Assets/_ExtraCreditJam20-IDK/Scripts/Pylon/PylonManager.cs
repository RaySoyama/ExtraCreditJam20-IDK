using System.Collections.Generic;
using UnityEngine;

public class PylonManager : MonoBehaviour
{
    public static PylonManager instance = null;

    [SerializeField, ReadOnlyField]
    private List<PylonController> allActivePylons = new List<PylonController>();
    public List<PylonController> AllActivePylons
    {
        get
        {
            return allActivePylons;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }

    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void AddPylonToList(PylonController pylon)
    {
        allActivePylons.Add(pylon);
    }

    public void RemovePylonFromList(PylonController pylon)
    {
        allActivePylons.Remove(pylon);
    }
}
