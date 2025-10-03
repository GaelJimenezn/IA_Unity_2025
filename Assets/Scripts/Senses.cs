using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Lo vamos a usar para poner todo lo que tiene que ver con detección, visión, y si da tiempo, tacto y oído.
/// </summary>
public class Senses : MonoBehaviour
{

    // Opciones para remplazar valores hardcodeados:
    // 1) hacer una variable que se puede cambiar desde el editor.
    public float radioDeDeteccion = 20.0f;
    // 1.A) hacer una variable no-pública que se puede cambiar desde el editor
    [SerializeField] private float radioDeDeteccionPrivado = 2.0f;

    [SerializeField] private LayerMask desiredDetectionLayers; 
    
    // 2) variable pública estática.
    // La variable static solo una vez se le puede asignar valor y después ya nunca puede cambiar.
    public static float RadioDeDeteccionStatic = 20.0f;
    
    // 2.A) variable const
    // es muy parecida a la static, pero NO se le puede asignar un valor ya en la ejecución.
    public const float RADIO_DE_DETECCION_CONST = 20.0f;
    
    // 3) Scriptable Objects
    // es un tipo de clase especial que sirve principalmente para guardar datos, pero también puede tener funciones.
    // Solo se instancía una vez, y todos los que referencíen a ese scriptableObject pueden acceder a esa única instancia.
    // Ayuda muchísimo a reducir el uso de memoria cuando A) se va a remplazar muchos datos de una clase y
    // B) cuando va a haber muchos que usen esos datos
    
    // 4) Un archivo de configuración.
    
    // List en C# es el equivalente de vector<> en C++ (es decir, es un array de tamaño dinámico, no una lista ligada).
    // Lista de GameObjects encontrados este frame
    private List<GameObject> _foundGameObjects ;
    public List<GameObject> foundGameObjects => _foundGameObjects;

    



        
    
    // Vamos a detectar cosas que estén en un radio determinado.
    void DetectarTodosLosGameObjects()
    {
        // Esta obtiene TODOS los gameObjects en la escena.
        _foundGameObjects = GetGameObjectsInsideRadius(radioDeDeteccion, transform.position);
    }

    public static List<GameObject> GetGameObjectsInsideRadius(float radius, Vector3 position)
    {
        List<GameObject> foundGO = FindObjectsByType<GameObject>(FindObjectsSortMode.InstanceID).ToList();

        List<GameObject> gameObjectsInsideRadius = new List<GameObject>();
        
        // Después los filtramos para que solo nos dé los que sí están dentro del radio determinado.
        foreach (var foundGameObject in foundGO)
        {
            if (Utilities.IsObjectInRange(foundGameObject.transform.position, position, radius))
            {
                gameObjectsInsideRadius.Add(foundGameObject);
            }
        }

        return gameObjectsInsideRadius;
    }




    

    
    public List<GameObject> GetAllObjectsByLayer(int layer)
    {
        List<GameObject> objects = new List<GameObject>();
        foreach (var foundObject in _foundGameObjects)
        {
            // break; // break es: salte del ciclo donde estés.

            if (foundObject.layer != layer)
                continue; // continue es: vete a la siguiente iteración del ciclo en donde estás.
                
            if (Utilities.IsObjectInRange(foundObject.transform.position, transform.position, radioDeDeteccion))
            {
                objects.Add(foundObject);
            }
                
        }

        return objects;
    }
    
    // Esta es peor en performance, por eso la quité, pero vean la flexibilidad que nos da tener las cosas en funciones.
    // public List<GameObject> GetAllObjectsByLayerAlterna(int layer)
    // {
    //     List<GameObject> objects = GetGameObjectsInsideRadius(radioDeDeteccion, transform.position);
    //     foreach (var foundObject in _foundGameObjects)
    //     {
    //         // break; // break es: salte del ciclo donde estés.
    //
    //         if (foundObject.layer != layer)
    //             continue; // continue es: vete a la siguiente iteración del ciclo en donde estás.
    //         
    //         // si sí son de la layer que quiero, entonces los añado a los gameObjects de salida.
    //         objects.Add(foundObject);
    //     }
    //
    //     return objects;
    // }

    public List<GameObject> GetPlayers()
    {
        return GetAllObjectsByLayer(LayerMask.NameToLayer("Player"));
    }
    
    public List<GameObject> GetEnemies()
    {
        return GetAllObjectsByLayer(LayerMask.NameToLayer("Enemy"));
    }
    
    public List<GameObject> GetEnemyBullets()
    {
        return GetAllObjectsByLayer(LayerMask.NameToLayer("EnemyBullet"));
    }


    
    // la Layer es solo un int, entonces solo necesitas una comparación para saber qué choca con qué.
    // Principalmente se usan para la simulación física. Podemos tener hasta 32 layers porque un entero tiene 32 bits (4 bytes).
    // [0100 0000] // este es un enemy
    // [1000 0000] // este de aquí es un player (
    //
    //
    // [1000 0000] -> [0000 1110][0100 0001] // esta es la máscara de bits de las cosas contras las que el Player sí quiere chocar.
    //
    // // chocamos contra un Enemy
    // [0000 0000][0100 0000] // enemy
    // [0000 1110][0100 0001] // máscara de bits de player
    // [0000 0000][0100 0000] // resultado del AND entre los bits de arriba
    
    // AND lógico -> && : necesita que ambos valores sean verdaderos para dar verdadero
    // los 0s son false, y los 1s son true.
    
    // OR lógico -> || : necesita que al menos uno de los valores sea true para dar true.
    
    // NOT lógico -> ! : invierte los bits de una máscara
    // [0100 0000] // Enemy
    // [1011 1111] // NOT an Enemy
    
    // XOR lógico -> xor : solo uno puede ser verdad y el otro no, y eso da true.
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // DetectarTodosLosGameObjects(); // esta de aquí ya no la llamamos porque es más pesada que la de abajo.
        _foundGameObjects = Utilities.GetObjectsInRadius(transform.position, radioDeDeteccion, desiredDetectionLayers);
    }

    // OnDrawGizmos Se manda a llamar cada que la pestaña de escena se actualiza. Se actualiza incluso cuando no estás en play mode.
    // OnDrawGizmosSelected hace lo mismo, pero solo cuando el gameObject con este script esté seleccionado en la escena.
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radioDeDeteccion);

        if (Application.isPlaying)
        {
            // Después los filtramos para que solo nos dé los que sí están dentro del radio determinado.
            foreach (var foundGameObject in _foundGameObjects)
            {
                if (Utilities.IsObjectInRange(foundGameObject.transform.position, transform.position, radioDeDeteccion))
                {
                    Gizmos.color = Color.green;
                    // Sí está dentro del radio
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.red;
                    // no está dentro del radio.
                    Gizmos.DrawWireCube(foundGameObject.transform.position, Vector3.one);
                }
                
            }
        }
    }
}

/*
 * Un valor hardcodeado (hardcoded) es un valor alfanumérico que está en el código, pero se necesita para ajustar el
 * funcionamiento de las cosas.
 *
 * El problema de los valores hardcodeados se vuelve más grande entre más veces se utilice dicho valor.
 * 
 */