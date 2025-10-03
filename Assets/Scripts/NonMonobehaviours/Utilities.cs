using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class Utilities
{
    // Todas las funciones aquí van a ser estáticas, porque no vamos a instanciar nunca un objeto de la clase Utilities.
    
    
    
    public static Vector3 PuntaMenosCola(Vector3 punta, Vector3 cola)
    {
        float x = punta.x - cola.x;
        float y = punta.y - cola.y;
        float z = punta.z - cola.z;
        return new Vector3(x, y, z);
        
        // Internamente, esta línea hace lo que las 4 líneas de arriba harían.
        // return punta - cola;
    }
    
    public static float Pitagoras(Vector3 vector3)
    {
        // hipotenusa = raíz cuadrada de a^2 + b^2 + c^2
        float hipotenusa = math.sqrt(vector3.x * vector3.x +
                                     vector3.y * vector3.y +
                                     vector3.z * vector3.z);
        return hipotenusa;

        // return vector3.magnitude;
    }
    
    public static bool IsObjectInRange(Vector3 posA, Vector3 posB, float range)
    {
        // Primero hacemos punta menos cola entre la posición de este GameObject y la del foundGameObject,
        // esto nos da la flecha que va del uno al otro,
        Vector3 puntaMenosCola = PuntaMenosCola(posA, posB);

        // Y ya con esa flecha, usamos el teorema de Pitágoras, para calcular la distancia entre este gameObject
        // que es dueño de este script Senses y el foundGameObject.
        float distancia = Pitagoras(puntaMenosCola);

        // ya con la distancia calculada, la comparamos contra este radio que determinamos.
        if (distancia < range)
        {
            // Sí está dentro del radio
            return true;
        }

        // no está dentro del radio.
        return false;
    }
    
    public static List<GameObject> GetObjectsInCube(Vector3 position, Vector3 extents, Quaternion orientation, LayerMask desiredLayers)
    {
        Collider[] collidersInBox = Physics.OverlapBox(position, extents, Quaternion.identity, desiredLayers);

        List<GameObject> objectsInBox = new List<GameObject>();
        foreach (var collider in collidersInBox)
        {
            objectsInBox.Add(collider.GameObject());
        }

        return objectsInBox;
    }
    
    /// <summary>
    /// Requiere que los objetos a detectarse tengan colliders que toquen a la esfera descrita por estos parámetros.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="radius"></param>
    /// <param name="desiredLayers"></param>
    /// <returns></returns>
    public static List<GameObject> GetObjectsInRadius(Vector3 position, float radius, LayerMask desiredLayers)
    {
        Collider[] collidersInRadius = Physics.OverlapSphere(position, radius, desiredLayers);

        List<GameObject> objectsInRadius = new List<GameObject>();
        foreach (var collider in collidersInRadius)
        {
            objectsInRadius.Add(collider.GameObject());
        }

        return objectsInRadius;
    }
    
    // esta es la que se usa para comparar entre Tags
    public static bool CompareString(string a, string b)
    {
        // si no son de la misma longitud, no pueden ser iguales
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        
        return true;
    }
    
}
