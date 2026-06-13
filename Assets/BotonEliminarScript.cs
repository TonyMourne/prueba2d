using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class BotonEliminarScript : MonoBehaviour
{
    public Button botonEliminar;
    public TMP_InputField TextoAdvertenciaEliminar;
    public Button botonEliminarConfirmacion;
    public LibraryManager GPE;
    public textmanagement tm;
    public TMP_InputField inputFiltro;
    private bool modo = false;

    void Start()
    {
        botonEliminar.gameObject.SetActive(modo);
        TextoAdvertenciaEliminar.gameObject.SetActive(false);
    }

    public void ActivarConfirmacionEliminar()
    {
        Debug.Log("Movemos texto confirmar eliminar");
        modo = !modo;
        TextoAdvertenciaEliminar.gameObject.SetActive(modo);

        if (modo)
        {
            TextoAdvertenciaEliminar.text = "Seguro que quieres eliminar?";
            TextoAdvertenciaEliminar.interactable = false;
        }
    }

    public void DesactivarEliminado()
    {
        Debug.Log("desactivamos eliminar confirmacion");
        modo = false;
        TextoAdvertenciaEliminar.gameObject.SetActive(false);
    }

    public void EliminarElemento()
    {
        Debug.Log("Eliminando elemento");
        var (id, tipo) = GPE.ObtenerIdSeleccionado();
        string baseUrl = "";

        switch (tipo)
        {
            case "P":
                baseUrl = "http://localhost:8080/api/personajes/";
                break;
            case "L":
                baseUrl = "http://localhost:8080/api/lugar/";
                break;
            case "C":
                baseUrl = "http://localhost:8080/api/campania/";
                break;
            default:
                Debug.LogError("Tipo desconocido: " + tipo);
                return;
        }

        string url = baseUrl + id;
        StartCoroutine(EliminarRequest(url));
    }

    IEnumerator EliminarRequest(string url)
    {
        UnityWebRequest request = UnityWebRequest.Delete(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Elemento eliminado correctamente");
            tm.LimpiarTodo();
            Debug.Log("Tras LimpiarTodo");
        }
        else
        {
            Debug.LogError("Error al eliminar: " + request.error);
        }

        DesactivarEliminado();
        inputFiltro.text="";
        GPE.reload();
        Debug.Log("reloading tras eliminacion");
    }
}