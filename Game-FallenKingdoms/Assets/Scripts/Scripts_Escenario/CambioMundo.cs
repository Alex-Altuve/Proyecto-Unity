using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioMundo : MonoBehaviour
{
    public static string lastScene;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ModoHistoria()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void Salir()
    {
        /// Estos se pondra cuando se cree el build
        //Debug.Log("Salir");
        //Application.Quit();

        /// esto es para sacarlo del modo game
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                    Application.Quit();
        #endif
    }
    public void MundoNuevo()
    {
        SceneManager.LoadScene("Mundo2");
    }
    public void TryAgain()
    {
        SceneManager.LoadScene("Mundo1");
    }

    public void IntruccionesVolver()
    {
        SceneManager.LoadScene(lastScene);

    }
    public void IrVentana()
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("Instrucciones");
    }

}
