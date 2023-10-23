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
                SnapToCurrent();
                OnCurrentWaypointChanged?.Invoke();
                RefreshPath();
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

        public void Initialize(Waypoint startWaypoint, Waypoint destinationWaypoint = null)
        {
            currentWaypoint = startWaypoint;
            SnapToCurrent();
            OnCurrentWaypointChanged?.Invoke();
            this.destinationWaypoint = destinationWaypoint;
            RefreshPath();
        }
        
        public Vector3 GetDirection()
        {
            if (_path.Count > 0)
            {
                Vector3 targetPosition = _path[0].transform.position + offset;
                return (targetPosition - transform.position).normalized;
            }
            return CurrentWaypoint 
                ? CurrentWaypoint.transform.forward 
                : transform.forward;
        }
        
        private void OnEnable() => Moving = false;

        private void OnDisable() => Moving = false;

        private void SnapToCurrent()
        {
            Transform currentWaypointTransform = currentWaypoint ? currentWaypoint.transform : null;
            if (!currentWaypointTransform) return;
            Transform myTransform = transform;
            myTransform.position = currentWaypointTransform.position + offset;
        }

        private void RefreshPath()
        {
            _path.Clear();
            currentWaypoint = WaypointManager.GetClosestWaypointTo(transform.position);
            if (currentWaypoint && destinationWaypoint)
            {
                _path.AddRange(WaypointManager.GetShortestPath(currentWaypoint, destinationWaypoint));
                if (_path.Count > 1) _path.RemoveAt(0);
            }
        }

        private void RefreshCurrentWaypoint()
        {
            while (_path is {Count: > 0} && !_path[0]) 
            {
                _path.RemoveAt(0);
            }
            currentWaypoint = _path is {Count: > 0} 
                ? _path[0] 
                : null;
        }

        private void Update()
        {
            if (destinationWaypoint
                && transform.position != destinationWaypoint.transform.position
                && (_path is null or {Count: 0} || _path[^1] != destinationWaypoint))
            {
                RefreshPath();
            }

            RefreshCurrentWaypoint();
            
            Vector3 direction = GetDirection();

            if (rotate)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationMultiplier);
            }
            
            Moving = currentWaypoint;
            
            if (!Moving) return;
            
            Vector3 targetPosition = currentWaypoint.transform.position + offset;
            float remainingDistance = Vector3.Distance(transform.position, targetPosition);
            float frameDistance = multiplier * speed * Time.deltaTime;
            if (frameDistance < remainingDistance)
            {
                transform.Translate(direction * frameDistance, Space.World);
            }
            else
            {
                transform.Translate(direction * remainingDistance, Space.World);
                transform.position = currentWaypoint.transform.position;
                if (_path.Count > 0) _path.RemoveAt(0);
            }
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.red;
            if (_path.Count > 0)
            {
                Handles.DrawLine(transform.position, _path[0].transform.position, 2);
            }
            for (int i = 0; i <= _path.Count - 2; i++)
            {
                Waypoint a = _path[i];
                Waypoint b = _path[i + 1];
                Handles.DrawLine(a.transform.position, b.transform.position, 2);
            }
        }
    }
}