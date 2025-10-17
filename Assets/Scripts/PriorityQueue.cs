using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// T1 es lo que guardas en realidad.
// T2 es un float o un int para guardar la prioridad
public class PriorityQueue<TElement, TPriority>
{
    private SortedDictionary<TPriority, Queue<TElement>> _data = new SortedDictionary<TPriority, Queue<TElement>>();
    
    // Insertar
    public void Enqueue(TElement element, TPriority priority)
    {
        // Ver si ya está en el diccionario esa llave.
        if(_data.TryGetValue(priority, out Queue<TElement> queue))
        {
            // Si sí está ya esa key, ponemos al nuevo elemento al final de la fila.
            queue.Enqueue(element);
            return;
        }
        // si no está esa key, entonces tenemos que insertarla y crear una nueva queue para esa llave, y meter 
        // el nuevo elemento en esa queue.
        Queue<TElement> newQueue = new Queue<TElement>();
        newQueue.Enqueue(element);
        _data.Add(priority, newQueue);
    }
    
    // public void Enqueue(TElement element, TPriority priorityCost, TPriority priorityHeuristic)
    // {
    //     dynamic cost = priorityCost;
    //     dynamic heuristic = priorityHeuristic;
    //     TPriority totalCost = cost + heuristic;
    //     
    //     // Ver si ya está en el diccionario esa llave.
    //     if(_data.TryGetValue(totalCost, out Queue<TElement> queue))
    //     {
    //         queue.ToArray();
    //         // y aquí tenemos que poner que los que tengan menos distance vayan quedando al frente.
    //         for()
    //         
    //         // Si sí está ya esa key, ponemos al nuevo elemento al final de la fila.
    //         queue.Enqueue(element);
    //         
    //         return;
    //     }
    //     // si no está esa key, entonces tenemos que insertarla y crear una nueva queue para esa llave, y meter 
    //     // el nuevo elemento en esa queue.
    //     Queue<TElement> newQueue = new Queue<TElement>();
    //     newQueue.Enqueue(element);
    //     _data.Add(priority, newQueue);
    // }
    
    // Sacar elementos
    public TElement Dequeue()
    {
        if(_data.Count == 0)
        {
            Debug.LogError("Trató de desencolar cuando está vacía la Priority Queue.");
            return default(TElement); // como en templates NO hay una forma de regresar un elemento inválido porque puede ser cualquier tipo de dato.
        }
        
        // Si sí hay cosas en la priority queue, quitamos y devolvemos el primer elemento de la Queue con la Key
        // con menor valor
        // Primero obtenemos la Key más baja.
        TPriority lowestKey = _data.Keys.Min();
        // Luego obtenemos la Queue de esa llave.
        Queue<TElement> highestPriorityQueue = _data[lowestKey];
        TElement result = highestPriorityQueue.Dequeue();
        
        // Si quedó vacía la Queue, hay que remover la Key del diccionario.
        if (highestPriorityQueue.Count == 0)
        {
            _data.Remove(lowestKey);
        }
        
        return result;
    }

    // si no tiene elementos, entonces está vacía. Si no, no.
    public bool IsEmpty()
    {
        return _data.Count == 0;
    }
    
    // Buscar
    
    // y acceder
    

    // public void ImprimirArray(int []array, int size)
    // {
    //     for (int i = 0; i < size; i++)
    //     {
    //         Debug.Log(array[i]);
    //     }
    // }
    //
    // public void ImprimirArray(float []array, int size)
    // {
    //     for (int i = 0; i < size; i++)
    //     {
    //         Debug.Log(array[i]);
    //     }
    // }
    //
    // public void ImprimirArray(char []array, int size)
    // {
    //     for (int i = 0; i < size; i++)
    //     {
    //         Debug.Log(array[i]);
    //     }
    // }
    //
    // void EjemploDeFuncionesIgualesPeroConParametrosDistintos()
    // {
    //     float[] arrayFloats = {1, 2, 3, 4, 5 };
    //     ImprimirArray(arrayFloats, 5);
    //     
    //     char[] arrayChars = {'1', '2', '3', '4', '5' };
    //     ImprimirArray(arrayChars, 5);
    // }
    
    
    
}

public class AStarPriorityQueue
{
    private SortedDictionary<float, List<Node>> _data = new SortedDictionary<float, List<Node>>();
    
    public void Enqueue(Node element, float priority)
    {
        // Ver si ya está en el diccionario esa llave.
        if (_data.TryGetValue(priority, out List<Node> list))
        {
            Debug.Log($"Adding node X{element.X},Y{element.Y} to list with priority: {priority}");
            list.Add(element);
            list.Sort((p1, p2) => p1.HCost.CompareTo(p2.HCost) );
            string order = "Order after sorting";
            foreach (var node in list)
            {
                order += $"X{node.X},Y{node.Y}-> Hcost: {node.HCost}, TotalCost: {node.TotalCost}; ";
            }
            Debug.Log(order);
            return;
        }
        // si no está esa key, entonces tenemos que insertarla y crear una nueva queue para esa llave, y meter 
        // el nuevo elemento en esa queue.
        List<Node> newList = new List<Node>();
        newList.Add(element);
        _data.Add(priority, newList);
    }
    
    public Node Dequeue()
    {
        if(_data.Count == 0)
        {
            Debug.LogError("Trató de desencolar cuando está vacía la Priority Queue.");
            return null; // como en templates NO hay una forma de regresar un elemento inválido porque puede ser cualquier tipo de dato.
        }
        
        // Si sí hay cosas en la priority queue, quitamos y devolvemos el primer elemento de la Queue con la Key
        // con menor valor
        // Primero obtenemos la Key más baja.
        float lowestKey = _data.Keys.Min();
        // Luego obtenemos la Queue de esa llave.
        List<Node> highestPriorityList = _data[lowestKey];
        Node result = highestPriorityList.First();
        highestPriorityList.RemoveAt(0);
        
        // Si quedó vacía la Queue, hay que remover la Key del diccionario.
        if (highestPriorityList.Count == 0)
        {
            _data.Remove(lowestKey);
        }
        
        Debug.LogWarning($"Dequeueing node with priority {lowestKey}, X{result.X},Y{result.Y} -> HCost: {result.HCost}, TotalCost: {result.TotalCost}");
        
        return result;
    }
    
    // si no tiene elementos, entonces está vacía. Si no, no.
    public bool IsEmpty()
    {
        return _data.Count == 0;
    }
}
