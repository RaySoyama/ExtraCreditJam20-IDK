using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance = null;

    private Camera cam;
    public Transform camPos;

    private Rigidbody rb;

    private Vector3 lookOffset = new Vector3(0f, 4f, 0f);

    public float jumpForce;
    public float moveForce;

    public float maxSpeed;
    public float lookSpeed;

    public float gravity;
    public float lowGravity;

    private float currentGravity;
    private float camDistance;

	private float shakeMagnitude = 0f;


	private bool isGrounded = false;
	private bool isWalking = false;

	private bool firing = false;
	private bool waterFiring = false;
    private float firingStart = 0f;

    public float fireDuration;

    private Vector3 blastDestination = Vector3.zero;
    private Vector3 waterDestination = Vector3.zero;

    public Transform blastStart;
    public Transform blastEnd;
    public Transform waterStart;
    public Transform waterEnd;
    public List<GameObject> blasts;
    public List<GameObject> waterBlasts;
    float blastDiss = 0f;
    float waterDiss = 0f;

    public float waterStorageMax = 5f;
    public float currentWaterStorage = 5f;

    public ParticleSystem epParticle;

    public Animator corsshair;

    public AudioClip jumpSound;
	public List<AudioClip> blastSounds;
    private AudioSource audio;

	public GameObject miniMap;

    private List<GameObject> grounds = new List<GameObject>();
    private Vector3 currentShake;

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

        audio = GetComponent<AudioSource>();

		Debug.Log("kys");
    }

    void Update()
    {
        blastEnd.position = blastDestination;
        blastEnd.LookAt(transform.position);
        if (firing && Time.time >= firingStart + fireDuration)
        {
            firing = false;
            blastDiss = 0f;

            corsshair.ResetTrigger("shoot");
            corsshair.SetTrigger("stopShoot");

            foreach (var blast in blasts)
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
            var em = epParticle.emission;
            em.enabled = true;

            corsshair.SetTrigger("shoot");

            foreach (var blast in blasts)
            {
                blast.GetComponent<SkinnedMeshRenderer>().material.SetFloat("dissolve", blastDiss);
            }
        }
        else if (!firing)
        {
            blastDiss = 0f;
            var em = epParticle.emission;
            em.enabled = false;
        }

        foreach (var water in waterBlasts)
        {
            water.GetComponent<SkinnedMeshRenderer>().material.SetFloat("dissolve", waterDiss);
        }

		//Camera Movement
		Vector3 shake = Vector3.zero;
		if (shakeMagnitude > 0f)
		{
			shake = new Vector3(Random.Range(-0.1f, 0.1f) * shakeMagnitude, Random.Range(-0.1f, 0.1f) * shakeMagnitude, Random.Range(-0.1f, 0.1f) * shakeMagnitude);
			shakeMagnitude -= 0.05f;
		}
		else if (shakeMagnitude < 0f)
		{
			shakeMagnitude = 0f;
		}

        currentShake = Vector3.Lerp(currentShake, shake * 2, shakeMagnitude * 0.4f);

		cam.transform.position = camPos.position + currentShake;
        cam.transform.LookAt(transform.position + lookOffset);
        camPos.LookAt(transform.position + lookOffset);

        //Input
        //Keys
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.AddForce(transform.up * jumpForce);
                audio.PlayOneShot(jumpSound);
            }
        }

		isWalking = false;

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * moveForce);
			isWalking = true;
		}
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * moveForce);
			isWalking = true;
		}

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * moveForce);
			isWalking = true;
		}
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * moveForce);
			isWalking = true;
		}

		//minimap
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			miniMap.SetActive(!miniMap.active);
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

				shakeMagnitude += 1f;

                GameObject rootObj = hit.transform.root.gameObject;

                EnemyController enemyComp;
                if (rootObj.TryGetComponent<EnemyController>(out enemyComp))
                {
                    enemyComp.TakeHit();
					shakeMagnitude += 1f;
				}

                try
                {
                    audio.PlayOneShot(blastSounds[Random.Range((int)0, (int)blastSounds.Count)]);
                }
                catch
                {

                }
            }
        }

        if (Input.GetMouseButton(1))
        {
            RaycastHit hit;
            Vector3 heading = (transform.position + lookOffset) - camPos.position;
            Vector3 rayVect = heading / heading.magnitude;

            Ray r = new Ray(transform.position + lookOffset, rayVect);

            if (Physics.Raycast(r, out hit))
            {
                waterFiring = true;

                waterEnd.position = hit.point;
                waterDestination = hit.point;

                shakeMagnitude += Time.deltaTime * 1.1f;

                GameObject rootObj = hit.transform.root.gameObject;

                PylonController pylonOut;
                if (currentWaterStorage > 0 && rootObj.TryGetComponent<PylonController>(out pylonOut))
                {
                    //DO HEALING HERE!!!!!!!!!!!!
                    //pylonOut.Heal();
                    waterDiss = Mathf.Lerp(waterDiss, 1, 0.2f);
                    shakeMagnitude += 0.1f;
                    currentWaterStorage = Mathf.Clamp(currentWaterStorage - (Time.deltaTime * 4), 0, waterStorageMax);
                }
                else
                {
                    if (currentWaterStorage < waterStorageMax && hit.transform.gameObject.name == "Water")
                    {
                        waterDiss = Mathf.Lerp(waterDiss, 1, 0.3f);
                        shakeMagnitude += Time.deltaTime * 1.5f;
                        currentWaterStorage = Mathf.Clamp(currentWaterStorage + (Time.deltaTime * 1.5f), 0, waterStorageMax);
                    }
                    else
                    {
                        if (hit.transform.gameObject.name != "Water" && currentWaterStorage > 0)
                        {
                            waterDiss = Mathf.Lerp(waterDiss, 1, 0.15f);
                            currentWaterStorage = Mathf.Clamp(currentWaterStorage - Time.deltaTime * 2f, 0, waterStorageMax);

                            //Make player fly around
                            rb.AddForce(-camPos.forward * 75, ForceMode.Acceleration);
                        }
                        else
                        {
                            waterFiring = false;
                            waterDiss = Mathf.Lerp(waterDiss, 0, 0.6f);
                        }
                    }
                }
            }
        }
        else
        {
            waterFiring = false;
            waterDiss = Mathf.Lerp(waterDiss, 0, 0.06f);
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
                camPos.localPosition = new Vector3(camPos.localPosition.x, ySet, camPos.localPosition.z + Input.GetAxis("Mouse Y") * Time.deltaTime * lookSpeed / 5f);

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
            if (camPos.localPosition.z > -3f)
            {
                camPos.localPosition = new Vector3(camPos.localPosition.x, camPos.localPosition.y, -3f);
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
		if (!isGrounded)
		{
			rb.AddForce(new Vector3(0f, -currentGravity, 0f));
		}
		else
		{
			if (isWalking)
			{
				rb.AddForce(new Vector3(0f, -currentGravity * 0.25f, 0f));
			}
			else
			{
				rb.AddForce(new Vector3(0f, -currentGravity * 2f, 0f));
			}
		}
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            grounds.Add(collision.gameObject);
        }

        if (grounds.Count > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "ground")
        {
            if (grounds.Contains(collision.gameObject))
            {
                grounds.Remove(collision.gameObject);
            }
        }

        if (grounds.Count > 0)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}
