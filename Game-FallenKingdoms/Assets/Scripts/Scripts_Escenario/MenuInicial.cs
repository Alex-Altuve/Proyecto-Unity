using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
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
    public void IntruccionesIr()
    {
        SceneManager.LoadScene("Instrucciones");
    }
}
