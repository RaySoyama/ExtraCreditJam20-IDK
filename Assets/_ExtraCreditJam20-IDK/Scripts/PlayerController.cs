using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    float fireRateTimeRemaining = 0f;
    float speedTimeRemaining = 0f;
    float instaKillTimeRemaining = 0f;

	private bool isGrounded = false;
	private bool isWalking = false;

	private bool firing = false;
	private bool waterFiring = false;
	private bool waterWasFiringLastFrame = false;
    private float firingStart = 0f;

    public float fireDuration;

    private Vector3 blastDestination = Vector3.zero;
    private Vector3 waterDestination = Vector3.zero;

    public Transform blastStart;
    public Transform blastEnd;
    public Transform waterStart;
    public Transform waterEnd;
    public List<SkinnedMeshRenderer> blasts;
    public List<SkinnedMeshRenderer> waterBlasts;
    public MeshRenderer waterFillMat;
    float blastDiss = 0f;
    float waterDiss = 0f;

    public float waterStorageMax = 5f;
    public float currentWaterStorage = 5f;

    public ParticleSystem epParticle;
    public ParticleSystem epWaterParticle;
    public ParticleSystem movementParticle;

    public GameObject powerup_FireRate;
    public GameObject powerup_SpeedBoost;
    public GameObject powerup_InstaKill;

    public Animator corsshair;

    public Animator PlayerAnim;
    float lRotate = 0;
    float rRotate = 0;

    public AudioClip jumpSound;
	public List<AudioClip> blastSounds;

	public AudioClip waterStartSound;
	public AudioClip waterLoopSound;
	public AudioClip waterStopSound;

	private float waterSoundStartTime = 0;

    private AudioSource audio;

	public GameObject miniMap;

    private List<GameObject> grounds = new List<GameObject>();
    private Vector3 currentShake;

    public UnityEvent onFillingWaterOrb;
    public UnityEvent notFillingWaterOrb;

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
    }

    void Update()
    {
        fireRateTimeRemaining -= Time.deltaTime;
        powerup_FireRate.SetActive(fireRateTimeRemaining > 0);
        speedTimeRemaining -= Time.deltaTime;
        powerup_SpeedBoost.SetActive(speedTimeRemaining > 0);
        instaKillTimeRemaining -= Time.deltaTime;
        powerup_InstaKill.SetActive(instaKillTimeRemaining > 0);

        fireDuration = fireRateTimeRemaining > 0 ? 0.1f : 1f;

        //Water sounds
        if (waterFiring && !waterWasFiringLastFrame)
		{
			//Start water sound
			waterSoundStartTime = Time.time;
			audio.PlayOneShot(waterStartSound);
		}
		else if (waterFiring && waterWasFiringLastFrame)
		{
			if (audio.clip == waterStartSound && waterSoundStartTime + waterStartSound.length >= Time.time)
			{
				audio.Stop();
				audio.clip = waterLoopSound;
				audio.loop = true;
			}
		}
		else if (!waterFiring && waterWasFiringLastFrame)
		{
			audio.Stop();
			audio.loop = false;
			audio.PlayOneShot(waterStopSound);
		}

		waterWasFiringLastFrame = waterFiring;

        PlayerAnim.SetBool("isRunning", isWalking);
        PlayerAnim.SetBool("isSpraying", waterFiring);

        PlayerAnim.SetLayerWeight(2, lRotate);
        PlayerAnim.SetLayerWeight(3, rRotate);

        if (isGrounded)
        {
            ParticleSystem.EmissionModule module = movementParticle.emission;
            module.rateOverDistance = 8;

            PlayerAnim.SetBool("isFalling", false);
        }
        else
        {
            ParticleSystem.EmissionModule module = movementParticle.emission;
            module.rateOverDistance = 0;

            PlayerAnim.SetBool("isFalling", true);
        }

        blastEnd.position = blastDestination;
        blastEnd.LookAt(transform.position);
        if (firing && Time.time >= firingStart + fireDuration)
        {
            firing = false;
            blastDiss = 0f;

            corsshair.ResetTrigger("shoot");
            corsshair.SetTrigger("stopShoot");

            PlayerAnim.ResetTrigger("attack");
            PlayerAnim.SetTrigger("stopAttack");

            foreach (var blast in blasts)
            {
                blast.material.SetFloat("dissolve", blastDiss);
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

            foreach (var blast in blasts)
            {
                blast.material.SetFloat("dissolve", blastDiss);
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
            water.material.SetFloat("dissolve", waterDiss);
        }

        waterFillMat.material.SetFloat("fill", (currentWaterStorage / waterStorageMax) + 0.55f);
		
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
            PlayerAnim.SetTrigger("jump");

            if (isGrounded)
            {
                rb.AddForce(transform.up * jumpForce);
                audio.PlayOneShot(jumpSound);
            }
        }

		isWalking = false;

        float speedAlteration = speedTimeRemaining > 0 ? 2.5f : 1f;

        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(transform.forward * speedAlteration * moveForce);
			isWalking = true;

            if(lRotate > .5)
            {
                lRotate = Mathf.Lerp(lRotate, 0.5f, Time.deltaTime * 10);
            }
            if (rRotate > .5)
            {
                rRotate = Mathf.Lerp(rRotate, 0.5f, Time.deltaTime * 10);
            }
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(-transform.forward * speedAlteration * moveForce);
			isWalking = true;            
        }

        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(-transform.right * speedAlteration * moveForce);
			isWalking = true;

            lRotate = Mathf.Lerp(lRotate, 1, Time.deltaTime * 2);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(transform.right * speedAlteration * moveForce);
			isWalking = true;

            rRotate = Mathf.Lerp(rRotate, 1, Time.deltaTime * 2);
        }

        if (!Input.GetKey(KeyCode.A))
        {
            lRotate = Mathf.Lerp(lRotate, 0, Time.deltaTime * 3);
        }

        if (!Input.GetKey(KeyCode.D))
        {
            rRotate = Mathf.Lerp(rRotate, 0, Time.deltaTime * 3);
        }

        //minimap
        if (Input.GetKeyDown(KeyCode.Tab))
		{
			miniMap.SetActive(!miniMap.activeSelf);
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

                corsshair.SetTrigger("shoot");

                PlayerAnim.SetTrigger("attack");

                shakeMagnitude += 1f;

                GameObject rootObj = hit.transform.root.gameObject;

                EnemyController enemyComp;
				Bell bell;
                if (rootObj.TryGetComponent<EnemyController>(out enemyComp))
                {
                    enemyComp.TakeHit(instaKillTimeRemaining > 0 ? 999 : 1);
					shakeMagnitude += 1f;
				}
				else if (hit.transform.TryGetComponent<Bell>(out bell))
				{
					bell.TakeHit();
					shakeMagnitude += 0.5f;
				}

				audio.PlayOneShot(blastSounds[Random.Range((int)0, (int)blastSounds.Count)]);
            }
        }
        else if (Input.GetMouseButton(1))
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
                    notFillingWaterOrb.Invoke();
                }
                else
                {
                    if (currentWaterStorage < waterStorageMax && hit.transform.gameObject.name == "Water")
                    {
                        waterDiss = Mathf.Lerp(waterDiss, 1, 0.3f);
                        shakeMagnitude += Time.deltaTime * 1.5f;
                        currentWaterStorage = Mathf.Clamp(currentWaterStorage + (Time.deltaTime * 2f), 0, waterStorageMax);
                        rb.AddForce(camPos.forward * 15, ForceMode.Acceleration);
                        onFillingWaterOrb.Invoke();
                    }
                    else
                    {
                        if (hit.transform.gameObject.name != "Water" && currentWaterStorage > 0)
                        {
                            waterDiss = Mathf.Lerp(waterDiss, 1, 0.15f);
                            currentWaterStorage = Mathf.Clamp(currentWaterStorage - Time.deltaTime * 3.5f, 0, waterStorageMax);

                            //Make player fly around
                            rb.AddForce(-camPos.forward * 75, ForceMode.Acceleration);
                            notFillingWaterOrb.Invoke();


                            var em = epWaterParticle.emission;
                            em.enabled = true;

                        }
                        else
                        {
                            waterFiring = false;
                            var em = epWaterParticle.emission;
                            em.enabled = false;
                            waterDiss = Mathf.Lerp(waterDiss, 0, 0.6f);
                            notFillingWaterOrb.Invoke();
                        }
                    }
                }
            }
        }
        else
        {
            notFillingWaterOrb.Invoke();
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Contains("Pickup_FireRate"))
        {
            fireRateTimeRemaining = 12f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Pickup_HealRandomDamagedPylon"))
        {
            //Heal pylon
            PylonManager.instance.HealLowestPylon(5f);
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Pickup_InstaKill"))
        {
            instaKillTimeRemaining = 12f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Pickup_SpeedBoost"))
        {
            speedTimeRemaining = 12f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.name.Contains("Pickup_WaterCapacity"))
        {
            waterStorageMax += 10f;
            Destroy(other.gameObject);
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
