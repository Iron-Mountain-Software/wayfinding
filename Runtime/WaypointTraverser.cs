using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    public class WaypointTraverser : MonoBehaviour
    {
        public enum RotationType
        {
            None,
            Align,
            AlignClamped,
            LookAt,
            LookAtClamped
        }

        public event Action OnMovingChanged;
        public event Action OnCurrentWaypointChanged;

        [SerializeField] private Waypoint currentWaypoint;
        [SerializeField] private Waypoint destinationWaypoint;
        [Space]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float multiplier = 1f;
        [SerializeField] private Vector3 offset;
        [Space]
        [SerializeField] private RotationType rotateWhenMoving = RotationType.Align;
        [SerializeField] private RotationType rotateWhenStationary = RotationType.Align;
        [SerializeField] private Transform lookAtTransform;
        [SerializeField] private Vector3 clampNormal = Vector3.up;
        [SerializeField] private float rotationMultiplier = 3f;

        [Header("Cache")]
        private bool _moving;
        private readonly List<Waypoint> _path = new ();

        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        
        public float Multiplier
        {
            get => multiplier;
            set => multiplier = value;
        }
        
        public Vector3 Offset
        {
            get => offset;
            set => offset = value;
        }
        
        public RotationType RotateWhenMoving
        {
            get => rotateWhenMoving;
            set => rotateWhenMoving = value;
        }
        
        public RotationType RotateWhenStationary
        {
            get => rotateWhenStationary;
            set => rotateWhenStationary = value;
        }

        public Transform LookAtTransform
        {
            get => lookAtTransform;
            set => lookAtTransform = value;
        }
        
        public float RotationMultiplier
        {
            get => rotationMultiplier;
            set => rotationMultiplier = value;
        }

        public bool Moving
        {
            get => _moving;
            private set
            {
                if (_moving == value) return;
                _moving = value;
                OnMovingChanged?.Invoke();
            }
        }
        
        public Waypoint CurrentWaypoint
        {
            get => currentWaypoint;
            set
            {
                if (!value || currentWaypoint == value) return;
                currentWaypoint = value;
                OnCurrentWaypointChanged?.Invoke();
            }
        }

        public Waypoint DestinationWaypoint
        {
            get => destinationWaypoint;
            set
            {
                if (destinationWaypoint == value) return;
                destinationWaypoint = value;
                RefreshPath();
            }
        }

        public void Initialize(Waypoint start, Waypoint destination = null)
        {
            CurrentWaypoint = start;
            SnapToCurrent();
            destinationWaypoint = destination;
            RefreshPath();
        }
        
        public Vector3 GetDirection()
        {
            return CurrentWaypoint
                ? Moving 
                    ? (CurrentWaypoint.transform.position + offset - transform.position).normalized
                    : CurrentWaypoint.transform.forward 
                : transform.forward;
        }
        
        private void OnEnable() => Moving = false;

        private void OnDisable() => Moving = false;

        private void SnapToCurrent()
        {
            Transform currentWaypointTransform = CurrentWaypoint ? CurrentWaypoint.transform : null;
            if (!currentWaypointTransform) return;
            Transform myTransform = transform;
            myTransform.position = currentWaypointTransform.position + offset;
        }

        private void RefreshPath()
        {
            _path.Clear();
            CurrentWaypoint = WaypointManager.GetClosestWaypointTo(transform.position);
            if (CurrentWaypoint && destinationWaypoint)
            {
                _path.AddRange(WaypointManager.GetShortestPath(CurrentWaypoint, destinationWaypoint));
                if (_path.Count > 1) _path.RemoveAt(0);
            }
        }

        private void RefreshCurrentWaypoint()
        {
            while (_path is {Count: > 0} && !_path[0]) 
            {
                _path.RemoveAt(0);
            }
            if (_path is {Count: > 0}) CurrentWaypoint = _path[0];
            if (!CurrentWaypoint) CurrentWaypoint = WaypointManager.GetClosestWaypointTo(transform.position);
        }
        
        private void Update()
        {
            if (destinationWaypoint
                && transform.position != destinationWaypoint.transform.position + offset
                && (_path is null or {Count: 0} || _path[^1] != destinationWaypoint))
            {
                RefreshPath();
            }
            RefreshCurrentWaypoint();
            if (!CurrentWaypoint) return;
            Vector3 moveDirection = GetDirection();
            HandleMovement(moveDirection);
            HandleRotation(moveDirection);
        }

        private void HandleMovement(Vector3 moveDirection)
        {
            Vector3 targetPosition = CurrentWaypoint.transform.position + offset;
            float frameDistance = multiplier * speed * Time.deltaTime;
            if (frameDistance < Vector3.Distance(transform.position, targetPosition))
            {
                transform.Translate(moveDirection * frameDistance, Space.World);
                Moving = true;
            }
            else
            {
                transform.position = targetPosition;
                if (_path.Count > 0) _path.RemoveAt(0);
                Moving = _path.Count > 0;
                if (!Moving) destinationWaypoint = null;
            }
        }

        private void HandleRotation(Vector3 moveDirection)
        {
            Vector3 lookDirection = Vector3.zero;
            
            switch (Moving ? rotateWhenMoving : rotateWhenStationary)
            {
                case RotationType.Align:
                    lookDirection = moveDirection;
                    break;
                case RotationType.AlignClamped:
                    lookDirection = Vector3.ProjectOnPlane(moveDirection, clampNormal).normalized;
                    break;
                case RotationType.LookAt:
                    lookDirection = lookAtTransform
                        ? lookAtTransform.position - transform.position
                        : moveDirection;
                    break;
                case RotationType.LookAtClamped:
                    lookDirection = lookAtTransform
                        ? lookAtTransform.position - transform.position
                        : moveDirection;
                    lookDirection = Vector3.ProjectOnPlane(lookDirection, clampNormal).normalized;
                    break;
            }

            if (lookDirection != Vector3.zero)
            {                
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationMultiplier);
            }
        }

#if UNITY_EDITOR
        
        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (_path.Count > 0)
            {
                Handles.DrawLine(transform.position, _path[0].transform.position + offset, 2);
            }
            for (int i = 0; i <= _path.Count - 2; i++)
            {
                Waypoint a = _path[i];
                Waypoint b = _path[i + 1];
                Handles.DrawLine(a.transform.position + offset, b.transform.position + offset, 2);
            }
        }
        
#endif

    }
}