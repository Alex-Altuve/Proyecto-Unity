
using UnityEngine;

public class Trampolin : MonoBehaviour
{
    public Animator animator; // Asigna el Animator desde el Inspector
    public float fuerza = 5f; // Fuerza con la que se lanzará el objeto
    private bool jugadorEncima = false;

    void Update()
    {
        // Verifica si el jugador está encima del objeto y presiona 'F'
        if (jugadorEncima && Input.GetKeyDown(KeyCode.F))
        {
            // Activa el Animator
            animator.SetBool("isOnTrampo", true); // Asegúrate de tener un trigger llamado "Activar" en tu Animator

            // Obtén la referencia al jugador
            GameObject jugador = GameObject.FindGameObjectWithTag("Player"); // Asegúrate de que tu jugador tenga la etiqueta "Player"
            if (jugador != null)
            {
                // Obtén el Rigidbody2D del jugador
                Rigidbody2D rb = jugador.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // Modifica la velocidad en Y del jugador para lanzarlo hacia arriba
                    rb.velocity = new Vector2(rb.velocity.x, fuerza);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que colisiona es el jugador
        if (collision.CompareTag("Player")) // Asegúrate de que tu jugador tenga la etiqueta "Jugador"
        {
            jugadorEncima = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica si el objeto que salió de la colisión es el jugador
        if (collision.CompareTag("Player"))
        {
            jugadorEncima = false;
            animator.SetBool("isOnTrampo", false); // Asegúrate de tener un trigger llamado "Activar" en tu Animator
        }
    }
}