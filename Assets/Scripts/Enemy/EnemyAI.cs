using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyAI : MonoBehaviour
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
        player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Health>();

        health.OnDeath.AddListener(DED);
    }

    private void Update()
    {
        isInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        isInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!isInSight && !isInAttackRange) Patroling();
        if (isInSight && !isInAttackRange) ChasePlayer();
        if (isInSight && isInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        if (!walkPointSet) Search();

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

    private void Search()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, groundLayer))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

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
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    private void DED()
    {
        Destroy(gameObject);
    }

}
