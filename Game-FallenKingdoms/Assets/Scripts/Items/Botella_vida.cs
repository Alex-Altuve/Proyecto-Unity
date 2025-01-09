using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlePickupVida : MonoBehaviour
{
    public int VidaIncrease = 20; // Incremento de estamina
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
            playerController.maxHealth += VidaIncrease; // Aumentar el máximo de estamina
            playerController.currentHealth += VidaIncrease; // Restablecer la estamina actual
            playerController.healthBar.SetHealth(playerController.currentHealth);
            // Destruir la luz, si está asignada
            if (luz != null)
            {
                Destroy(luz); // Destruir el objeto de luz
            }
        
            Destroy(gameObject); // Destruir la botella después de recogerla
        }
    }
}