using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Camera cam;
	public Transform camPos;

	private Rigidbody rb;

	private Vector3 lookOffset = new Vector3(0f, 2.5f, 0f);

	public float jumpForce;
	public float moveForce;

	public float maxSpeed;
	public float lookSpeed;

	public float gravity;
	public float lowGravity;

	private float currentGravity;
	private float camDistance;

	private bool isGrounded = false;

    // Start is called before the first frame update
    void Start()
    {
		cam = Camera.main;
		rb = GetComponent<Rigidbody>();

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		cam.transform.position = camPos.position;
		camDistance = Vector3.Distance(camPos.position, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
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
		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			Vector3 heading = (transform.position + lookOffset) - camPos.position;
			Vector3 rayVect = heading / heading.magnitude;

			if (Physics.Raycast(transform.position + lookOffset, rayVect, out hit))
			{
				Debug.Log(hit.distance);
			}
			Debug.DrawRay(transform.position + lookOffset, rayVect);
		}

		//REMOVE FOR DEBUG
		if (Input.GetMouseButton(0))
		{
			RaycastHit hit;
			Vector3 heading = (transform.position + lookOffset) - camPos.position;
			Vector3 rayVect = heading / heading.magnitude;
			Debug.DrawRay(transform.position, rayVect, Color.red, 1f, true);
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
