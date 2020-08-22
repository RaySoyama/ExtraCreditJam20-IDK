using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    private Camera cam;
    public Transform camPos;

    private Rigidbody rb;

    private Vector3 lookOffset = new Vector3(0f, 2.5f, 0f);

	public Transform blastStart;
	public Transform blastEnd;
    public List<GameObject> blasts;
    float blastDiss = 0f;

    public float jumpForce;
    public float moveForce;

    public float maxSpeed;
    public float lookSpeed;

    public float gravity;
    public float lowGravity;

    private float currentGravity;
    private float camDistance;

    private bool isGrounded = false;

	private bool firing = false;
	private float firingStart = 0f;

	public float fireDuration;

	private Vector3 blastDestination = Vector3.zero;

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
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam.transform.position = camPos.position;
        camDistance = Vector3.Distance(camPos.position, transform.position);
    }

    void Update()
    {
		blastEnd.position = blastDestination;
		if (firing && Time.time >= firingStart + fireDuration)
		{
			firing = false;
			blastDiss = 0f;
            foreach(var blast in blasts)
            {
                blast.GetComponent<SkinnedMeshRenderer>().material.SetFloat("dissolve", blastDiss);
            }
		}
		else if (firing)
		{
			blastEnd.position = blastDestination;

			//firing
			blastDiss = ((firingStart + fireDuration) - Time.time) / fireDuration;
            //blastDiss = Mathf.Lerp(blastDiss, 0f, Time.deltaTime + fireDuration * 0.05f);
            //blastDiss = Mathf.Lerp(blastDiss, 0, Time.deltaTime * ((firingStart + fireDuration) - Time.time));
            foreach (var blast in blasts)
            {
                blast.GetComponent<SkinnedMeshRenderer>().material.SetFloat("dissolve", blastDiss);
            }
        }
        else if (!firing)
        {
			blastDiss = 0f;
		}
		

		//Camera Movement
		cam.transform.position = camPos.position;
        cam.transform.LookAt(transform.position + lookOffset);
        camPos.LookAt(transform.position + lookOffset);

        //Input
        //Keys
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(transform.up * jumpForce);
            }
        }

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * moveForce);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * moveForce);
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * moveForce);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * moveForce);
        }

        //Mouse
        //Clicks
        if (Input.GetMouseButtonDown(0) && !firing)
        {
            RaycastHit hit;
            Vector3 heading = (transform.position + lookOffset) - camPos.position;
            Vector3 rayVect = heading / heading.magnitude;

            Ray r = new Ray(transform.position + lookOffset, rayVect);

            if (Physics.Raycast(r, out hit))
            {
				blastEnd.position = hit.point;
				blastDestination = hit.point;
				firing = true;
				firingStart = Time.time;
				blastDiss = 1f;
			}
        }

        //Mouse moves
        transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * Time.deltaTime * lookSpeed, 0f));

        float zMax = -1f;
        float yMin = -0.3f;
        float ySet = -0.4f;

        //Cam Y movement when "on ground"
        if (camPos.localPosition.y < yMin)
        {
            if (camPos.localPosition.z <= zMax && camPos.localPosition.z >= -camDistance)
            {
                camPos.localPosition = new Vector3(camPos.localPosition.x, ySet, camPos.localPosition.z + Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed / 3f);

                if (camPos.localPosition.z > zMax)
                {
                    camPos.localPosition = new Vector3(camPos.localPosition.x, ySet, zMax);
                }
            }
            else if (camPos.localPosition.z < -camDistance)
            {
                camPos.localPosition = new Vector3(camPos.localPosition.x, -0.2f, -camDistance);
            }
            else if (camPos.localPosition.z > zMax)
            {
                camPos.localPosition = new Vector3(camPos.localPosition.x, ySet, zMax);
            }

        }
        //Cam Y when in the air.
        else
        {
            camPos.transform.RotateAround(transform.position, transform.right, -Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed);
            if (camPos.localPosition.z > -1f)
            {
                camPos.localPosition = new Vector3(camPos.localPosition.x, camPos.localPosition.y, -1f);
            }
        }

        //Speed limit
        float yVel = rb.velocity.y;
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.AddForce(-rb.velocity * (0.1f * (rb.velocity.magnitude - maxSpeed)));
        }
        rb.velocity = new Vector3(rb.velocity.x, yVel, rb.velocity.z);

        //Set gravity
        if (Input.GetKey(KeyCode.Space))
        {
            currentGravity = lowGravity;
        }
        else
        {
            currentGravity = gravity;
        }
    }

    private void FixedUpdate()
    {
        //Add Gravity
        rb.AddForce(new Vector3(0f, -currentGravity, 0f));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            isGrounded = false;
        }
    }
}
