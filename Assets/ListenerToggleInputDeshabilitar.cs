using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListenerToggleInputDeshabilitar : MonoBehaviour
{
    [Header("Listas (mismo orden e índice)")]
    public List<Toggle> toggles;
    public List<TMP_InputField> inputFields;

    [Header("Colores")]
    public Color fondoActivo    = Color.white;
    public Color fondoDesactivo = new Color(0.75f, 0.75f, 0.75f, 1f);
    public Color textoActivo    = Color.black; // <-- texto oscuro
    public Color textoDesactivo = new Color(0.5f, 0.5f, 0.5f, 1f);
    private void Start()
    {
        for (int i = 0; i < toggles.Count; i++)
        {
            int index = i; // captura local para el closure
            toggles[i].onValueChanged.AddListener((isOn) => OnToggleChanged(index, isOn));

            // Estado inicial
            AplicarEstado(i, toggles[i].isOn);
        }
    }

    private void OnToggleChanged(int index, bool isOn)
    {
        if (index >= inputFields.Count) return;
        AplicarEstado(index, isOn);
    }

    private void AplicarEstado(int index, bool isOn)
    {
        TMP_InputField campo = inputFields[index];

        campo.interactable = isOn;

        campo.textComponent.color = isOn ? textoActivo : textoDesactivo;

        Image fondo = campo.GetComponent<Image>();
        if (fondo != null)
            fondo.color = isOn ? fondoActivo : fondoDesactivo;
    }
}
