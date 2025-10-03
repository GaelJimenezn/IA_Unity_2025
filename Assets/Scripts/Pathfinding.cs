using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


// las structs son POD (Plain Old Data), se manejan como si fueran int, float, etc. y no como punteros internamente.
// En contraste, las class siempre se manejan como punteros.

public enum EDirections : int
{
    Up = -1,
    Right = 1,
    Left = -1,
    Down = 1
}

public class Node
{
    public Node()
    {
        Parent = null;
    }
        
    public Node(int x, int y, bool isWalkable = true)
    {
        X = x;
        Y = y;
        
        // Hay que tener la certeza de que inicializa en null, para hacer el pathfinding bien.
        Parent = null;
        Walkable = isWalkable;
    }
    
    // Tienen información de algo relevante en un grafo... ¿pero qué?
    public int X { get; }

    public int Y { get; }

    public bool Walkable = true; 

    // aristas que nos dicen a qué nodos puede visitar este nodo.
    // private List<Edge> edges = new List<Edge>();
    /*
     * public edge UpEdge = this, upNode;
     * public edge RightEdge = this, RightNode;
     * public edge DownEdge = this, downNode;
     * public edge LeftEdge = this, leftNode;
     * 

     * public Node UpNode;
     * public Node RightNode;
     * public Node DownNode;
     * public Node LeftNode;
     *
     * Ni siquiera necesitamos esos Node de arriba, abajo, derecha, izquierda, porque los índices en la cuadrícula
     * ya nos dicen cuáles serían dichos vecinos arriba (-1 en Y), abajo (+1 en Y), derecha (+1 en X), izquierda (-1 en X)
     */
    
    // Tiene una referencia al nodo Parent, es el que lo metió al árbol. (El que lo metió a la lista abierta).
    public Node Parent;
}

// No la vamos a usar en este caso, porque están implícitas las aristas en la estructura del grid.
// Si tuviéramos otro tipo de representación, como un grafo no-grid, ahí sí necesitaríamos tener aristas explícitas.
// public class Edge
// {
//     public Node A;
//     public Node B;
//     // podríamos poner que tiene un costo moverse entre ellos o algo.
//     // Pero ahorita lo voy a dejar así.
// }


public class Pathfinding : MonoBehaviour
{
    [SerializeField] private int height = 5;
    [SerializeField] private int width = 5;
    
    [SerializeField] private int originX = 1;
    [SerializeField] private int originY = 1;

    [SerializeField] private int goalX = 3;
    [SerializeField] private int goalY = 3;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float walkableProbability = 0.5f;
    
    // Alguien que contenga todos los Nodos.
    // Esos nodos van a estar en forma de Grid/Cuadrícula, entonces podemos usar un array bidimensional.
    private Node[][] _grid;
    
    // Algoritmo para inicializar el grid de width*height
    void InitializeGrid()
    {
        _grid = new Node[height][];
        // Primero eje vertical, 
        for (int i = 0; i < height; i++)
        {
            _grid[i] = new Node[width];
            // después el eje horizontal
            for (int j = 0; j < width; j++)
            {
                bool isWalkable = Random.value < walkableProbability;
                
                // NOTA: Entre más anidado (interno, profundo) esté el for, más a la derecha va en el corchete su índice.
                _grid[i][j] = new Node(j, i, isWalkable);
            }
        }
    }

