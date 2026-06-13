using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class EditButtonScriptt : MonoBehaviour
{
    
    public TMP_InputField textoSalida;
    public TMP_InputField textoSalida2;
    public Button botonGuardar;
    public Button botonEliminar;
    private bool EditarAvailable=false;
    public void Start()
    {
        textoSalida.interactable = false;
        textoSalida2.interactable = false;
    }

    public void setEditarAvailable(bool modo)
    {
        EditarAvailable=modo;
    }
    public void onButtonPressed()
    {
        if (EditarAvailable==false)
        {
            Debug.Log("Hace falta cargar un elemento antes de editar algo");
            textoSalida.text="Hace falta cargar un elemento antes de editar algo";
            return;
        }

        if (textoSalida.interactable==false)
        {
            textoSalida.interactable=true;
            textoSalida2.interactable=true;
            botonGuardar.gameObject.SetActive(true);
            botonEliminar.gameObject.SetActive(true);
            // deja escribir y activa el boton de guardar
        }
        else
        {
            textoSalida.interactable=false;
            textoSalida2.interactable=false;
            botonGuardar.gameObject.SetActive(false);
            botonEliminar.gameObject.SetActive(false);
            // lo contrario
        }
        
    }
}
