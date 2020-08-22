using UnityEngine;

public class PylonController : MonoBehaviour
{
    [SerializeField]
    private float pylonStartHealth = 20;
    public float PylonStartHealth
    {
        get
        {
            return pylonStartHealth;
        }
    }

    private float pylonCurrentHealth = 20;
    public float PylonCurrentHealth
    {
        get
        {
            return pylonCurrentHealth;
        }
    }

    private void Start()
    {
        pylonCurrentHealth = PylonStartHealth;
    }

    public void TakeDamage(float damage)
    {
        //damage number validation

        pylonCurrentHealth -= damage;

        if (PylonCurrentHealth <= 0)
        {
            //dead
        }
    }
}
