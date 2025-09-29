using System;
using UnityEngine;

public class RigidbodySteeringBehaviours : MonoBehaviour
{
    public ESteeringBehaviors currentBehavior = ESteeringBehaviors.Seek;
    
    // Velocidad máxima a la que puede ir este agente.
    public float maxSpeed = 10.0f;
    
    // máxima fuerza que se le puede aplicar
    public float maxForce = 5.0f;

    public float lookAheadTime = 2.0f;
    
    // Componente que maneja las fuerzas y la velocidad de nuestro agente.
    protected Rigidbody _rb;
    
    // Posición del objetivo.
    public Vector3 _targetPosition = Vector3.zero;
    public Rigidbody _targetRb; // Rigidbody del objetivo.


    protected bool _targetIsSet = false;

    public void SetTarget(Vector3 target, Rigidbody targetRb)
    {
        _targetPosition = target;
        _targetRb = targetRb;
        _targetIsSet = true;
    }

    public void RemoveTarget()
    {
        _targetIsSet = false;
        _targetRb = null; // lo quitamos, ahorita por pura seguridad, pero idealmente hay que quitarlo.
    }

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogWarning($"No se encontró el rigidbody para el agente: {name}. ¿Sí está asignado?");
        }
    }
    
    public Vector3 Seek(Vector3 targetPosition)
    {
        // Si sí hay un objetivo, empezamos a hacer Seek, o sea, a perseguir ese objetivo.
        // Lo primero es obtener la dirección deseada. El método punta menos cola lo usamos con nuestra posición
        // como la cola, y la posición objetivo como la punta
        Vector3 puntaMenosCola = Utilities.PuntaMenosCola(targetPosition, transform.position);
        Vector3 desiredDirection = puntaMenosCola.normalized; // normalized nos da la pura dirección con una magnitud de 1.

        // Ya que tenemos esa dirección, la multiplicamos por nuestra velocidad máxima posible, y eso es la velocidad deseada.
        Vector3 desiredVelocity = desiredDirection * maxSpeed;
        
        // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
        Vector3 steeringForce = desiredVelocity - _rb.linearVelocity;
        return steeringForce;
    }

    public Vector3 Flee(Vector3 targetPosition)
    {
        //porque flee es lo mismo que seek pero la direccion opuesta
        return -Seek(targetPosition);
    }
    
    public Vector3 PredictPosition(Vector3 startingTargetPosition, Vector3 targetVelocity)
    {
        // la distancia entre mi objetivo y yo en este preceso momento/ mi max speed
        float lookAheadCalculado = Utilities.PuntaMenosCola(startingTargetPosition, transform.position).magnitude/maxForce;
   
        Vector3 targetCurrentVelocity = targetVelocity;
        
        Vector3 predictedPosition = startingTargetPosition + targetCurrentVelocity * lookAheadCalculado;
        
        return predictedPosition;
    }
    
    public Vector3 Pursuit(Vector3 targetPosition)
    {
        Vector3 predictedPosition = PredictPosition(_targetPosition, _targetRb.linearVelocity);
        
        // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
        Vector3 steeringForce = Seek(predictedPosition);
        
        return steeringForce;
    }
    
    public Vector3 Evade(Vector3 targetPosition)
    {
        // el sigono '-' es porque Evade es exactamente lo mismo que Pursuit pero en el sentido opuesto
      Vector3 steeringForce = -Pursuit(targetPosition);
      return steeringForce;
    }
    
    
    // Update is called a fixed number of times each second. 50 by default.
    void FixedUpdate()
    {
        // Ver si hay una posición objetivo a la cual moverse.
        if (!_targetIsSet)
            return; // si no lo hay, no hagas nada.

        Vector3 steeringForce = Vector3.zero;
        
        switch (currentBehavior)
        {
            case ESteeringBehaviors.DontMove:
                _rb.linearVelocity = Vector3.zero; // le hacemos la velocidad 0 para que deje de moverse completamente.
                break;
            case ESteeringBehaviors.Seek:
                steeringForce = Seek(_targetPosition);
                break;
            case ESteeringBehaviors.Flee:
                steeringForce = Flee(_targetPosition);
                break;
            case ESteeringBehaviors.Pursuit:
                steeringForce = Pursuit(_targetPosition);
                break;
            case ESteeringBehaviors.Evade:
                steeringForce = Evade(_targetPosition);
                break;
            case ESteeringBehaviors.Arrive:
               // steeringForce = Arrive(_targetPosition);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
        
        // Aplicamos esta fuerza para mover a nuestro agente.
        _rb.AddForce(steeringForce, ForceMode.Acceleration);

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            Debug.Log($"Cuidado, _currentVelocity es mayor que maxSpeed: {_rb.linearVelocity.magnitude}");
        }
        
        // el cambio de posición ya lo hace automáticamente el rigidbody por nosotros.
    }

    protected void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        Gizmos.color = Color.red;
        
        Gizmos.DrawCube(_targetPosition, Vector3.one*0.5f);

        // Si sí hay un rigidbody del target para hacerle Pursuit of evade:
        if (_targetRb != null)
        {
            // dibujamos el gizmo de la posición predicha.
            Vector3 predictedPosition = PredictPosition(_targetPosition, _targetRb.linearVelocity);
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(predictedPosition, Vector3.one*0.5f);   
            
            // Línea desde el agente hasta la posición predicha:
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, predictedPosition);
            
            // línea hacia la posición predicha, pero con la magnitud de nuestra maxSpeed
            Vector3 directionToPredictedPosition = (predictedPosition - transform.position).normalized;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + directionToPredictedPosition*maxSpeed);
            
            // línea de la velocidad real a la que va este agente
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + _rb.linearVelocity);
        
            // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
            Vector3 steeringForce = Pursuit(_targetPosition);
            
            // la steering force no puede ser mayor que la max steering force PERO sí puede ser menor.
            steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, transform.position + steeringForce);
        }

    }
}
