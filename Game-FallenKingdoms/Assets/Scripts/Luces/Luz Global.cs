using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LuzGlobal : MonoBehaviour
{
    public Light2D globalLight; // Referencia a la luz global
    public Color bloodColor = new Color(1f, 0f, 0f, 1f); // Color sangre
    public float maxIntensity = 1f; // Intensidad m�xima
    public float minIntensity = 0.1f; // Intensidad m�nima

    private void Start()
    {
        if (globalLight == null)
        {
            globalLight = GetComponent<Light2D>(); // Obt�n el componente de luz
        }
    }

    // M�todo para actualizar la luz global basado en la salud
    public void UpdateGlobalLight(int currentHealth, int maxHealth)
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        globalLight.color = bloodColor; // Cambia el color
        globalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, healthPercentage); // Ajusta la intensidad
    }
}