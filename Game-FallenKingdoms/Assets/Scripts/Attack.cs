using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentamos obtener el componente Damageable del objeto con el que colisionamos
        Damageable damageable = collision.GetComponent<Damageable>();

        // Intentamos obtener el componente PlayerController del objeto con el que colisionamos
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (damageable != null)
        {
            // Aplicamos el da�o al objeto con el componente Damageable
            damageable.Hit(attackDamage);
            Debug.Log(collision.name + " hit for " + attackDamage);
        }

        // Si el objeto colisionado es el jugador, aplicamos el da�o a su salud
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
        }
    }
}
