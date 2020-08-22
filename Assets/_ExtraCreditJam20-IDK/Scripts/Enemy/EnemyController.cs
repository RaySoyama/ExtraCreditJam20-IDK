using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    protected State enemyState = State.IDLE;
    public State EnemyState
    {
        get
        {
            return enemyState;
        }
    }


    [SerializeField]
    protected string enemyName = "DefaultName";
    public string EnemyName
    {
        get
        {
            return enemyName;
        }
    }

    [SerializeField]
    protected float enemySpeed = 1;
    public float EnemySpeed
    {
        get
        {
            return enemySpeed;
        }
    }

    [SerializeField]
    protected float enemyStartHealth = 1;
    public float EnemyStartHealth
    {
        get
        {
            return enemyStartHealth;
        }
    }

    [SerializeField]
    protected float enemyHealth = 1;
    public float EnemyHealth
    {
        get
        {
            return enemyHealth;
        }
    }

    [SerializeField]
    protected float enemyDamage = 1;
    public float EnemyDamage
    {
        get
        {
            return enemyDamage;
        }
    }

    [SerializeField, ReadOnlyField]
    protected float timeToNextAttack = 0;
    public float TimeToNextAttack
    {
        get
        {
            return timeToNextAttack;
        }
    }

    [SerializeField]
    protected float enemyAttackSpeed = 1;
    public float EnemyAttackSpeed
    {
        get
        {
            return enemyAttackSpeed;
        }
    }

    [SerializeField]
    protected float enemyAttackRange = 10;
    public float EnemyAttackRange
    {
        get
        {
            return enemyAttackRange;
        }
    }

    [SerializeField, ReadOnlyField]
    protected PylonController targetPylon = null;
    public PylonController TargetPylon
    {
        get
        {
            return targetPylon;
        }
    }

    protected Vector3 unitSphereVal;



    //Don't forget to unsub to the events
    public delegate void EnemyEvent();
    public EnemyEvent OnEnemySpawn;
    public EnemyEvent OnEnemyAttack;
    public EnemyEvent OnEnemyHit;
    public EnemyEvent OnEnemyDeath;


    public enum State
    {
        IDLE,
        SPAWN,
        SEARCH,
        ATTACK,
        DEATH
    }


    protected virtual void OnSpawnStart()
    {
        enemyState = State.SPAWN;
    }
    protected virtual void OnSpawn() { }
    protected virtual void OnSpawnEnd()
    {
        OnSearchStart();
    }


    protected virtual void OnSearchStart()
    {
        enemyState = State.SEARCH;

        //if no pylons
        if (PylonManager.instance.AllActivePylons.Count <= 0)
        {
            OnDeathStart();
            return;
        }

        //Find closest pylon
        float shortestDistance = float.MaxValue;
        PylonController closestPylon = null;

        foreach (PylonController pylon in PylonManager.instance.AllActivePylons)
        {
            if (Vector3.Distance(transform.position, pylon.transform.position) < shortestDistance)
            {
                shortestDistance = Vector3.Distance(transform.position, pylon.transform.position);
                closestPylon = pylon;
            }
        }

        if (closestPylon == null)
        {
            //tf do i do
            Debug.LogError("WTF NO PYLONS");
        }
        else
        {
            targetPylon = closestPylon;
        }
    }
    protected virtual void OnSearch() { }
    protected virtual void OnSearchEnd()
    {
        OnAttackStart();
    }

    protected virtual void OnAttackStart()
    {
        enemyState = State.ATTACK;
    }
    protected virtual void OnAttack() { }
    protected virtual void OnAttackEnd() { }

    protected virtual void OnDeathStart()
    {
        enemyState = State.DEATH;
    }
    protected virtual void OnDeath() { }
    protected virtual void OnDeathEnd() { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, EnemyAttackRange);

        if (EnemyState == State.SEARCH)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(TargetPylon.transform.position + unitSphereVal, 0.5f);
        }
    }
}
