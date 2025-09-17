using System;
using UnityEngine;

public class SteeringBehaviors : MonoBehaviour
{
    // Velocidad máxima a la que puede ir este agente.
    public float maxSpeed = 10.0f;
    
    // máxima fuerza que se le puede aplicar
    public float maxForce = 5.0f;
    
    // nuestra posición actual.
    // ya está almacenada en transform.position
    
    // Dirección y magnitud de movimiento actual.
    private Vector3 _currentVelocity = Vector3.forward;
    
    // Dirección deseada (es decir, desde mi posición hasta la de mi objetivo)
    private Vector3 _desiredDirection = Vector3.forward;
    
    // Velocidad deseada es la velocidad máxima a la que puede ir este agente, pero en la dirección deseada
    private Vector3 _desiredVelocity = Vector3.zero;
    
    // Fuerza de redirección (Steering force)
    private Vector3 _steeringForce = Vector3.zero;
    
    // Posición del objetivo.
    public Vector3 _targetPosition = Vector3.zero;


    private bool _targetIsSet = false;

    public void SetTarget(Vector3 target)
    {
        _targetPosition = target;
        _targetIsSet = true;
    }

    public void RemoveTarget()
    {
        _targetIsSet = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame

    public Vector3 Seek(Vector3 _targetPosition)
    {
        // Si sí hay un objetivo, empezamos a hacer Seek, o sea, a perseguir ese objetivo.
        // Lo primero es obtener la dirección deseada. El método punta menos cola lo usamos con nuestra posición
        // como la cola, y la posición objetivo como la punta
        Vector3 puntaMenosCola = Senses.PuntaMenosCola(_targetPosition, transform.position);
        Vector3 _desiredDirection = puntaMenosCola.normalized; // normalized nos da la pura dirección con una magnitud de 1.

        // Ya que tenemos esa dirección, la multiplicamos por nuestra velocidad máxima posible, y eso es la velocidad deseada.
        Vector3 _desiredVelocity = _desiredDirection * maxSpeed;
        
        // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
        Vector3 _steeringForce = _desiredVelocity - _currentVelocity;
        
        return _steeringForce;
    }
    void Update()
    {
        // Ver si hay una posición objetivo a la cual moverse.
        if (!_targetIsSet)
            return; // si no lo hay, no hagas nada.
        
     //pursuit
     //tenemos que obtener la posicion futura del objetivo.necesitamos"
     //A) posicion actual del objetivo
     //B)velocidad actual del objetivo (el vector que trae tanto magnitud como direccion)
     //C) el tiempo en el futuro en el que queremos predecir 
    // _targetPosition
     Vector3 PredictedPosition = Vector3.one;
     
     Vector3 puntaMenosCola = Senses.PuntaMenosCola(_targetPosition, transform.position);
     Vector3 _desiredDirection = puntaMenosCola.normalized; // normalized nos da la pura dirección con una magnitud de 1.

     // Ya que tenemos esa dirección, la multiplicamos por nuestra velocidad máxima posible, y eso es la velocidad deseada.
     Vector3 _desiredVelocity = _desiredDirection * maxSpeed;
        
     // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
     _steeringForce = _desiredVelocity - _currentVelocity;
        

// la steering force no puede ser mayor que la max steering force PERO sí puede ser menor.
        _steeringForce = Vector3.ClampMagnitude(_steeringForce, maxForce);
        // Nos falta usar esta fuerza para mover a nuestro agente.
        // F = m*a   
        // F/1 = a
        // Con 1 de masa, nuestra fuerza se convierte en nuestra aceleración directamente
        _currentVelocity += _steeringForce * Time.deltaTime;

        if (_currentVelocity.magnitude > maxSpeed)
        {
            Debug.Log($"Cuidado, _currentVelocity es mayor que maxSpeed: {_currentVelocity.magnitude}");
        }
        
        // y después, cambiamos la posición del agente conforme a la velocidad y cuánto tiempo ha pasado.
        transform.position += _currentVelocity * Time.deltaTime;


    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Gizmos.DrawCube(_targetPosition, Vector3.one*0.5f);
        
    }
}