using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class EnemyController : NetworkBehaviour
{
    private NavMeshAgent navMesh = null;
    private Animator animation = null;
    private EnemyStats enemyStats = null;
    [SerializeField] private float stoppingDistance;
    private Transform target;
    private float timeOfLastAttack = 0;
    private bool hasStopped = false;

    private void Start()
    {
        GetReferences();
    }

    private void Update()
    {
        if (!isServer) return;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        navMesh.SetDestination(target.position);

        float distanceToTarget = Vector3.Distance(target.position, transform.position);

        animation.SetFloat("Speed", 1f, 0.3f, Time.deltaTime);

        RotateToTarget();

        if (distanceToTarget <= navMesh.stoppingDistance)
        {
            animation.SetFloat("Speed", 0f);
            if (!hasStopped)
            {
                hasStopped = true;
                timeOfLastAttack = Time.time;
            }

            

            if(Time.time >= timeOfLastAttack + enemyStats.attackSpeed)
            {
                timeOfLastAttack = Time.time;
                var targetStats = target.GetComponent<CharacterStats>();
                if (targetStats != null)
                {
                    AttackTarget(targetStats);
                }
            }
        }
        else
        {
            if (hasStopped)
            {
                hasStopped = false;
            }
        }
    }

    private void RotateToTarget()
    {
        Vector3 direction = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = rotation;
    }

    private void AttackTarget(CharacterStats damageStats)
    {
        animation.SetTrigger("attack");
        enemyStats.DealDamage(damageStats);
    }

    private void GetReferences()
    {
        navMesh = GetComponent<NavMeshAgent>();
        animation = GetComponentInChildren<Animator>();
        enemyStats = GetComponent<EnemyStats>();

        //TODO: change to overlaps sphere
        target = RigidBodyMovement.instance;
    }
}
