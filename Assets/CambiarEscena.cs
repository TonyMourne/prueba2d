using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CambiarEscena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panelCARGADO;
    public GameObject panelGENERACION;

    public GameObject panel_fijo_GC;

    public GameObject panelMENU_INICIO;
    public textmanagement tm;
    
    public BotonEliminarScript BES;

    public LibraryManager GPE;
    public GameObject filtroPanel;
    public GameObject inputFiltro;
    Vector2 posicionCentro = new Vector2(0, 0);
    Vector2 posicionFuera = new Vector2(-3500, 0);

    public ManejadorBotonesInicio MBI;
    int modo = 0; 
    // modo = 1 : Generacion
    // modo = 2 : CargadoElemento
    // modo = 3 : MenuInicio

    void Start()
    {
        filtroPanel.SetActive(false);
        inputFiltro.SetActive(false);
        CambiarEscenaInicio();
    }

    public void CambiarEscenaInicio()
    {
        GPE.SetEstamosEnCarga(false);
        tm.LimpiarTodo();
        // escondemos otra vez los filtros
        filtroPanel.SetActive(false);
        inputFiltro.SetActive(false);
        MBI.empezar();
        SwapEscenario(3);

        
    }

    public void OnGeneracionButtonPressed()
    {
        SwapEscenario(1);
        GPE.SetEstamosEnCarga(false);
        tm.LimpiarTodo();
        BES.DesactivarEliminado();
        // escondemos otra vez los filtros
        filtroPanel.SetActive(false);
        inputFiltro.SetActive(false);
    }


    public IEnumerator OnCargadoElementosButtonPressedCoroutine()
    {
        SwapEscenario(2);
        GPE.SetEstamosEnCarga(true);
        yield return StartCoroutine(GPE.StartLuegoDeCambiarEscena(false));
    }
    public void OnCargadoElementosButtonPressed()
    {
        SwapEscenario(2);
        GPE.SetEstamosEnCarga(true);
        StartCoroutine(GPE.StartLuegoDeCambiarEscena(true));
    }

    private void SwapEscenario(int nuevomodo)
    {
        if (modo==nuevomodo)
        {
            return;
        }
        modo = nuevomodo;
        if (modo==1)
        {
            CambiarEscenaGeneracion();
        }
        else if (modo==2)
        {
            CambiarEscenaCargadoElemento();
        }
        else if (modo==3)
        {
            CambiarEscenaMenuInicio();
        }
        else
        {
            Debug.Log("Este caso es imposible de darse en el cambiado de modo");
        }
    }
    private void CambiarEscenaGeneracion()
    {


        panelGENERACION.GetComponent<RectTransform>().anchoredPosition = posicionCentro;
        panelCARGADO.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panelMENU_INICIO.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panel_fijo_GC.GetComponent<RectTransform>().anchoredPosition = posicionCentro;

    }

    private void CambiarEscenaCargadoElemento()
    {

        panelGENERACION.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panelCARGADO.GetComponent<RectTransform>().anchoredPosition = posicionCentro;
        panelMENU_INICIO.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panel_fijo_GC.GetComponent<RectTransform>().anchoredPosition = posicionCentro;
        
    }

    private void CambiarEscenaMenuInicio()
    {
        panelMENU_INICIO.GetComponent<RectTransform>().anchoredPosition = posicionCentro;
        panelGENERACION.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panelCARGADO.GetComponent<RectTransform>().anchoredPosition = posicionFuera;
        panel_fijo_GC.GetComponent<RectTransform>().anchoredPosition = posicionFuera;

    }
}
