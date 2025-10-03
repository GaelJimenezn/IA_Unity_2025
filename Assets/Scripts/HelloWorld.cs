using System;
using UnityEngine;



// los ':' indican Herencia. En este caso nos dice que la clase HelloWorld hereda de la clase MonoBehaviour.
public class HelloWorld : MonoBehaviour
{
    private NonMonoBehaviour _nonMonoBehaviour = new NonMonoBehaviour();

    public Vector3 velocity = Vector3.zero;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Todos los Start de todos los monoBehaviours se van a ejecutar antes que el update de cualquier monobehaviour.
    void Start()
    {
        Debug.Log("Hello World");
        Debug.LogWarning("Hello World");
        Debug.LogError("Hello World");
        
        _nonMonoBehaviour.Start();
    }

    // Update is called once per frame
    // cuánto es un frame en tiempo? 1 segundo? 12 minutos? 3 horas? 1 milisegundo?
    // FPS -> frames per second
    // FPS adecuado es de 1/60 es 1 segundo entre 60 frames. Esto nos daría 60 frames por segundo
    // 60hz, 120hz, 144hz, 240hz
    // en el cine nosotros nos dan 24 frames por segundo
    // La realidad es que update no se ejecuta un número fijo de veces en el tiempo, se ejecuta TODAS LAS QUE PUEDA
    void Update()
    {
        // Idealmente se ejecutaría estas 60 veces por segundo
        // Debug.Log("Hola mundo");

        // Quiero mi objeto se mueva 1 unidad por frame
        // transform.position += new Vector3(1, 0, 0);

        velocity += new Vector3(0.0f, -9.81f, 0.0f) * Time.deltaTime;

        transform.position += velocity * Time.deltaTime;
        
        
        // Quiero mi objeto se mueva 1 unidad por segundo
        // transform.position += new Vector3(5, 0, 0) * Time.deltaTime;

        // Qué significa "Delta"?
        // Viene del abecedario griego y es el equivalente de la letra D.
        // Delta se le conoce como "cuantificar el cambio"
        // Qué significa Time?
        // Time en español es tiempo.
        // Entonces delta Time es "El cambio del tiempo".
        // Más específicamente es: Cuánto tiempo pasó entre que se ejecutó el último frame y este frame.
        
        // Hace rato dijimos que el "Hola mundo" se ejecutó como 80 veces.
        // En promedio, 1 segundo entre 80 veces, que sería una ejecución cada 0.0125 segundos, o 12.5 milisegundos.
        // uno pudo haber tardado 12 milisegundos y otro pudo haber tardado 13, pero en promedio quedaría en 12.5
        
    }

    private void FixedUpdate()
    {
        // Senses.RadioDeDeteccionStatic
    }
}
