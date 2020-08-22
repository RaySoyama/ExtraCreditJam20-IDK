using UnityEngine;

public class FlyingEnemy : EnemyController
{
    [SerializeField]
    private Weapon laserWeapon = null;

    [SerializeField]
    private AnimationEventCallback animationEvent;

    [SerializeField]
    private Animator animCtrl;
    void Start()
    {
        OnSpawnStart();
        enemyState = State.SPAWN;
        animationEvent.EventCall += Blast;
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
        unitSphereVal = Random.insideUnitSphere.normalized * EnemyAttackRange;

        if (unitSphereVal.y <= 0)
        {
            unitSphereVal = new Vector3(unitSphereVal.x, -unitSphereVal.y, unitSphereVal.z);
        }
    }
    protected override void OnSearch()
    {
        if (TargetPylon == null || TargetPylon.PylonIsDestroyed == true)
        {
            OnSearchStart();
            return;
        }

        base.OnSearch();
        //move to target Jank edition
        transform.LookAt(TargetPylon.shootTarget.position + unitSphereVal);
        transform.Translate(Vector3.forward * EnemySpeed * Time.deltaTime);

        //distance check. Will not work if the enemy is too close
        if (Vector3.Distance(transform.position, TargetPylon.shootTarget.position + unitSphereVal) <= 0.1f)
        {
            OnSearchEnd();
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
        if (TargetPylon == null || TargetPylon.PylonIsDestroyed == true)
        {
            OnSearchStart();
            return;
        }

        base.OnAttack();

        //"Attack"
        timeToNextAttack -= Time.deltaTime;

        if (TimeToNextAttack <= 0.0f)
        {
            animCtrl.SetTrigger("shoot");
            timeToNextAttack = 1.0f / EnemyAttackSpeed;
        }

    }
    protected override void OnAttackEnd()
    {
        base.OnAttackEnd();
    }


    protected override void OnDeathStart()
    {
        base.OnDeathStart();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        OnDeathEnd();
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

    }

    private void OnDestroy()
    {
        animationEvent.EventCall -= Blast;
    }
}
