using Unity.Mathematics;
using UnityEngine;


// investigar #define RADIO_DE_DETECCION_DEFINE 20.0f;

//Poner todoo lo que tenga que ver con deteccion, vision y deteccion.
//Si da tiempo tacto y oido
public class Sense : MonoBehaviour
{
    //Opciones para reemplazar valores hardcodeados
    // 1) Hacer una variable que se puede cambiar desde el editor
    public float radioDeDeteccion = 20;
    
    //1.A) Hacer unavarieble no-publica que se puede cambiar desde el editor
    [SerializeField] private float radioDeDeteccionPrivada = 2.0f;
    
    // 2) Variable publica estatica
    //La variable static solo una vez se le puede asignar valor y despues ya nunca puede combinar
    public static float radioDeDeteccionStatic = 20.0f;
    
    // 2.A) Variable const
    //Muy parecida la static pero NO se le puede asiganr un valor ya en la ejecucion
    public const float RADIO_DE_DETECCION_CONST = 20.0f;
    
    // 3) Scriptable Objects
    // es un tipo de clase especial que sirve principalmente para guardar datos, pero tambien puede tener funciones
    // Solo se instancaiba una vez, y todos los que referencien a ese scriptableObject pueden acceder a esa unica instancia
    // ayuda muchisimo a reducir el uso de memeoria cuando A) se va a remplazar muchos datos de una clase y 
    // B)Cuando va a haber muchos que usen esos datos
    
    // 4) Un arcihvo de configuracion
    
    public static Vector3 PuntaMenosCola(Vector3 punta, Vector3 cola)
    {
        //float x = punta.x - cola.x;
        //float y = punta.y - cola.y;
        //float z = punta.z - cola.z;
        //return new Vector3(x, y, z);
        
        //intermanrte esta linea hace lo que las 4 lineas de arriba harian
        return punta - cola;
    }

    public static float Pitagoras(Vector3 vector3)
    {
        // hiponenusa = raiz cuadrad de a cuadrada + b cuadrada + c cuadrada
        float hipetenusa = math.sqrt(vector3.x * vector3.x + 
                                     vector3.y * vector3.y + 
                                     vector3.z * vector3.z);
        return hipetenusa;

        //return vector3.magnitude;
    }
    
    
    
    //Vamos a detectar codad que esten en un radio determinado
    
    //Esta obtiene todos los gameObjects en la escena
    void DetectarTodosLosGameObjects()
    {
        //Esta obtiene  TODOS los gameObjects en la escena
        GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); 
        
        // Despues los filtramos para que solo nos de los que si estan dentro del radio determiando
        foreach (var foundGameObject in allGameObjects)
        {
            // Primero hacemos punta menos cola entre la pisicion de este GameObjetc y la del foundGameObjetc
            // Esto nos da la felcah que va del uno al otro
            
            Vector3 puntaMenosCola = PuntaMenosCola(foundGameObject.transform.position, gameObject.transform.position);
            
            
            //Calcular la distancia entre este gameOject que es dueno de este script Sense y este foundGameeObject
            // que es dueno de este sript senses y el foundGameObject
            
            float distancia = Pitagoras(puntaMenosCola);
            
            // ya con la distancai calculada, la comparamos con este radio que determinamos. 
            
            if (distancia < 10.0f)
            {
                // Si esta dentro del radio
                Debug.Log($"El objeto: {foundGameObject.name} si esta dentro del radio.");
            }
            else
            {
                // no esta dentro del radio
                Debug.Log($"El objeto: {foundGameObject.name} No esta dentro del radio.");
            }
                    
                    
            
        }
    }
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DetectarTodosLosGameObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    // Se manda a llamar acada que la pestana de escena se actualiza. Se actualiza includo cuando no estas en play mode
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5.0f);

        if (Application.isPlaying)
        {
            //Esta obtiene  TODOS los gameObjects en la escena
            GameObject[] allGameObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None); 
        
            // Despues los filtramos para que solo nos de los que si estan dentro del radio determiando
            foreach (var foundGameObject in allGameObjects)
            {
                // Primero hacemos punta menos cola entre la pisicion de este GameObjetc y la del foundGameObjetc
                // Esto nos da la felcah que va del uno al otro

                Vector3 puntaMenosCola =
                    PuntaMenosCola(foundGameObject.transform.position, gameObject.transform.position);


                //Calcular la distancia entre este gameOject que es dueno de este script Sense y este foundGameeObject
                // que es dueno de este sript senses y el foundGameObject

                float distancia = Pitagoras(puntaMenosCola);

                // ya con la distancai calculada, la comparamos con este radio que determinamos. 

                if (distancia < 5.0f)
                {
                    Gizmos.color = Color.green;
                    // Si esta dentro del radio
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.red;
                    // no esta dentro del radio
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
            }
        }
    }
}

/*
 * Un valor hardcodeado es un valor alfan numerico que esta en le codigo pero necesia para austar el funcionamiento
 * de las cosas
 *
 * El problema de los valores
*/