    /// <summary>
    /// Algoritmo que nos dice si hay un camino desde un Nodo origen hacia un Nodo objetivo.
    /// </summary>
    /// <returns></returns>
    private bool DepthFirstSearchRecursive(Node origin, Node goal)
    {
        // Los algoritmos de pathfinding construyen un árbol con la información del grafo.
        // Lo primero que tenemos que establecer es ¿Cuál es la raíz del árbol?
        // La raíz del árbol siempre va a ser el punto de origen del algoritmo.
        // Node root = origin; // su padre tiene que ser null
        
        // si esto es recursivo, ¿Qué valor es el que vamos "avanzando"?
        // la meta no va a cambiar, entonces lo único que podemos avanzar es el origen
        
        // ¿Cuándo se detiene esta recursión? Hay dos:
        // A) cuando llegamos a la meta.
        if (origin == goal)
            return true; // sí hubo camino
        
        // B) cuando no hay camino.

        int x = origin.X;
        int y = origin.Y;
        
        // Mandamos a llamar la misma función, pero un paso más adelante en el tiempo.
        // Primero checamos arriba:
        if(CheckNode(origin, 0, EDirections.Up, goal))
            return true;
        
        // checamos derecha:
        if(CheckNode(origin, EDirections.Right, 0, goal))
            return true;

        // nodo de la izquierda
        if(CheckNode(origin, EDirections.Left, 0, goal))
            return true;
        
        // cambia la comparación de si se puede ir a esa dirección o no; hacia donde se obtiene el nodo vecino, 
        // Nodo de abajo
        if(CheckNode(origin, 0, EDirections.Down, goal))
            return true;
        
        // si se acabó la recursión y nunca llegaste a la meta, es que no hay camino.
        return false; 
    }

    private bool CheckNode(Node current, EDirections xOffset, EDirections yOffset, Node goal)
    {
        return CheckNode(current, (int)xOffset, (int)yOffset, goal);
    }

    private bool CheckNode(Node current, int xOffset, int yOffset, Node goal)
    {
        if (current.Y + yOffset >= height
            || current.Y + yOffset < 0
            || current.X + xOffset >= width
            || current.X + xOffset < 0) 
            return false; // si se sale del grid, entonces no es nodo válido.
        
        Node neighborNode = _grid[current.Y + yOffset][current.X + xOffset];
        if (neighborNode.Parent == null && neighborNode.Walkable) // tiene que ser un nodo no-visitado Y que sea caminable.
        {
            neighborNode.Parent = current;
            // Si eso fue true, regresamos true
            if (DepthFirstSearchRecursive(neighborNode, goal))
            {
                Debug.Log($"el nodo {current.X},{current.Y} fue parte del camino.");
                return true;
            }
        }

        return false;
    }

    
    private Node CheckNode(Node current, int xOffset, int yOffset)
    {
        if (current.Y + yOffset >= height
            || current.Y + yOffset < 0
            || current.X + xOffset >= width
            || current.X + xOffset < 0) 
            return null; // si se sale del grid, entonces no es nodo válido.
        
        Node neighborNode = _grid[current.Y + yOffset][current.X + xOffset];
        if (neighborNode.Parent == null && neighborNode.Walkable) // tiene que ser un nodo no-visitado Y que sea caminable.
        {
            neighborNode.Parent = current;
            // Si eso fue true, regresamos true
            return neighborNode;
        }

        return null;
    }

    private bool DepthFirstSearchIterative(Node origin, Node goal)
    {
        // Ponerle al nodo raíz que él es su propio padre:
        origin.Parent = origin; // esto ya lo hace SetupGrid(); Lo dejo por pura claridad.
        
        // Nodo actual, que representa al parámetro Origin en la versión recursiva, 
        // es el que representa el "avance" en el algoritmo.
        Node current = origin;

        // Necesitamos un registro de cuáles nodos ya se han "conocido" o "abierto" en el algoritmo.
        // Aquí necesitamos memoria, algo que almacene cuáles nodos ya conocimos, para diferenciarlos de cuáles no.
        Stack<Node> openList = new Stack<Node>();
        // lista abierta son los nodos que se sabe que existen, pero todavía visitan.
        
        // los nodos "visitados" o explorados, o expandidos, se les conoce como "Cerrados"
        // Si cerraste todos los nodos, y no llegaste a la meta, quiere decir que no hay camino.
        // List<Node> closedList = new List<Node>();
        
        // La lista abierta empieza con el origen dentro
        openList.Push(origin);
        
        // mientras nuestro nodo actual no sea nuestro nodo meta, Y mientras todavía haya nodos por explorar,
        // entonces le seguimos.
        while (current != goal && openList.Count > 0)
        {
            // revisar el del tope de la pila, SIN SACARLO, porque esto es como una pila de llamadas, no se termina
            // de procesar ese nodo hasta que se terminen de procesar todos los que irían encima de él en la pila de llamadas.
            current = openList.Peek();
            
            // metemos elementos en la pila de abiertos.
            // Metemos nodos, que sean vecinos de current (arriba, abajo, izquierda, derecha),
            // y que su parent sea null, y él sea caminable.

            int x = current.X;
            int y = current.Y;
            // checamos nodo de arriba
            Node upNode = CheckNode(current, 0, (int)EDirections.Up);
            if (upNode != null)
            {
                openList.Push(upNode);
                continue;
            }

            Node rightNode = CheckNode(current, (int)EDirections.Right, 0);
            if (rightNode != null)
            {
                openList.Push(rightNode);
                continue;
            }
            
            Node downNode = CheckNode(current, 0, (int)EDirections.Down);
            if (downNode != null)
            {
                openList.Push(downNode);
                continue;
            }
            
            Node leftNode = CheckNode(current, (int)EDirections.Left, 0);
            if (leftNode != null)
            {
                openList.Push(leftNode);
                continue;
            }
            
            // esto de aquí hace que su antecesor continue justo donde se había quedado.
            openList.Pop();
        }
        
        // necesitamos revisar por cuál de las condiciones se rompió el while.
        if (current == goal)
            return true; // si sí se llegó a la meta, sí hubo un camino.
        
        // si no, pos no.
        return false;
    }

    
    
    
    // recursivo VS iterativo
    // Recursivo es que se manda a llamar a sí mismo dentro del cuerpo de la función.
    public int RestaHastaCero(int value)
    {
        // stopping condition. La condición que hace que la recursión se detenga.
        if (value < 0)
            return value; // aquí ya no va a volver a mandar a llamarse, y entonces se acaba la recursión.
        
        Debug.Log($"Conteo regresivo: {value}");
        return RestaHastaCero(value - 1);
    }


