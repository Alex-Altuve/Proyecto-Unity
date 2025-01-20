using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plataforma : MonoBehaviour
{
    [SerializeField] private Transform[] puntosMovimiento;
    [SerializeField] private float velocidadMovimiento;
    private int siguienteplafatorma = 1;
    private bool ordenPlataforma = true;
   

    private void Update()
    {
        if(ordenPlataforma && siguienteplafatorma + 1 >= puntosMovimiento.Length)
        {
            ordenPlataforma = false;
        }

        if (!ordenPlataforma && siguienteplafatorma  <= 0 )
        {
            ordenPlataforma = true;
        }

        if (Vector2.Distance(transform.position, puntosMovimiento[siguienteplafatorma].position) < 0.1f)
        {
            if (ordenPlataforma)
            {
                siguienteplafatorma += 1;
            }
            else
            {
                siguienteplafatorma -= 1;
            }
                
        }
        transform.position = Vector2.MoveTowards(transform.position, puntosMovimiento[siguienteplafatorma].position, velocidadMovimiento * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(this.transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
