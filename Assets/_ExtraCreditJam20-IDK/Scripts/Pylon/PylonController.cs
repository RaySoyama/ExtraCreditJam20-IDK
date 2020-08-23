using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PylonController : MonoBehaviour
{
	private AudioSource audio;
	public AudioClip vent;
	public AudioClip die;
	public AudioClip deactivate;

	public GameObject crystal;

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

    public UnityEvent onStartCooling;
    public UnityEvent onDoneCooling;

	private void Awake()
	{
		audio = GetComponent<AudioSource>();
	}

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
        onStartCooling.Invoke();
        isEnabled = true;
        PylonManager.instance.ActivatePylon(this);
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);
        material.SetFloat("pylonActive", 1.0f);

		audio.PlayOneShot(vent);
    }

    [ContextMenu("DeActivatePylon")]
    public void DeActivatePylon()
    {
        onDoneCooling.Invoke();
        isEnabled = false;
        PylonManager.instance.DeActivatePylon(this);
        material.SetFloat("pylonHealth", PylonCurrentHealth / PylonStartHealth);
        material.SetFloat("pylonActive", 0.0f);

		audio.PlayOneShot(deactivate);
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
            onDoneCooling.Invoke();
            PylonManager.instance.DeActivatePylon(this);
        }
    }

	private void Update()
	{
		if (isEnabled)
		{
			//crystal.transform.rotation = Quaternion.Euler(new Vector3(crystal.transform.rotation.x, 10f + crystal.transform.rotation.y, crystal.transform.rotation.z));
			//crystal.transform.Rotate(0f, 1f, 0f);
		}
	}
}
