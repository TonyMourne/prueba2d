using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DropdownColorizer : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public LibraryManager gpe;
    public Color colorMarcado = Color.yellow;
    public Color colorLockeado = new Color(1f, 0.5f, 0f);
    
    private HashSet<string> marcados = new HashSet<string>();
    private string idLockeado = null;
    private bool listaAbiertaAntes = false;

    public void MarcarId(string id)
    {
        marcados.Add(id);
    }

    public void LimpiarMarcados()
    {
        marcados.Clear();
    }

    public void SetLockeado(string id)
    {
        idLockeado = id;
    }

    public void ClearLockeado()
    {
        idLockeado = null;
    }

    void Update()
    {
        GameObject listaObj = GameObject.Find("Dropdown List");
        bool listaAbiertaAhora = listaObj != null;
        if (listaAbiertaAhora && !listaAbiertaAntes)
            StartCoroutine(ColorearItems(listaObj.transform));
        listaAbiertaAntes = listaAbiertaAhora;
    }

    IEnumerator ColorearItems(Transform lista)
    {
        yield return null;
        Transform content = lista.Find("Viewport/Content");
        if (content == null) yield break;

        var ids = gpe.GetIds();

        for (int i = 1; i < content.childCount; i++)
        {
            TMP_Text label = content.GetChild(i).Find("Item Label")?.GetComponent<TMP_Text>();
            if (label == null) continue;

            int dropdownIndex = i - 1;
            if (dropdownIndex >= ids.Count) continue;

            string id = ids[dropdownIndex].id;

            if (marcados.Contains(id))
                label.color = colorMarcado;

            if (idLockeado != null && id == idLockeado)
                label.color = colorLockeado;
        }
    }
}