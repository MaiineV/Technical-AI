using System;
using Controller;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        private Vector3 _moveVector;
        [SerializeField] private float speed;
        private Rigidbody _rigidbody;
        
        private void Awake()
        {
            ControllerManager.AddPerformanceEvent(InputNames.Move, OnMove);
            ControllerManager.AddCancelEvent(InputNames.Move, OnMove);
            
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            _rigidbody.velocity = _moveVector * speed;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            _moveVector = context.ReadValue<Vector2>();
            _moveVector.z = _moveVector.y;
            _moveVector.y = 0;
        }
    }
}
