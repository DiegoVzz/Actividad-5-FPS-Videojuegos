using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Configuraci�n")]
    public float flightSpeed = 5f;
    public float reachedDistance = 1f;
    public PathMode pathMode = PathMode.Loop;
    public List<Transform> waypoints = new List<Transform>();

    private int currentWaypointIndex = 0;
    private bool isForward = true;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

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

        // Rotaci�n corregida
        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            // A�ade una compensaci�n de -90� en el eje X
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 90, 0);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * 5f
            );
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

    // Visualizaci�n de la ruta en el Editor
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