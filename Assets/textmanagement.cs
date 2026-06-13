using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class textmanagement : MonoBehaviour
{
    public TMP_InputField textoSalida;
    public TMP_InputField textoSalida2;
    private Dictionary<string, string> seccionesOriginales;
    private bool WriteInInput = false;
    private List<Page> paginas;
    private int indiceActual = 0;

    private int maxCaracteresPorPagina = 450;

    private class Page
    {
        public string titulo;
        public string contenido;
    }

    public Dictionary<string, string> ProcesarRespuesta(string textoCompleto)
    {
        seccionesOriginales = DividirEnSecciones(textoCompleto);
        ConstruirPaginasVisuales();
        indiceActual = 0;
        MostrarSeccionActual();
        return seccionesOriginales;
    }

    public void CargarDesdeDiccionario(Dictionary<string, string> diccionario)
    {
        seccionesOriginales = diccionario;
        ConstruirPaginasVisuales();
        indiceActual = 0;
        MostrarSeccionActual();
    }

    void ConstruirPaginasVisuales()
    {
        paginas = new List<Page>();

        foreach (var kvp in seccionesOriginales)
        {
            if (kvp.Key == "ELEMENTOS_VINCULADOS") continue;
            List<string> fragmentos = DividirSiMuyLargo(kvp.Value);

            for (int i = 0; i < fragmentos.Count; i++)
            {
                Page nuevaPagina = new Page();
                nuevaPagina.titulo = fragmentos.Count > 1
                    ? kvp.Key + " (Parte " + (i + 1) + ")"
                    : kvp.Key;
                nuevaPagina.contenido = fragmentos[i];
                paginas.Add(nuevaPagina);
            }
        }
    }

    public void MostrarSeccionActual()
    {
        if (paginas == null || paginas.Count == 0)
            return;

        // --- Página izquierda (siempre existe si llegamos aquí) ---
        textoSalida.gameObject.SetActive(true);
        textoSalida.text = FormatearPagina(paginas[indiceActual]);

        // --- Página derecha (puede no existir si es la última página impar) ---
        bool hayPaginaDerecha = (indiceActual + 1) < paginas.Count;

        textoSalida2.gameObject.SetActive(hayPaginaDerecha);

        if (hayPaginaDerecha)
        {
            textoSalida2.text = FormatearPagina(paginas[indiceActual + 1]);
        }
        else
        {
            textoSalida2.text = string.Empty;
        }
    }

    string FormatearPagina(Page pagina)
    {
        return "| " + pagina.titulo + " |\n\n" + pagina.contenido;
    }

    // Avanzamos de 2 en 2, con wrap-around al inicio
    public void Siguiente()
    {
        if (paginas == null) return;

        indiceActual += 2;

        if (indiceActual >= paginas.Count)
            indiceActual = 0;

        MostrarSeccionActual();
    }

    // Retrocedemos de 2 en 2, con wrap-around al final
    public void Anterior()
    {
        if (paginas == null) return;

        indiceActual -= 2;

        if (indiceActual < 0)
        {
            // Si el total es impar, el último "spread" empieza en Count-1
            // Si es par, empieza en Count-2
            indiceActual = (paginas.Count % 2 == 0)
                ? paginas.Count - 2
                : paginas.Count - 1;
        }

        MostrarSeccionActual();
    }

    List<string> DividirSiMuyLargo(string texto)
    {
        List<string> partes = new List<string>();

        for (int i = 0; i < texto.Length; i += maxCaracteresPorPagina)
        {
            int length = Mathf.Min(maxCaracteresPorPagina, texto.Length - i);
            partes.Add(texto.Substring(i, length));
        }

        return partes;
    }

    private Dictionary<string, string> DividirEnSecciones(string texto)
    {
        Dictionary<string, string> resultado = new Dictionary<string, string>();
        MatchCollection matches = Regex.Matches(texto, @"###_(.*?)_###");

        for (int i = 0; i < matches.Count; i++)
        {
            string titulo = matches[i].Groups[1].Value.Trim();
            int inicio = matches[i].Index + matches[i].Length;
            int fin = (i + 1 < matches.Count) ? matches[i + 1].Index : texto.Length;
            resultado[titulo] = texto.Substring(inicio, fin - inicio).Trim();
        }

        return resultado;
    }

    public void setWriteInInput(bool mode) => WriteInInput = mode;
    public bool getWriteInInput() => WriteInInput;



    public void LimpiarTodo()
    {
        // Limpiar textos UI
        textoSalida.text = string.Empty;
        textoSalida2.text = string.Empty;

        // Limpiar datos internos
        paginas?.Clear();
        paginas = null;

        seccionesOriginales?.Clear();
        seccionesOriginales = null;

        indiceActual = 0;
    }
}