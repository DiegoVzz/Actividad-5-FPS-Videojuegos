using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Configuración")]
    public int maxHealth = 3;

    public int currentHealth;
    private Renderer enemyRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        UpdateColor();
    }

    public void TakeDamage(int damageAmount = 1)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateColor();

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateColor()
    {
        Color newColor = Color.white;

        switch (currentHealth)
        {
            case 3:
                newColor = Color.green;
                break;
            case 2:
                newColor = Color.yellow;
                break;
            case 1:
                newColor = Color.red;
                break;
        }

        enemyRenderer.material.color = newColor;
    }
}