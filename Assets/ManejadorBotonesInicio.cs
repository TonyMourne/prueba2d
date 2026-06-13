using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ManejadorBotonesInicio : MonoBehaviour
{
    public Image Tablero2;
    public Image Tablero1;

    public Image logo;

    public Button botonP;
    public Button botonL;
    public Button botonC;

    public Button BotonGenerar;
    public Button BotonCargar;

    private int modo = -1;

    public CambiarEscena CE;

    public GenerarOrquest GO;
    public LibraryManager GPE;

    void Start()
    {
        empezar();
    }

    public void empezar()
    {
        logo.gameObject.SetActive(true);
        Tablero1.gameObject.SetActive(true);
        BotonGenerar.gameObject.SetActive(true);
        BotonCargar.gameObject.SetActive(true);
        Tablero2.gameObject.SetActive(false);
        botonP.gameObject.SetActive(false);
        botonC.gameObject.SetActive(false);
        botonL.gameObject.SetActive(false);
    }

    public void Setmodo(int m)
    {
        modo = m;
    }

    public int Getmodo()
    {
        return modo;
    }

    public void OnButtonPressedGenerar()
    {
        Setmodo(0);
        Tablero2.gameObject.SetActive(true);
        botonP.gameObject.SetActive(true);
        botonC.gameObject.SetActive(true);
        botonL.gameObject.SetActive(true);
    }

    public void OnButtonPressedCargar()
    {
        Setmodo(1);
        GPE.SetEstamosEnCarga(true);
        Tablero2.gameObject.SetActive(true);
        botonP.gameObject.SetActive(true);
        botonC.gameObject.SetActive(true);
        botonL.gameObject.SetActive(true);
    }

    public void BotonPERSONAJE()
    {
        StartCoroutine(BotonPersonajeCoroutine());
    }

    public void BotonLUGAR()
    {
        StartCoroutine(BotonLugarCoroutine());
    }

    public void BotonCAMPANIA()
    {
        StartCoroutine(BotonCampaniaCoroutine());
    }

    IEnumerator BotonPersonajeCoroutine()
    {
        if (modo == 0)
        {
            CE.OnGeneracionButtonPressed();
            GO.cambiarmodo(1);
        }
        else if (modo == 1)
        {
            yield return StartCoroutine(CE.OnCargadoElementosButtonPressedCoroutine());
            GPE.BotonPersonajes();
        }
    }

    IEnumerator BotonLugarCoroutine()
    {
        if (modo == 0)
        {
            CE.OnGeneracionButtonPressed();
            GO.cambiarmodo(2);
        }
        else if (modo == 1)
        {
            yield return StartCoroutine(CE.OnCargadoElementosButtonPressedCoroutine());
            GPE.BotonLugares();
        }
    }

    IEnumerator BotonCampaniaCoroutine()
    {
        if (modo == 0)
        {
            CE.OnGeneracionButtonPressed();
            GO.cambiarmodo(3);
        }
        else if (modo == 1)
        {
            yield return StartCoroutine(CE.OnCargadoElementosButtonPressedCoroutine());
            GPE.BotonCampanias();
        }
    }
}