using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PantallaDeCarga : MonoBehaviour
{
    public static PantallaDeCarga Instancia {get; private set; }

    public Image imagenDeCarga;
    [Range(0.01f, 0.1f)]
    public float velocidadAparecer = 0.5f;
    [Range(0.01f, 0.1f)]
    public float velocidadOcultar = 0.5f;
    void Awake()
    {
        DefinirSingleton();
    }

    private void DefinirSingleton()
    {
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(this);
            imagenDeCarga.gameObject.SetActive(true);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void CargarEscena(string nombreEscena)
    {
        StartCoroutine(MostrarPantallaDeCarga(nombreEscena));
    }
    private IEnumerator MostrarPantallaDeCarga(string nombreEscena)
    {
        imagenDeCarga.gameObject.SetActive(true);
        Color c = imagenDeCarga.color;
        c.a = 0.0f;

        //Mientras no esté totalmente visible va aumentando su visibilidad
        while(c.a < 1)
        {
            imagenDeCarga.color = c;
            c.a += velocidadAparecer;
            yield return null;
        }

        //Cargar la escena
        SceneManager.LoadScene(nombreEscena);
        //Espera a que haya cargado la nueva escena
        while (!nombreEscena.Equals(SceneManager.GetActiveScene().name))
        {
            yield return null;
        }

        while(c.a > 0)
        {
            imagenDeCarga.color = c;
            c.a -= velocidadOcultar;
            yield return null;
        }
        imagenDeCarga.gameObject.SetActive(false);
    }
}
