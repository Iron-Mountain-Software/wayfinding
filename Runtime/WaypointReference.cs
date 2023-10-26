using System;
using UnityEngine;

namespace IronMountain.Wayfinding
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Gameplay/AI/Waypoint Reference")]
    public class WaypointReference : ScriptableObject
    {
        [SerializeField] private string id;
        
        public string ID
        {
            get => id;
            set => id = value;
        }

        public virtual string Name => name;

        private void OnEnable()
        {
            WaypointManager.WaypointReferences.Add(this);
        }

        private void OnDisable()
        {
            WaypointManager.WaypointReferences.Remove(this);
        }

#if UNITY_EDITOR

        public virtual void Reset()
        {
            GenerateNewID();
        }
        
        [ContextMenu("Generate New ID")]
        private void GenerateNewID()
        {
            ID = UnityEditor.GUID.Generate().ToString();
        }

#endif
    }
}