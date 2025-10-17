using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class PatrolSteeringBehavior : RigidbodySteeringBehaviours
{
    [SerializeField] private List<Vector3> waypoints = new List<Vector3>();
    [SerializeField] private float toleranceRadius = 2.0f;
    
    private int _currentTargetWaypoint = 0;

    private Pathfinding _pathfinding;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        _pathfinding = GetComponent<Pathfinding>();
        if (_pathfinding == null)
        {
            Debug.LogError("No pathfinding component found in this gameObject" + name);
        }

        _pathfinding.FindPath();
        foreach (var node in _pathfinding.PathToGoal)
        {
            // creamos una posición en el mundo por cada nodo del camino
            Vector3 newWaypoint = new Vector3(node.X, -node.Y, 0.0f);
            waypoints.Add(newWaypoint);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // este agente tiene una serie de waypoints a los cuales tiene que visitar en orden.


        // La dejo comentada porque yo voy a estar usando lo del OnTriggerEnter.
        CambiarWaypointManualmente();
        
        // se va a mover usando un steering behavior de seek, hacia el waypoint objetivo actual
        // y para ello, necesitamos guardar cuál es el objetivo actual.
        Vector3 steeringForce = Seek(waypoints[_currentTargetWaypoint]);
        
        // la steering force no puede ser mayor que la max steering force PERO sí puede ser menor.
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        
        // Aplicamos esta fuerza para mover a nuestro agente.
        _rb.AddForce(steeringForce, ForceMode.Acceleration);
        
    }

    private void CambiarWaypointManualmente()
    {
        // para cambiar el índice hacia el siguiente waypoint lo hacemos cuando ya hayamos llegado al actual.
        float distanceToWaypoint =
            Utilities.PuntaMenosCola(waypoints[_currentTargetWaypoint], transform.position).magnitude;

        // esto es la manera estándar en que lo harían
        // Vector3.Distance(waypoints[_currentTargetWaypoint].position, transform.position);
        
        if (distanceToWaypoint < toleranceRadius)
        {
            // si las posiciones son iguales, entonces ya llegamos.
            _currentTargetWaypoint++;
            // Lo ciclamos al 0 en caso de que haya sido el último waypoint.
            _currentTargetWaypoint %= waypoints.Count;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != this.gameObject)
            // entonces no chocaron consigo mismos.
        
        // poner en el script de la bala que tenga una referencia al gameObject que la disparó.
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Waypoint"))
        {
            // si las posiciones son iguales, entonces ya llegamos.
            _currentTargetWaypoint++;
            // Lo ciclamos al 0 en caso de que haya sido el último waypoint.
            _currentTargetWaypoint %= waypoints.Count;
        }
        
        
        Debug.Log($"El objeto: {name} chocó contra el trigger: {other.name}");
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // si me toca un enemy con Collider (no-trigger), se destruye el gameObject dueño de este script.
            // Destroy(gameObject);
        }
        
        Debug.Log($"El objeto: {name} chocó contra el collider (no-trigger): {other.gameObject.name}");
    }

    // Casi nunca se usa porque es muy pesada. Hay alternativas mejores.
    // private void OnTriggerStay(Collider other)
    // {
    //     
    // }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.pink;
        foreach (var waypoint in waypoints)
        {
            Gizmos.DrawWireSphere(waypoint, toleranceRadius);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (waypoints.Count == 0)
            return;
        // Línea hacia su target
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, waypoints[_currentTargetWaypoint]);
    }
}
