using UnityEngine;

// Desarrollado por: Gael (Proyecto Individual)
// Esta clase hereda de RigidbodySteeringBehaviours y añade la lógica de persecución.

public class PursuitSteering : RigidbodySteeringBehaviours
{
    // Las propiedades de la clase base, como maxSpeed y maxForce, se heredan automáticamente.

    public void UpdateMovement()
    {
        if (!_targetIsSet || _targetRb == null) 
            return;
        
        Vector3 targetCurrentVelocity = _targetRb.linearVelocity;
        Vector3 predictedPosition = _targetPosition + targetCurrentVelocity * lookAheadTime;
        
        Vector3 puntaMenosCola = (predictedPosition - transform.position);
        Vector3 desiredDirection = puntaMenosCola.normalized; 

        Vector3 desiredVelocity = desiredDirection * maxSpeed;
        
        Vector3 steeringForce = desiredVelocity - _rb.linearVelocity;
        
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        _rb.AddForce(steeringForce, ForceMode.Acceleration);

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            _rb.linearVelocity = _rb.linearVelocity.normalized * maxSpeed;
        }
    }
}