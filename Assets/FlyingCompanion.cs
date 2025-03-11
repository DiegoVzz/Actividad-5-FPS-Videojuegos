using UnityEngine;

public class FlyingCompanion : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;           // Jugador
    public Transform cameraTransform;  // Asigna la cámara del jugador
    public float followSpeed = 5f;
    public float sideOffset = 2f;      // Derecha/Izquierda de la cámara
    public float minDistance = 1.5f;
    public float heightOffset = 1f;
    public float floatIntensity = 0.2f;

    private Vector3 targetPosition;

    void Update()
    {
        if (player == null || cameraTransform == null) return;

        // Efecto de flotar
        float floatingOffset = Mathf.Sin(Time.time * 2f) * floatIntensity;

        // Dirección lateral RELATIVA A LA CÁMARA (no al jugador)
        Vector3 cameraRight = cameraTransform.right;
        cameraRight.y = 0; // Ignora la altura de la cámara
        cameraRight.Normalize();

        // Posición objetivo (lateral + altura + flotación)
        Vector3 sidePosition = cameraRight * sideOffset;
        targetPosition = player.position + sidePosition + Vector3.up * (heightOffset + floatingOffset);

        // Mantener distancia mínima del jugador
        float currentDistance = Vector3.Distance(transform.position, player.position);
        if (currentDistance < minDistance)
        {
            Vector3 retreatDirection = (transform.position - player.position).normalized;
            targetPosition += retreatDirection * (minDistance - currentDistance);
        }

        // Movimiento suave
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );

        // Rotación: Mira hacia la CÁMARA (no al jugador)
        Vector3 lookDirection = cameraTransform.position - transform.position;
        lookDirection.y = 0; // Opcional: bloquear rotación vertical
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            followSpeed * Time.deltaTime
        );
    }
}