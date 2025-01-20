using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;

    public HealthBar bossHealthBar;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Intentamos obtener el componente Damageable del objeto con el que colisionamos
        Damageable damageable = collision.GetComponent<Damageable>();

        // Intentamos obtener el componente PlayerController del objeto con el que colisionamos
        PlayerController playerController = collision.GetComponent<PlayerController>();

        // Intentamos obtener el componente EsqueletonEnemy del objeto con el que colisionamos
        EsqueletonEnemy esqueletonEnemy = collision.GetComponent<EsqueletonEnemy>();

        // Intentamos obtener el componente BandidoEnemy del objeto con el que colisionamos
        BandidoEnemy bandidoEnemy = collision.GetComponent<BandidoEnemy>();

        // Intentamos obtener el componente BandidoEnemy_BOSS del objeto con el que colisionamos
        BandidoEnemy_BOSS bandidoBoss = collision.GetComponent<BandidoEnemy_BOSS>();

        // Intentamos obtener el componente BossController del objeto con el que colisionamos
        BossController bossController = collision.GetComponent<BossController>();

        // Este es el código que maneja la pérdida de vida del jugador
        // Si el objeto colisionado es el jugador, aplicamos el daño a su salud
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage, knockback);
        }

        // Si el objeto colisionado es el esqueleto, aplicamos el daño a su salud
        if (esqueletonEnemy != null)
        {
            esqueletonEnemy.EnemyDamage(attackDamage, knockback);
        }

        // Si el objeto colisionado es el bandido, aplicamos el daño a su salud
        if (bandidoEnemy != null)
        {
            bandidoEnemy.EnemyDamage(attackDamage, knockback);
        }

        // Si el objeto colisionado es el boss, aplicamos el daño a su salud
        if (bandidoBoss != null)
        {
            Debug.Log("Le pegué al bandido boss");
            bandidoBoss.OnHit(attackDamage, knockback);
        }
    }
}