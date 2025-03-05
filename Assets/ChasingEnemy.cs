using UnityEngine;

public class ChasingEnemy : MonoBehaviour
{
    [Header("Configuración")]
    public float moveSpeed = 3f;
    public float detectionRange = 3f;

    private Transform player;
    private EnemyHealth health;

    void Start()
    {
        // Busca al jugador de forma segura
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró objeto con tag 'Player'");
        }

        health = GetComponent<EnemyHealth>();
        if (health == null)
        {
            Debug.LogError("Falta componente EnemyHealth en el enemigo");
        }
    }

    void Update()
    {
        if (health == null || player == null) return;

        if (health.currentHealth > 0)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }
}