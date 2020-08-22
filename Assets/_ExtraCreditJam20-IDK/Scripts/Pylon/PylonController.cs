using UnityEngine;
using UnityEngine.UI;
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


    public Image pylonHealthUI = null;

    private void Start()
    {
        pylonCurrentHealth = PylonStartHealth;
    }

    public void TakeDamage(float damage)
    {
        //damage number validation

        pylonCurrentHealth -= damage;

        if (pylonHealthUI != null)
        {
            pylonHealthUI.fillAmount = PylonCurrentHealth / PylonStartHealth;
        }
        else
        {
            Debug.LogError("Missing reference to Pylon Health UI");
        }

        if (PylonCurrentHealth <= 0)
        {
            //dead
        }
    }
}
