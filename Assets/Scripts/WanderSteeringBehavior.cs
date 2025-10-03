using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class WanderSteeringBehavior : RigidbodySteeringBehaviours
{

    public float minWanderDistance = 3.0f;
    public float maxWanderDistance = 10.0f;

    public float radiusToTargetPositionTolerance = 1.0f;
    private Vector3 _currentTargetPosition;

    public void UpdateWanderTargetPosition()
    {
        // Obtener una dirección al azar.
        // Por ejemplo, hacer 3 randoms (x, y, z) y luego lo normalizamos.
        // Podríamos quitarle la Y, para que no se mueva en Y.
        Vector3 direction = new Vector3(Random.Range(-1.0f, 1.0f), 0.0f, Random.Range(-1.0f, 1.0f)).normalized;

        float randomDistance = Random.Range(minWanderDistance, maxWanderDistance);

        _currentTargetPosition = transform.position + direction * randomDistance;
    }

    public void Start()
    {
        // primero manda a llamar el start de la clase padre, para obtener el rigidbody component.
        base.Start();
        // la inicializamos así para que en el primer update saque una nueva posición.
        _currentTargetPosition = transform.position;
    }
    
    public void FixedUpdate()
    {
        Vector3 steeringForce = Vector3.zero;
        // siempre va a hacer wander

        // Si ya llegamos o estamos cerca de la currentTargetPosition, entonces obtenemos una nueva.
        if (Utilities.IsObjectInRange(transform.position, _currentTargetPosition, radiusToTargetPositionTolerance))
        {
            UpdateWanderTargetPosition();
        }

        steeringForce = Arrive(_currentTargetPosition);
        
        steeringForce += ObstacleAvoidance();
        
        // la steering force no puede ser mayor que la max steering force PERO sí puede ser menor.
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        

        // Aplicamos esta fuerza para mover a nuestro agente.
        _rb.AddForce(steeringForce, ForceMode.Acceleration);

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            Debug.Log($"Cuidado, _currentVelocity es mayor que maxSpeed: {_rb.linearVelocity.magnitude}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.crimson;
        Gizmos.DrawWireSphere(_currentTargetPosition, 0.5f);
    }
}
