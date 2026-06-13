using UnityEngine;
using UnityEngine.UI;

public class StartHideGuardar : MonoBehaviour
{
    public Button botonGuardar;

    void Start()
    {
        botonGuardar.gameObject.SetActive(false); // siempre va  a estar escondido al empezar la app
    }
}