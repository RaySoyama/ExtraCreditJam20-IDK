using UnityEngine;

public class EnemyController : MonoBehaviour
{
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
            return EnemySpeed;
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

    [SerializeField]
    protected float attackSpeed = 1;
    public float AttackSpeed
    {
        get
        {
            return attackSpeed;
        }
    }




    public delegate void EnemyEvent();
    public EnemyEvent OnEnemyAttack;
    public EnemyEvent OnEnemyHit;
    public EnemyEvent OnEnemyDeath;

    protected virtual void OnSpawn()
    {

    }

    protected virtual void OnSearch()
    {

    }

    protected virtual void OnAttack()
    {

    }

    protected virtual void OnDeath()
    {

    }

}
