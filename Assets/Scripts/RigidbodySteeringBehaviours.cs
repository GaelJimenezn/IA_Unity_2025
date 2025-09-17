using UnityEngine;

public class RigidbodySteeringBehaviours : MonoBehaviour
{
    // Velocidad máxima a la que puede ir este agente.
    public float maxSpeed = 10.0f;
    
    // máxima fuerza que se le puede aplicar
    public float maxForce = 5.0f;
    
    public float lookAheadTime = 2.0f;
    //componente que maneja las fuerzas y velocidad de nuestro agente
    private Rigidbody _rb;
    
    public Vector3 _targetPosition = Vector3.zero;
    public Rigidbody _targetRb; //Rigidbody del objetivo

    private bool _targetIsSet = false;

    public void SetTarget(Vector3 target, Rigidbody targetRb)
    {
        _targetPosition = target;
        _targetRb = targetRb;
        _targetIsSet = true;
    }

    public void RemoveTarget()
    {
        _targetIsSet = false;
        if(_rb != null) 
            _rb.linearVelocity = Vector3.zero;
    }

    public void Start()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogWarning($"no se encontro rigidbody para el agente: {name}. Si esta asignado?");
        }
    }

    public Vector3 Seek(Vector3 seekTargetPosition)
    {
        // Si sí hay un objetivo, empezamos a hacer Seek, o sea, a perseguir ese objetivo.
        // Lo primero es obtener la dirección deseada. El método punta menos cola lo usamos con nuestra posición
        // como la cola, y la posición objetivo como la punta
        Vector3 puntaMenosCola = Senses.PuntaMenosCola(seekTargetPosition, transform.position);
        Vector3 _desiredDirection = puntaMenosCola.normalized; // normalized nos da la pura dirección con una magnitud de 1.

        // Ya que tenemos esa dirección, la multiplicamos por nuestra velocidad máxima posible, y eso es la velocidad deseada.
        Vector3 _desiredVelocity = _desiredDirection * maxSpeed;
        
        // La steering force es la diferencia entre la velocidad deseada y la velocidad actual
        Vector3 _steeringForce = _desiredVelocity - _rb.linearVelocity;
        
        return _steeringForce;
    }
    // Update is called a fixed number of times each second. 50 by default.
     void FixedUpdate()
    {
        // Ver si hay una posición objetivo a la cual moverse.
        if (!_targetIsSet || _targetRb == null)
            return; // si no lo hay, no hagas nada.
        
     //pursuit
     //tenemos que obtener la posicion futura del objetivo.necesitamos"
     //A) posicion actual del objetivo
     //B)velocidad actual del objetivo (el vector que trae tanto magnitud como direccion)
     //C) el tiempo en el futuro en el que queremos predecir 
    // _targetPosition
  
     Vector3 targetCurrentVelocity = _targetRb.linearVelocity;
     Vector3 PredictedPosition = _targetPosition + targetCurrentVelocity * lookAheadTime;
     
     // Se busca la posición predicha en lugar de la actual para hacer el Pursuit
     Vector3 steeringForce = Seek(PredictedPosition);

     // la steering force no puede ser mayor que la max steering force PERO sí puede ser menor.
     steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);
     // Nos falta usar esta fuerza para mover a nuestro agente.
     // F = m*a   
     // F/1 = a
     // Con 1 de masa, nuestra fuerza se convierte en nuestra aceleración directamente
     _rb.AddForce(steeringForce, ForceMode.Acceleration);
      

        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            Debug.Log($"Cuidado, _currentVelocity es mayor que maxSpeed: {_rb.linearVelocity.magnitude}");
        }
        // el cambio de posicion ya lo hace automaticamanete el ridgidbody por nosotros
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(_targetPosition, Vector3.one*0.5f);
        
        //dibujamos el gizmo de prediccion
        if (Application.isPlaying && _targetRb != null)
        {
            Vector3 targetCurrentVelocity = _targetRb.linearVelocity;
            Vector3 PredictedPosition = _targetPosition + targetCurrentVelocity * lookAheadTime;
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(PredictedPosition, Vector3.one*0.5f);
        }
    }
}