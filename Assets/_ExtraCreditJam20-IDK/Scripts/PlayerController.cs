using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	private Camera cam;
	public Transform camPos;

	private Rigidbody rb;

	public float jumpForce;
	public float moveForce;

	public float maxSpeed;
	public float lookSpeed;

	public float gravity;
	public float lowGravity;

	private float currentGravity;
	private float camDistance;

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
		cam.transform.LookAt(transform.position);
		camPos.LookAt(transform.position);

		//Input
		//Keys
		if (Input.GetKeyDown(KeyCode.Space))
		{
			rb.AddForce(transform.up * jumpForce);
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
		transform.Rotate(new Vector3(0f, Input.GetAxis("Mouse X") * Time.deltaTime * lookSpeed, 0f));

		//float newY = camPos.position.y - (Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed * 0.2f);

		//newY = Mathf.Clamp(newY, 0.1f, 10f);
		//camPos.transform.position = new Vector3(camPos.position.x, newY, camPos.position.z);

		if (camPos.position.y < 0.1f)
		{

			camPos.position = new Vector3(camPos.position.x, 0.08f, camPos.position.z);

			if (camPos.position.z < -0.1f && camPos.position.z > -camDistance)
			{
				camPos.position = new Vector3(camPos.position.x, 0.08f, camPos.position.z + Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed);
			}
			else if (camPos.position.z <= -camDistance)
			{
				camPos.position = new Vector3(camPos.position.x, 0.1f, -camDistance);
			}
			else if (camPos.position.z >= -0.1f)
			{
				Debug.Log("yo");
				camPos.position = new Vector3(camPos.position.x, 0.08f, -0.1f);
			}

		}
		else
		{
			camPos.transform.RotateAround(transform.position, transform.right, -Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed);
		}

		//Speed limit
		if (rb.velocity.magnitude > maxSpeed)
		{
			rb.AddForce(-rb.velocity * (0.1f * (rb.velocity.magnitude - maxSpeed)));
		}

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
}
