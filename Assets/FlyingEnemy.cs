using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Configuración")]
    public float flightSpeed = 5f;
    public float reachedDistance = 1f;
    public PathMode pathMode = PathMode.Loop;
    public List<Transform> waypoints = new List<Transform>();

    private int currentWaypointIndex = 0;
    private bool isForward = true;

    void Update()
    {
        if (waypoints.Count == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        MoveTowardsTarget(target.position);

        if (Vector3.Distance(transform.position, target.position) <= reachedDistance)
        {
            GetNextWaypoint();
        }
    }

    void MoveTowardsTarget(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, flightSpeed * Time.deltaTime);

        // Rotación suave hacia el objetivo
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    void GetNextWaypoint()
    {
        switch (pathMode)
        {
            case PathMode.Loop:
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                break;

            case PathMode.PingPong:
                if (isForward)
                {
                    if (currentWaypointIndex >= waypoints.Count - 1)
                    {
                        isForward = false;
                        currentWaypointIndex--;
                    }
                    else
                    {
                        currentWaypointIndex++;
                    }
                }
                else
                {
                    if (currentWaypointIndex <= 0)
                    {
                        isForward = true;
                        currentWaypointIndex++;
                    }
                    else
                    {
                        currentWaypointIndex--;
                    }
                }
                break;
        }
    }

    public enum PathMode
    {
        Loop,
        PingPong
    }

    // Visualización de la ruta en el Editor
    void OnDrawGizmosSelected()
    {
        if (waypoints.Count < 2) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawSphere(waypoints[i].position, 0.5f);
            }
        }
    }
}