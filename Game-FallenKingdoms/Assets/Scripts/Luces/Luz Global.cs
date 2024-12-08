using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuzGlobal : MonoBehaviour
{
    public Light2D globalLight; // Referencia a la luz global
    public Color bloodColor = new Color(1f, 0f, 0f, 1f); // Color sangre
    public float maxIntensity = 1f; // Intensidad máxima
    public float minIntensity = 0.1f; // Intensidad mínima

    private void Start()
    {
        if (globalLight == null)
        {
            globalLight = GetComponent<Light2D>(); // Obtén el componente de luz
        }
    }

    // Método para actualizar la luz global basado en la salud
    public void UpdateGlobalLight(int currentHealth, int maxHealth)
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        globalLight.color = bloodColor; // Cambia el color
        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, healthPercentage); // Ajusta la intensidad
    }
}