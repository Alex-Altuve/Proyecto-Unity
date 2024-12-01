using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Text healthText; // Referencia al componente Text para mostrar la vida

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateHealthText(health); // Inicializa el texto con el valor máximo
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        UpdateHealthText(health); // Actualiza el texto con el valor actual
    }

    private void UpdateHealthText(int health)
    {
        healthText.text = health.ToString(); // Convertir la salud a texto y mostrarla
    }
}