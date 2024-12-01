using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    public Slider slider;
    public Text staminaText; // Referencia al componente Text para mostrar la estamina

    public void SetMaxStamina(int stamina)
    {
        slider.maxValue = stamina;
        slider.value = stamina;
        UpdateStaminaText(stamina); // Inicializa el texto con el valor máximo
    }

    public void SetStamina(int stamina)
    {
        slider.value = stamina;
        UpdateStaminaText(stamina); // Actualiza el texto con el valor actual
    }

    private void UpdateStaminaText(int stamina)
    {
        staminaText.text = stamina.ToString(); // Convertir la estamina a texto y mostrarla
    }
}