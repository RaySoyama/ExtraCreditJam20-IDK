using System.Collections.Generic;
using UnityEngine;

public class PylonManager : MonoBehaviour
{
    public static PylonManager instance = null;

    [SerializeField, ReadOnlyField]
    private List<PylonController> allPylons = new List<PylonController>();
    public List<PylonController> AllPylons
    {
        get
        {
            return allPylons;
        }
    }

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
        allPylons.Add(pylon);
    }

    public void ActivatePylon(PylonController pylon)
    {
        allActivePylons.Add(pylon);
    }

    public void DeActivatePylon(PylonController pylon)
    {
        allActivePylons.Remove(pylon);
    }
}
