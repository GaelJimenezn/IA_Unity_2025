// Desarrollado por: Gael (Proyecto Individual)
// Este script implementa un cono de visión y su visualización para un agente.
// Basado en el video "How to Add a Field of View for Your Enemies [Unity Tutorial]" de Comp-3 Interactive.
// Fuente: https://youtu.be/j1-OyLo77ss?si=7B92T-AVf7LUOJh2
using UnityEngine;
using System.Collections;

public class VisionCone : MonoBehaviour
{
    [Header("Configuración de Visión")]
    [Tooltip("La distancia máxima que el agente puede ver.")]
    [Range(0.1f, 50f)]
    public float visionRadius = 10f;

    [Tooltip("El ángulo del cono de visión en grados.")]
    [Range(0, 360)]
    public float visionAngle = 90f;

    [Tooltip("Las capas que el cono de visión debe detectar como objetivos.")]
    public LayerMask targetMask;

    [Tooltip("Las capas que pueden obstruir el cono de visión (por ejemplo, paredes).")]
    public LayerMask obstructionMask;

    [Tooltip("La frecuencia de actualización para la verificación de visión (en segundos).")]
    [Range(0.05f, 1f)]
    public float visionCheckDelay = 0.2f;

    [Header("Visualización del Cono")]
    [Tooltip("El color del cono de visión cuando no se detecta ningún objetivo.")]
    public Color noDetectionColor = Color.green;

    [Tooltip("El color del cono de visión cuando se detecta un objetivo.")]
    public Color detectionColor = Color.red;

    public bool CanSeeTarget { get; private set; }
    public Transform DetectedTarget { get; private set; }

    private Coroutine _visionRoutine;
    private WaitForSeconds _waitDelay;

    void Start()
    {
        _waitDelay = new WaitForSeconds(visionCheckDelay);
        _visionRoutine = StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        while (true)
        {
            yield return _waitDelay;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRadius, targetMask);
        CanSeeTarget = false;
        DetectedTarget = null;

        if (rangeChecks.Length > 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    CanSeeTarget = true;
                    DetectedTarget = target;
                }
            }
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = CanSeeTarget ? detectionColor : noDetectionColor;
        Gizmos.DrawWireSphere(transform.position, visionRadius);

        Quaternion leftRayRotation = Quaternion.AngleAxis(-visionAngle / 2, Vector3.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(visionAngle / 2, Vector3.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(transform.position, leftRayDirection * visionRadius);
        Gizmos.DrawRay(transform.position, rightRayDirection * visionRadius);
        
        if (CanSeeTarget && DetectedTarget != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, DetectedTarget.position);
        }
    }
}