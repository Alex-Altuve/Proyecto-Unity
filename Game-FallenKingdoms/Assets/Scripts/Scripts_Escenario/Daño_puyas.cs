using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageZone : MonoBehaviour
{
    public int damageAmount = 20; // Daño que causará al jugador
    private Coroutine damageCoroutine; // Para almacenar la coroutine

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica si el objeto que colisiona tiene la etiqueta "Player"
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Inicia la coroutine para aplicar daño
                damageCoroutine = StartCoroutine(ApplyDamage(player));
                Debug.Log("El jugador ha entrado en la zona de daño.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Detiene el daño cuando el jugador sale de la zona
        if (other.CompareTag("Player"))
        {
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
                Debug.Log("El jugador ha salido de la zona de daño.");
            }
        }
    }

    private IEnumerator ApplyDamage(PlayerController player)
    {
        while (true) // Bucle infinito que se detiene cuando se sale de la zona
        {
            player.TakeDamage(damageAmount); // Aplica el daño
            yield return new WaitForSeconds(1.5f); // Espera 1.5 segundos
        }
    }
}