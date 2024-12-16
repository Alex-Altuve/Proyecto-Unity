using UnityEngine;
using UnityEngine.UI;

public class EfectoBlood : MonoBehaviour
{
    public Image bloodImage; // Asigna la imagen de sangre desde el Inspector
    public PlayerController playerController; // Referencia al script del jugador

    private void Start()
    {
        if (playerController != null)
        {
            playerController.OnHealthChanged += UpdateBloodEffect;
        }
    }

    public void UpdateBloodEffect(int currentHealth, int maxHealth)
    {
        float alpha = 1f - (float)currentHealth / maxHealth; // Calcula el alpha basado en la salud
        Color newColor = bloodImage.color;
        newColor.a = alpha; // Actualiza el alpha
        bloodImage.color = newColor;
    }
}