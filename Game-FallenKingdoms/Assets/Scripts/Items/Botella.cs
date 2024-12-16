using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottlePickup : MonoBehaviour
{
    public int staminaIncrease = 20; // Incremento de estamina
    private bool isPlayerInRange = false; // Para verificar si el jugador est� cerca
    private PlayerController playerController; // Referencia al PlayerController
    public GameObject luz; // Referencia al objeto de luz que se destruye
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Aseg�rate de que el objeto tiene la etiqueta "Player"
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
            playerController.maxStamina += staminaIncrease; // Aumentar el m�ximo de estamina
            playerController.currentStamina += staminaIncrease; // Restablecer la estamina actual
            playerController.staminaBar.SetStamina(playerController.currentStamina);
            // Destruir la luz, si est� asignada
            if (luz != null)
            {
                Destroy(luz); // Destruir el objeto de luz
            }
        
            Destroy(gameObject); // Destruir la botella despu�s de recogerla
        }
    }
}