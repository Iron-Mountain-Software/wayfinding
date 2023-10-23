using System.Collections;
using System.Collections.Generic;
using IronMountain.Wayfinding;
using UnityEditor;
using UnityEngine;

public class MenuItems : MonoBehaviour
{
    [MenuItem("Component/Wayfinding/Join %j", false)]
    static void JoinWaypoints(MenuCommand menuCommand)
    {
        List<Waypoint> waypoints = new List<Waypoint>();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Waypoint waypoint = gameObject.GetComponent<Waypoint>();
            if (waypoint) waypoints.Add(waypoint);
        }
        foreach (var waypoint in waypoints)
        {
            if (!waypoint) continue;
            foreach (var neighbor in waypoints)
            {
                if (!neighbor || waypoint == neighbor) continue;
                waypoint.AddNeighbor(neighbor);
            }
        }
    }
    
    [MenuItem("Component/Wayfinding/Separate %#j", false)]
    static void SeparateWaypoints(MenuCommand menuCommand)
    {
        List<Waypoint> waypoints = new List<Waypoint>();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            Waypoint waypoint = gameObject.GetComponent<Waypoint>();
            if (waypoint) waypoints.Add(waypoint);
        }
        foreach (var waypoint in waypoints)
        {
            if (!waypoint) continue;
            foreach (var neighbor in waypoints)
            {
                if (!neighbor || waypoint == neighbor) continue;
                waypoint.RemoveNeighbor(neighbor);
            }
        }
    }
}
