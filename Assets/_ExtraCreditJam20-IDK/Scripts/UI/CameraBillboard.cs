using UnityEngine;


public class CameraBillboard : MonoBehaviour
{

    [Header("Billboard Settings")]
    [SerializeField]
    private bool BillboardX = true;
    [SerializeField]
    private bool BillboardY = true;
    [SerializeField]
    private bool BillboardZ = true;
    [SerializeField]
    private float OffsetToCamera = 0;

    protected Vector3 localStartPosition = new Vector3();
    protected Vector3 localStartScale = new Vector3();

    [SerializeField]
    private float minRange = 3;

    [SerializeField]
    private float maxRange = 5;

    void Start()
    {
        localStartPosition = transform.localPosition;
        localStartScale = transform.localScale;
    }
    void Update()
    {

        if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < minRange)
        {
            transform.localScale = localStartScale;
        }
        else if (Vector3.Distance(PlayerController.instance.transform.position, transform.position) < maxRange)
        {
            transform.localScale = localStartScale * (1.0f - ((Vector3.Distance(PlayerController.instance.transform.position, transform.position) - minRange) / (maxRange - minRange)));
        }
        else
        {
            transform.localScale = Vector3.zero;
            return;
        }


        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        if (BillboardX == false || BillboardY == false || BillboardZ == false)
        {
            transform.rotation = Quaternion.Euler(BillboardX ? transform.rotation.eulerAngles.x : 0f, BillboardY ? transform.rotation.eulerAngles.y : 0f, BillboardZ ? transform.rotation.eulerAngles.z : 0f);
        }

        transform.localPosition = localStartPosition;
        transform.position = transform.position + transform.rotation * Vector3.forward * OffsetToCamera;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxRange);


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minRange);
    }

}
