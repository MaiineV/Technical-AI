using System;
using AI;
using UnityEngine;

namespace Enemy
{
    public class Enemy : Agent
    {
        private void Update()
        {
            FsmComponent.OnUpdate();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, SearchRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
        }
    }
}