    void SetupGrid()
    {
        // asegurarse de que tanto el origen como la meta son caminables.
        _grid[originY][originX].Walkable = true;
        _grid[goalY][goalX].Walkable = true;

        // le ponemos que es su propio padre.
        _grid[originY][originX].Parent = _grid[originY][originX];

    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RestaHastaCero(10);
        // Iterativo:
        for (int i = 10; i < 0; i--)
        {
            Debug.Log($"Conteo regresivo: {i}");
        }
        
        // Todo lo que se puede hacer iterativo se puede hacer recursivo y viceversa.

        // Inicializamos nuestra cuadrícula antes de mandar a llamar cualquier algoritmo de pathfinding.
        InitializeGrid();
        SetupGrid();
        
        // él es su propio parent, de lo contrario los otros nodos lo toman como que no ha sido visitado y lo usan 
        // para el pathfinding.
        _grid[originY][originX].Parent = _grid[originY][originX];
        //if (DepthFirstSearchRecursive(_grid[originY][originX], _grid[goalY][goalX]))
        if (DepthFirstSearchIterative(_grid[originY][originX], _grid[goalY][goalX]))
        {
            Debug.Log($"Sí se encontró camino desde {originX},{originY}, hasta {goalX},{goalY}");
        }
        else
        {
            Debug.Log($"No se encontró camino desde {originX},{originY}, hasta {goalX},{goalY}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (_grid == null)
            return;

        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // para cada cuadro, vamos a dibujar un cubo con cierto color. 
                // Blanco para los caminables, y magenta para los no caminables.
                if (_grid[y][x].Walkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.black;
                }

                Gizmos.DrawCube(new Vector3(x, -y, 0.0f), Vector3.one * 0.8f);

                if (_grid[y][x].Parent != null)
                {
                    Gizmos.color = Color.magenta;
                    // entonces que nos dibuje una línea desde el parent hasta este nodo.
                    Gizmos.DrawLine(new Vector3(_grid[y][x].Parent.X, -_grid[y][x].Parent.Y, 1.0f), new Vector3(x, -y, 1.0f));
                    // le ponemos una esfera chiquita al nodo hijo, para diferenciar quién es padre y quien es hijo.
                    Gizmos.DrawSphere(new Vector3(x, -y, 1.0f), 0.25f);
                }
            }
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(originX, -originY, 0.0f), Vector3.one * 0.9f);

        Gizmos.color = Color.red;
        Gizmos.DrawCube(new Vector3(goalX, -goalY, 0.0f), Vector3.one * 0.9f);
        
    }
}
