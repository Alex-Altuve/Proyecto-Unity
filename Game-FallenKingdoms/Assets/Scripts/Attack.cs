using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentamos obtener el componente Damageable del objeto con el que colisionamos
        Damageable damageable = collision.GetComponent<Damageable>();

        // Intentamos obtener el componente PlayerController del objeto con el que colisionamos
        PlayerController playerController = collision.GetComponent<PlayerController>();

        // Intentamos obtener el componente EsquletonEnemy del objeto con el que colisionamos
        EsqueletonEnemy esqueletonEnemy = collision.GetComponent<EsqueletonEnemy>();

        /*
        if (damageable != null)
        {
            // Aplicamos el daño al objeto con el componente Damageable
            damageable.Hit(attackDamage);
            Debug.Log(collision.name + " hit for " + attackDamage);
        }*/


        //Este es el código que maneja la pérdida de vida del jugador
        // Si el objeto colisionado es el jugador, aplicamos el daño a su salud
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage, knockback);
        }

        // Si el objeto colisionado es el enemigo, aplicamos el daño a su salud
        if (esqueletonEnemy != null)
        {
            esqueletonEnemy.EnemyDamage(attackDamage, knockback);
        }
    }
}
