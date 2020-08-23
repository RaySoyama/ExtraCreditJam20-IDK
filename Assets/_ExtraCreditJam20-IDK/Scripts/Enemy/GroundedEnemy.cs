using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : EnemyController
{
    [SerializeField]
    private Weapon laserWeapon = null;


    [SerializeField] private List<Rigidbody> debris = new List<Rigidbody>();
    public float debrisIntensity = 1000;
    [SerializeField] private float debrisDuration = 5.0f;
    [SerializeField] private bool debrisIsReadyToDestroy = false;
    [SerializeField, ReadOnlyField] private float debrisTime = 0.0f;

    [SerializeField] private AnimationEventCallback animationEvent;
    [SerializeField] private Animator animCtrl;
    [SerializeField] private UnityEngine.AI.NavMeshAgent navAgent;

    void Start()
    {
        OnSpawnStart();
        enemyState = State.SPAWN;
        animationEvent.EventCall += Blast;
        animationEvent.EventCall += Explode;
    }

    void Update()
    {
        switch (enemyState)
        {
            case State.IDLE:
                break;
            case State.SPAWN:
                OnSpawn();
                break;
            case State.SEARCH:
                OnSearch();
                break;
            case State.ATTACK:
                OnAttack();
                break;
            case State.DEATH:
                OnDeath();
                break;
        }
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        OnEnemySpawn?.Invoke();
    }

    protected override void OnSpawn()
    {
        base.OnSpawn();
        //once animation finished
        OnSpawnEnd();
    }

    protected override void OnSpawnEnd()
    {
        base.OnSpawnEnd();
    }

    protected override void OnSearchStart()
    {
        base.OnSearchStart();
        animCtrl.ResetTrigger("shoot");
        animCtrl.SetTrigger("stopShoot");

        if (TargetPylon != null)
        {
            navAgent.destination = TargetPylon.shootTarget.position;
        }
    }

    protected override void OnSearch()
    {
        base.OnSearch();

        if (TargetPylon == null || TargetPylon.PylonIsDestroyed == true)
        {
            OnSearchStart();
            return;
        }

        //move to target Good edition
        if (Vector3.Distance(transform.position, TargetPylon.shootTarget.position) <= navAgent.stoppingDistance + EnemyAttackRange)
        {
            navAgent.isStopped = true;
            OnSearchEnd();
        }
        else
        {
            navAgent.isStopped = false;
        }
    }

    protected override void OnSearchEnd()
    {
        transform.LookAt(TargetPylon.shootTarget.position);
        base.OnSearchEnd();
    }

    protected override void OnAttackStart()
    {
        base.OnAttackStart();
    }

    protected override void OnAttack()
    {
        base.OnAttack();

        //"Attack"
        timeToNextAttack -= Time.deltaTime;

        if (TimeToNextAttack <= 0.0f)
        {
            if (TargetPylon == null || TargetPylon.PylonIsDestroyed == true)
            {
                OnSearchStart();
                return;
            }

            animCtrl.SetTrigger("shoot");
            timeToNextAttack = 1.0f / EnemyAttackSpeed;
        }
    }

    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
    }

    [ContextMenu("Kill")]
    protected override void OnDeathStart()
    {
        base.OnDeathStart();
        animCtrl.SetTrigger("die");
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        debrisTime += Time.deltaTime;

        if (debrisTime >= debrisDuration && debrisIsReadyToDestroy == false)
        {
            debrisTime = 0;
            debrisIsReadyToDestroy = true;
        }

        //Lerp Scale down
        if (debrisIsReadyToDestroy == true)
        {
            foreach (Rigidbody piece in debris)
            {
                piece.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, debrisTime);
            }
        }

        if (debrisTime >= 1.0f && debrisIsReadyToDestroy == true)
        {
            OnDeathEnd();
        }

    }

    protected override void OnDeathEnd()
    {
        base.OnDeathEnd();
        Destroy(gameObject);
    }

    private void Blast(string eventName)
    {
        if (eventName != "Blast")
        {
            return;
        }

        OnEnemyAttack?.Invoke();

        //Do damage
        if (TargetPylon != null)
        {
            TargetPylon.TakeDamage(EnemyDamage);
        }
        else
        {
            Debug.LogError("Missing reference to Target Pylon");
        }


        //Show Laser
        if (laserWeapon != null)
        {
            laserWeapon.shotLength = EnemyAttackRange;
            laserWeapon.Fire();
        }
        else
        {
            Debug.LogError("Missing reference to Laser Weapon");
        }

        //play audio
        //audioSrc.PlayOneShot(attackSound);

    }

    private void Explode(string eventName)
    {
        if (eventName != "Explode")
        {
            return;
        }

        foreach (Rigidbody piece in debris)
        {
            piece.gameObject.SetActive(true);
            piece.AddForce(new Vector3(Random.Range(-debrisIntensity, debrisIntensity), Random.Range(0, debrisIntensity), Random.Range(-debrisIntensity, debrisIntensity)));
        }
    }

    private void OnDestroy()
    {
        animationEvent.EventCall -= Blast;
        animationEvent.EventCall -= Explode;
    }
}
