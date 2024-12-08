using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cainos.LucidEditor;

public class Cofre : MonoBehaviour
{
    [FoldoutGroup("Reference")]
    public Animator animator;
    public GameObject botella; // Prefab de la botella de estamina
    public GameObject luz; // Referencia al objeto de luz
    public float interactionDistance = 2f; // Distancia de interacción
    private Transform player; // Referencia al jugador
    [FoldoutGroup("Reference")]
    public AudioSource audioSource; // Componente AudioSource
    public AudioClip openSound; // Clip de sonido para abrir el cofre

    [FoldoutGroup("Runtime"), ShowInInspector, DisableInEditMode]
    public bool IsOpened
    {
        get { return isOpened; }
        set
        {
            isOpened = value;
            animator.SetBool("IsOpened", isOpened);
            if (isOpened)
            {
                ActivateBottle();
                PlayOpenSound(); // Reproduce el sonido al abrir
            }
            else
            {
                StopOpenSound(); // Detiene el sonido al cerrar
            }
        }
    }
    private bool isOpened;
    private bool hasInteracted; // Para verificar si ya se ha interactuado

    private void Start()
    {
        // Encuentra el objeto jugador por su etiqueta
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Desactiva la botella al inicio
        if (botella != null)
        {
            botella.SetActive(false);
        }
    }

    private void Update()
    {
        // Verifica la distancia al jugador y si se presiona la tecla "F"
        if (!hasInteracted && Vector2.Distance(transform.position, player.position) < interactionDistance && Input.GetKeyDown(KeyCode.F))
        {
            Open();
            hasInteracted = true; // Marca que ya se ha interactuado
        }
    }

    [FoldoutGroup("Runtime"), Button("Open"), HorizontalGroup("Runtime/Button")]
    public void Open()
    {
        IsOpened = true;
    }

    private void PlayOpenSound()
    {
        if (audioSource != null && openSound != null)
        {
            audioSource.clip = openSound; // Asigna el clip de sonido
            audioSource.Play(); // Reproduce el sonido
        }
    }

    private void StopOpenSound()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // Detiene el sonido
        }
    }

    [FoldoutGroup("Runtime"), Button("Close"), HorizontalGroup("Runtime/Button")]
    public void Close()
    {
        IsOpened = false;

        // Desactiva la botella al cerrar el cofre
        if (botella != null)
        {
            botella.SetActive(false);
        }
    }

    private void ActivateBottle()
    {
        if (botella != null)
        {
            // Activa la botella y la levanta
            botella.SetActive(true);
            StartCoroutine(LiftBottle());
        }

        if (luz != null)
        {
            // Levantar la luz
            StartCoroutine(LiftLight());
        }
    }

    private IEnumerator LiftBottle()
    {
        Vector3 startPosition = botella.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up; // Mover hacia arriba en 1 unidad
        float duration = 1f; // Duración del movimiento
        float elapsed = 0f;

        while (elapsed < duration)
        {
            botella.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegúrate de que termine exactamente en la posición objetivo
        botella.transform.position = targetPosition;
    }

    private IEnumerator LiftLight()
    {
        Vector3 startPosition = luz.transform.position;
        Vector3 targetPosition = startPosition + Vector3.up; // Mover hacia arriba en 1 unidad
        float duration = 1f; // Duración del movimiento
        float elapsed = 0f;

        while (elapsed < duration)
        {
            luz.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegúrate de que termine exactamente en la posición objetivo
        luz.transform.position = targetPosition;
    }
}