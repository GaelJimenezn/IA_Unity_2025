// PlayerMovement.cs
//
//Integrantes del equipo: Gael (individual)
//
// Descripción: Permite al jugador mover un GameObject usando las teclas direccionales (WASD).
// El personaje rota para mirar en la dirección del movimiento.
//
// Fuentes:
// - Unity Manual: Input
// - Unity API: Rigidbody, Transform, Vector3, Quaternion

using UnityEngine;

/// <summary>
/// Proporciona un controlador de movimiento básico para el jugador usando las teclas.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Velocidad de movimiento del jugador.")]
    public float moveSpeed = 5.0f; 

    // Referencia al Rigidbody del jugador.
    public Rigidbody rb;

    void FixedUpdate()
    {
        // Obtiene la entrada horizontal y vertical del Input Manager de Unity.
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D o flechas izquierda/derecha.
        float verticalInput = Input.GetAxis("Vertical"); // W/S o flechas arriba/abajo.

        // Crea un vector de movimiento en el plano XZ (horizontal) y lo normaliza para tener una dirección consistente.
        Vector3 movement = new Vector3(horizontalInput, 0.0f, verticalInput).normalized;

        // Mueve la posición del personaje según la dirección, velocidad y el tiempo del frame.
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        
        // Si hay movimiento, rota el personaje para que mire en esa dirección.
        if (movement != Vector3.zero)
        {
            // Crea una rotación que mira en la dirección del movimiento.
            Quaternion newRotation = Quaternion.LookRotation(movement);
            // Suaviza la rotación para evitar cambios bruscos y aplica la rotación al transform del personaje.
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, newRotation, Time.fixedDeltaTime * 10f));
        }
    }
}