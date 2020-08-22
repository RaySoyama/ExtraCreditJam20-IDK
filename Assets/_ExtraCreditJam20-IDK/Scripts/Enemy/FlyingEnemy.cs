using UnityEngine;

public class FlyingEnemy : EnemyController
{


    void Start()
    {
        OnSpawnStart();
        enemyState = State.SPAWN;
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
    }
    protected override void OnSearch()
    {
        base.OnSearch();
        //move to target Jank edition
        transform.LookAt(TargetPylon.transform.position);
        transform.Translate(Vector3.forward * EnemySpeed * Time.deltaTime);

        //distance check. Will not work if the enemy is too close
        if (Vector3.Distance(transform.position, TargetPylon.transform.position) < EnemyAttackRange)
        {
            OnSearchEnd();
        }
    }
    protected override void OnSearchEnd()
    {
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
            OnEnemyAttack?.Invoke();

            //Do damage
            Debug.Log("Enemy Attack");
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
    }
    protected override void OnDeathEnd()
    {
        base.OnDeathEnd();
    }

}
