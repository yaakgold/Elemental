using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : NetworkBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public GameObject FIREBALL;
    public GameObject AIRBALL;
    public GameObject WATERBALL;
    public GameObject ROCKBALL;

    public float speed;

    public Health health;

    public LayerMask groundLayer;
    public LayerMask playerLayer;

    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    public float timeBetweenAttacks;
    bool hasAttacked;

    public float sightRange;
    public float attackRange;
    public bool isInSight;
    public bool isInAttackRange;

    public abilities eAbilities;

    public enum abilities
    {
        Earth,
        Fire,
        Air,
        Water
    }

    private void Start()
    {
        if (!isServer) return;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        health.OnDeath.AddListener(DED);

        NetworkServer.Spawn(gameObject);
    }

    private void Update()
    {
        if (!isServer) return;

        isInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(isInSight)
        {
            player = FindClosestPlayer();
        }
        else
        {
            player = null;
        }

        CmdStateMachine();
    }

    [Command(requiresAuthority = false)]
    private void CmdStateMachine()
    {
        if (!isInSight && !isInAttackRange) Patroling();
        if (isInSight && !isInAttackRange) ChasePlayer();
        if (isInSight && isInAttackRange) AttackPlayer();
    }

    private Transform FindClosestPlayer()
    {
        Transform p = ElemNetworkManager.playerObjs[0].transform;

        for (int i = 0; i < ElemNetworkManager.playerObjs.Count; i++)
        {
            if(Mathf.Abs(Vector3.Distance(transform.position, p.position)) < Mathf.Abs(Vector3.Distance(transform.position, ElemNetworkManager.playerObjs[i].transform.position)))
            {
                p = ElemNetworkManager.playerObjs[0].transform;
            }
        }

        return p;
    }

    [ClientRpc]
    private void Patroling()
    {
        if (!walkPointSet) CmdCallSearch();

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }

        Vector3 distance = transform.position - walkPoint;

        if(distance.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdCallSearch()
    {
        Search();
    }

    [ClientRpc]
    private void Search()
    {
        agent.updateRotation = true;
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    [ClientRpc]
    private void ChasePlayer()
    {
        if (player == null) return;
        agent.SetDestination(player.position);
    }

    [ClientRpc]
    private void RotateTowards(Transform target)
    {
        if (target == null) return;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
    }

    [ClientRpc]
    private void AttackPlayer()
    {
        if (player == null)
        {
            print("No player" + gameObject.name);
            return;
        }

        //agent.SetDestination(transform.position);
        RotateTowards(player);

        if(!hasAttacked)
        {
            switch (eAbilities)
            {
                case abilities.Earth:
                    GameObject rock = Instantiate(ROCKBALL, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

                    rock.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
                    break;
                case abilities.Fire:
                    GameObject fireball = Instantiate(FIREBALL, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

                    fireball.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
                    break;
                case abilities.Air:
                    GameObject airBall = Instantiate(AIRBALL, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

                    airBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
                    break;
                case abilities.Water:
                    GameObject whip = Instantiate(WATERBALL, transform.position + (-transform.up * 2) + (transform.forward * 2), transform.rotation) as GameObject;

                    whip.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 2));
                    break;
                default:
                    break;
            }

            hasAttacked = true;
            Invoke(nameof(CallResetAttack), timeBetweenAttacks);
        }
    }

    [Command(requiresAuthority = false)]
    private void CallResetAttack()
    {
        ResetAttack();
    }

    [ClientRpc]
    private void ResetAttack()
    {
        hasAttacked = false;
    }

    [ClientRpc]
    private void DED()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawSphere(transform.position, sightRange);

        Gizmos.color = new Color(0, 1, 0, .5f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
}
