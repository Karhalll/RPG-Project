using UnityEngine;
using UnityEngine.AI;

using GameDevTV.Utils;

using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float chaseSpeed = 4f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float agroCooldownTime = 6f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float patrolSpeed = 2f;
        [SerializeField] float dwellingTime = 3f;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float shoutDistance = 5f;

        GameObject player;
        Health health;
        Fighter fighter;
        Mover mover;
        NavMeshAgent navMeshAgent;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceStartDwelling = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        int curretWaypointIndex = 0;

        private void Awake() 
        {
            fighter = GetComponent<Fighter>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start() 
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead()) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceStartDwelling += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceStartDwelling = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceStartDwelling > dwellingTime)
            {
                navMeshAgent.speed = patrolSpeed;
                mover.StartMoveAction(nextPosition);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            curretWaypointIndex = patrolPath.GetNextIndex(curretWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(curretWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            navMeshAgent.speed = chaseSpeed;
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);

            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;
                
                ai.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return distanceToPlayer <= chaseDistance || timeSinceAggrevated < agroCooldownTime;
        }

        // Called by Unity
        private void OnDrawGizmosSelected() 
        {
           Gizmos.color = Color.blue;
           Gizmos.DrawWireSphere(transform.position, chaseDistance);  
        }
    }
}
