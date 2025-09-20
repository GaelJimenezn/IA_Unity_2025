// Desarrollado por: Gael (Proyecto Individual)
// Este script sirve como puente entre VisionCone y RigidbodySteeringBehaviours,
// manejando la lógica de persecución y seguimiento.
// Basado en la clase "RigidbodySteeringBehaviours" del profesor y adaptado del código de un compañero.
// Fuentes: Código original del curso, código de un compañero (sebastian) para ver otros puntosde vista y programcion.
using UnityEngine;

public class VisionConeSteering : RigidbodySteeringBehaviours
{
    [Header("Steering Settings")]
    [Tooltip("El componente VisionCone en este GameObject.")]
    private VisionCone _visionCone;

    [Tooltip("La velocidad de rotación cuando el agente sigue al objetivo con la mirada.")]
    [SerializeField] private float lookAtSpeed = 5.0f;

    public new void Start()
    {
        base.Start();
        
        _visionCone = GetComponent<VisionCone>();
        if (_visionCone == null)
        {
            Debug.LogError("VisionConeSteering requiere un componente VisionCone en el mismo GameObject.", this);
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        // Si el cono de visión detecta un objetivo...
        if (_visionCone.CanSeeTarget && _visionCone.DetectedTarget != null)
        {
            // ...configura el objetivo para la persecución en la clase base
            SetTarget(_visionCone.DetectedTarget.position, _visionCone.DetectedTarget.GetComponent<Rigidbody>());
        }
        else
        {
            // ...de lo contrario, quita el objetivo
            RemoveTarget();
            
            // Si el agente no tiene un objetivo, se detiene
            if (_rb != null)
            {
                _rb.linearVelocity = Vector3.zero;
            }
        }

        // Lógica de persecución
        if (_targetIsSet && _targetRb != null)
        {
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
    
    void Update()
    {
        // Lógica para seguir con la mirada (siempre activa)
        LookAtTarget();
    }

    private void LookAtTarget()
    {
        if (_visionCone.DetectedTarget != null)
        {
            Vector3 direction = _visionCone.DetectedTarget.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookAtSpeed);
            }
        }
    }
}