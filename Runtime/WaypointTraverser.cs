using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    public class WaypointTraverser : MonoBehaviour
    {
        public event Action OnMovingChanged;
        public event Action OnCurrentWaypointChanged;

        [SerializeField] private Waypoint currentWaypoint;
        [SerializeField] private Waypoint destinationWaypoint;
        [Space]
        [SerializeField] private float speed = 5f;
        [SerializeField] private float multiplier = 1f;
        [SerializeField] private Vector3 offset;
        [Space]
        [SerializeField] private bool rotate = true;
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
        
        public bool Rotate
        {
            get => rotate;
            set => rotate = value;
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
            
            Vector3 direction = GetDirection();

            if (rotate)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationMultiplier);
            }
            
            Vector3 targetPosition = CurrentWaypoint.transform.position + offset;
            float remainingDistance = Vector3.Distance(transform.position, targetPosition);
            float frameDistance = multiplier * speed * Time.deltaTime;
            if (frameDistance < remainingDistance)
            {
                transform.Translate(direction * frameDistance, Space.World);
                Moving = true;
            }
            else
            {
                transform.position = CurrentWaypoint.transform.position + offset;
                if (_path.Count > 0) _path.RemoveAt(0);
                Moving = _path.Count > 0;
                if (!Moving) destinationWaypoint = null;
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