using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : NetworkBehaviour
{
    public NavMeshAgent agent;

    [SyncVar]
    public Transform player;
    [SyncVar]
    public int spawnerId;

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

    private Animator anim;

    public enum abilities
    {
        Earth,
        Fire,
        Air,
        Water
    }

    private void Start()
    {
        if (!isServer)
        {
            return;
        }
        GetComponent<NavMeshAgent>().enabled = true;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();
        anim = GetComponentInChildren<Animator>();


        health.OnDeath.AddListener(DED);

        GameManager.Instance.enemies.Add(gameObject);
    }

    private void Update()
    {
        if (!isServer)
        {
            if (transform.parent == null)
            {
                GameObject go = GameObject.FindGameObjectsWithTag("Spawner").First(x => x.GetComponent<Spawner>().id == spawnerId);
                if(go)
                {
                    transform.SetParent(go.transform);
                }
            }
            return;
        }

        isInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(isInSight)
        {
            CmdFindClosestPlayer();
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
        if (!isInSight && !isInAttackRange) CmdPatroling();
        if (isInSight && !isInAttackRange) CmdChasePlayer();
        if (isInSight && isInAttackRange) CmdAttackPlayer();
    }

    [Command(requiresAuthority = false)]
    private void CmdFindClosestPlayer()
    {
        Transform p = ElemNetworkManager.playerObjs[0].transform;

        for (int i = 0; i < ElemNetworkManager.playerObjs.Count; i++)
        {
            if(Mathf.Abs(Vector3.Distance(transform.position, p.position)) < Mathf.Abs(Vector3.Distance(transform.position, ElemNetworkManager.playerObjs[i].transform.position)))
            {
                p = ElemNetworkManager.playerObjs[0].transform;
            }
        }

        player =  p;
    }

    [Command(requiresAuthority = false)]
    private void CmdPatroling()
    {
        if (!walkPointSet) CmdCallSearch();

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
            anim.SetFloat("Movement", agent.speed);
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
        CmdSearch();
    }

    [Command(requiresAuthority = false)]
    private void CmdSearch()
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

    [Command(requiresAuthority = false)]
    private void CmdChasePlayer()
    {
        if (player == null || health.GetHealth() <= 0) return;
        agent.SetDestination(player.position);
    }

    [Command(requiresAuthority = false)]
    private void RotateTowards(Transform target)
    {
        if (target == null) return;
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));    // flattens the vector3
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
    }

    [Command(requiresAuthority = false)]
    private void CmdRunRotTowards(Transform target)
    {
        RotateTowards(target);
    }

    [Command(requiresAuthority = false)]
    private void CmdAttackPlayer()
    {
        if (player == null)
        {
            print("No player" + gameObject.name);
            return;
        }

        //agent.SetDestination(transform.position);
        CmdRunRotTowards(player);

        if(!hasAttacked)
        {
            switch (eAbilities)
            {
                case abilities.Earth:
                    GameObject rock = Instantiate(ROCKBALL, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

                    rock.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), false);
                    break;
                case abilities.Fire:
                    GameObject fireball = Instantiate(FIREBALL, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

                    fireball.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), false);
                    break;
                case abilities.Air:
                    GameObject airBall = Instantiate(AIRBALL, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

                    airBall.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), false);
                    break;
                case abilities.Water:
                    GameObject whip = Instantiate(WATERBALL, transform.position + (-transform.up * 2) + (transform.forward * 4), transform.rotation) as GameObject;

                    whip.GetComponent<BaseAbility>().AbilityInitial(speed, transform.position + (transform.up) + (transform.forward * 4), false);
                    break;
                default:
                    break;
            }

            anim.SetTrigger("Attack");

            hasAttacked = true;
            Invoke(nameof(CallResetAttack), timeBetweenAttacks);
        }
    }

    [Command(requiresAuthority = false)]
    private void CallResetAttack()
    {
        CmdResetAttack();
    }

    [Command(requiresAuthority = false)]
    private void CmdResetAttack()
    {
        hasAttacked = false;
    }

    [ClientRpc]
    private void DED()
    {
        int i = Random.Range(1, 3);
        GetComponentInParent<Spawner>().spawnEnemy = false;
        anim.SetTrigger("Death" + i);
        anim.SetBool("IsAlive", false);
        agent.enabled = false;

        GameManager.Instance.RemoveEnemyFromList(gameObject);
        Destroy(gameObject, 2);
    }
}
