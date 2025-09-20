using System;
using System.Collections.Generic;
using UnityEngine;

public class PatrolSteeringBehavior : RigidbodySteeringBehaviours
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float toleranceRadius = 2.0f;

    private int _currentTargetWaypoint = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected new void Start() // <- ya no override
    {
        base.Start(); // Ojo: esto solo compila si Start() existe en la base como public/protected
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 steeringForce = Seek(waypoints[_currentTargetWaypoint].position);
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        _rb.AddForce(steeringForce, ForceMode.Acceleration);
    }

    private void CambiarWaypointManualmente()
    {
        float distanceToWaypoint =
            Senses.PuntaMenosCola(waypoints[_currentTargetWaypoint].position, transform.position).magnitude;

        if (distanceToWaypoint < toleranceRadius)
        {
            _currentTargetWaypoint++;
            _currentTargetWaypoint %= waypoints.Count;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            _currentTargetWaypoint++;
            _currentTargetWaypoint %= waypoints.Count;
        }

        Debug.Log($"El objeto: {name} chocó contra el trigger: {other.name}");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Destroy(gameObject);
        }

        Debug.Log($"El objeto: {name} chocó contra el collider (no-trigger): {other.gameObject.name}");
    }

    private void OnDrawGizmosSelected()
    {
        // Color personalizado para rosa claro
        Gizmos.color = new Color(1f, 0.4f, 0.7f);
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint.position, toleranceRadius);
        }
    }

    protected new void OnDrawGizmos() // <- usamos new en lugar de override
    {
        Gizmos.color = Color.red;
        if (waypoints != null && waypoints.Count > 0)
        {
            Gizmos.DrawLine(transform.position, waypoints[_currentTargetWaypoint].position);
        }
    }
}
