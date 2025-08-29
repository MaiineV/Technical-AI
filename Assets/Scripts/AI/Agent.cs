using System;
using UnityEngine;
using AI.FSM;

namespace AI
{
    public abstract class Agent : MonoBehaviour
    {
        protected FSMComponent FsmComponent;

        [Header("FSM Variables")]
        [SerializeField] private StateData[] statesData;
        [SerializeField] private StateType initialState;

        [Header("Attack Variables")]
        [SerializeField] private float tba;
        public float Tba => tba;
        public float CurrentTba { get; set; }

        [SerializeField] private float attackRange;
        public float AttackRange => attackRange;

        [Header("Patrol Variables")]
        [SerializeField] private Transform[] waypoints;
        public Transform[] Waypoints => waypoints;

        public Transform Target { set; get; }

        [Header("Movement Variables")]
        [SerializeField] private float speed;
        public float Speed => speed;

        [Header("Searching Variables")]
        [SerializeField] private float fieldOfView;
        public float FieldOfView => fieldOfView;
        [SerializeField] private float searchRadius;
        public float SearchRadius => searchRadius;
        [SerializeField] private LayerMask targetLayer;
        public LayerMask TargetLayer => targetLayer;
        [SerializeField] private LayerMask obstacleLayer;
        public LayerMask ObstacleLayer => obstacleLayer;
        
        public StateType CurrentState => FsmComponent.CurrentState;
        
        [SerializeField] private Animator animator;
        public Animator Animator => animator;

        private void Awake()
        {
            FsmComponent = new FSMComponent(statesData, this, initialState);
            CurrentTba = tba;
        }

        public void ChangeState(StateType newState)
        {
            FsmComponent.ChangeState(newState);
        }
    }
}