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

    [SerializeField, ReadOnlyField]
    private float pylonCurrentHealth = 20;
    public float PylonCurrentHealth
    {
        get
        {
            return pylonCurrentHealth;
        }
    }

    public bool PylonIsDestroyed
    {
        get
        {
            if (PylonCurrentHealth <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    [SerializeField]
    private Image pylonHealthUI = null;

    [SerializeField]
    private Renderer renderer = null;

    private Material material = null;

    private void Start()
    {
        PylonManager.instance.AddPylonToList(this);

        pylonCurrentHealth = PylonStartHealth;

        material = renderer.material;
    }

    public void ActivatePylon()
    {
        PylonManager.instance.ActivatePylon(this);
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);
        material.SetFloat("pylonActive", 1.0f);
    }

    public void DeActivatePylon()
    {
        PylonManager.instance.DeActivatePylon(this);
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);
        material.SetFloat("pylonActive", 0.0f);
    }

    public void TakeDamage(float damage)
    {
        //damage number validation

        pylonCurrentHealth -= damage;
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);

        if (pylonHealthUI != null)
        {
            pylonHealthUI.fillAmount = PylonCurrentHealth / PylonStartHealth;
        }
        else
        {
            Debug.LogError("Missing reference to Pylon Health UI");
        }

        if (PylonIsDestroyed)
        {
            //DEAD
            PylonManager.instance.DeActivatePylon(this);
        }
    }
}
