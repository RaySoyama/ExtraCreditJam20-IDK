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

    public Transform shootTarget;

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

    [SerializeField, ReadOnlyField]
    private bool isEnabled = false;
    public bool IsEnabled
    {
        get
        {
            return isEnabled;
        }
    }

    [SerializeField]
    private Image pylonHealthUI = null;

    [SerializeField]
    private Renderer renderRef = null;

    private Material material = null;


    private void Start()
    {
        PylonManager.instance.AddPylonToList(this);

        pylonCurrentHealth = PylonStartHealth;

        material = renderRef.material;

        isEnabled = false;
    }

    [ContextMenu("ActivatePylon")]
    public void ActivatePylon()
    {
        isEnabled = true;
        PylonManager.instance.ActivatePylon(this);
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);
        material.SetFloat("pylonActive", 1.0f);
    }
    [ContextMenu("DeActivatePylon")]
    public void DeActivatePylon()
    {
        isEnabled = false;
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
