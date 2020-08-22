using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum FiringMode { Laser, RapidFire, Shotgun, Burst}
    public FiringMode fireMode = FiringMode.Laser;

    public int damage = 1;
    public float shotCooldown = 0.5f;
    public float shotLength = 100f;
    public float beamAliveTime = 0.5f;
    
    public GameObject laser;

    public bool isPlayer = false;
    public bool fireQueued = false;

    float shotCD = 0f;
    float beamTime = 0f;

    void Update()
    {
        if (isPlayer && Input.GetMouseButton(0))
            Fire();

        shotCD += Time.deltaTime;

        if(laser.activeSelf)
        {
            fireQueued = false;

            //Do raycast damage here
            
            beamTime += Time.deltaTime;

            if(beamTime >= beamAliveTime)
            {
                beamTime = 0f;
                laser.SetActive(false);
            }
        }

        switch (fireMode)
        {
            case FiringMode.Laser:
                if (fireQueued && shotCD >= shotCooldown)
                {
                    shotCD = 0f;
                    laser.SetActive(true);
                }

                break;
            default:
                Debug.Log("Not implemented");
                break;
        }
    }

    public void Fire()
    {
        fireQueued = true;
    }
}