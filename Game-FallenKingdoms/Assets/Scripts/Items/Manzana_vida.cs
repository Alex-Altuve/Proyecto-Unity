using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manzana_Vida : MonoBehaviour
{
    public int vidaIncrease = 15; // Incremento de estamina
    private bool isPlayerInRange = false; // Para verificar si el jugador está cerca
    private PlayerController playerController; // Referencia al PlayerController
    public GameObject luz; // Referencia al objeto de luz que se destruye
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que el objeto tiene la etiqueta "Player"
        {
            isPlayerInRange = true;
            playerController = other.GetComponent<PlayerController>(); // Obtener el PlayerController
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            playerController = null;
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            Pickup();
        }
    }

    void Pickup()
    {
        if (playerController != null)
        {
            playerController.currentHealth += vidaIncrease; // Aumentar el máximo de vida
            if (playerController.currentHealth > playerController.maxHealth)
            {
                playerController.currentHealth = playerController.maxHealth; // No exceder la salud máxima
            }
            playerController.healthBar.SetHealth(playerController.currentHealth);
            //playerController.currentStamina = playerController.maxStamina; // Restablecer la estamina actual

            // Destruir la luz, si está asignada
            if (luz != null)
            {
                Destroy(luz); // Destruir el objeto de luz
            }

            Destroy(gameObject); // Destruir la botella después de recogerla
        }
    }
}