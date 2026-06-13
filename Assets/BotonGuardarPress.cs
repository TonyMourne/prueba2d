using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class BotonGuardarPress : MonoBehaviour
{
    public TMP_InputField textoInput;
    public TMP_InputField textoInput2;
    public LibraryManager GPE;
    public textmanagement tm;
    private string apiURL;

    [System.Serializable]
    public class CampoValor
    {
        public string campo;
        public string valor;

        public CampoValor(string campo, string valor)
        {
            this.campo = campo;
            this.valor = valor;
        }
    }

    // 🔥 IMPORTANTE: estos son los nombres EXACTOS que usa la DB para el mapeo
    private Dictionary<string, string> mapeoCampos = new Dictionary<string, string>()
    {
        // Personajes
        {"NOMBRE_Y_APELLIDOS", "NOMBRE_Y_APELLIDOS"},
        {"MOTES", "MOTES"},
        {"EDAD", "EDAD"},
        {"RAZA", "RAZA"},
        {"DESCRIPCIÓN_FÍSICA_DETALLADA", "DESCRIPCION_FISICA_DETALLADA"},
        {"DESCRIPCIÓN_PSICOLÓGICA_DETALLADA", "DESCRIPCION_PSICOLOGICA_DETALLADA"},
        {"MORAL", "MORAL"},
        {"CREENCIAS", "CREENCIAS"},
        {"HISTORIA_PERSONAL", "HISTORIA_PERSONAL"},
        {"NIVEL", "NIVEL"},
        {"HABILIDADES", "HABILIDADES"},
        {"EQUIPAMIENTO_Y_PERTENENCIAS", "EQUIPAMIENTO_Y_PERTENENCIAS"},

        // Lugares
        {"Nombre", "Nombre"},
        {"Historia", "Historia"},
        {"Extension", "Extension"},
        {"Poblacion", "Poblacion"},
        {"Razas", "Razas"},
        {"Lugares emblemáticos", "LugaresEmblematicos"},
        {"Ubicación Geográfica", "UbicacionGeografica"},
        {"Zonas", "Zonas"},
        {"Idioma(s)", "Idiomas"},
        {"Costumbres y tradiciones", "Costumbres"},
        {"Religión(es) y Crencias", "Religiones"},
        {"Fauna y flora", "FaunaFlora"},
        {"Gastronomía", "Gastronomia"},
        {"Arte y Arquitectura", "ArteArquitectura"},
        {"Recursos naturales", "RecursosNaturales"},
        {"Actividades Económicas", "ActividadesEconomicas"},
        {"Leyes principales", "Leyes"},
        {"Gobierno(s)", "Gobiernos"},
        {"Zonas de interes", "ZonasInteres"},
        {"Clima", "Clima"},
        {"Actividades de la población", "ActividadesPoblacion"},
        {"Posibles sucesos en el futuro en ese lugar", "SucesosFuturos"},
        {"Nivel educativo", "NivelEducativo"},
        {"Nivel tecnológico y mágico", "NivelTecnologicoMagico"},
        {"Problemas internos y rebeliones", "ProblemasRebeliones"},

        // Campañas
        {"TITULO_CAMPANIA", "TITULO_CAMPANIA"},
        {"NIVEL_RECOMENDADO", "NIVEL_RECOMENDADO"},
        {"NUMERO_SESIONES", "NUMERO_SESIONES"},
        {"TONO_NARRATIVO", "TONO_NARRATIVO"},
        {"TIPO_DE_CAMPANIA", "TIPO_DE_CAMPANIA"},
        {"GANCHO_INICIAL", "GANCHO_INICIAL"},
        {"CONFLICTO_CENTRAL", "CONFLICTO_CENTRAL"}
    };

    public void OnGuardarPressed()
    {
        if (!tm.getWriteInInput())
        {
            Debug.Log("No podemos guardar: no se ha cargado nada editable.");
            return;
        }

        RellenarCampos_URL_ID();

        GuardarInputField(textoInput);
        GuardarInputField(textoInput2);
    }

    void GuardarInputField(TMP_InputField input)
    {
        if (input == null || !input.gameObject.activeSelf)
            return;

        string texto = input.text;
        string campoExtraido = ExtraerCampo(texto);
        string valor = ExtraerValor(texto);

        if (string.IsNullOrEmpty(campoExtraido))
            return;

        Debug.Log("Campo RAW: [" + campoExtraido + "]");

        // 🔥 Limpiamos el campo (quitamos Parte X)
        string campoLimpio = LimpiarCampo(campoExtraido);

        Debug.Log("Campo LIMPIO: [" + campoLimpio + "]");

        // 🔥 Buscamos coincidencia flexible pero devolvemos valor exacto DB
        string campoFinal = BuscarCampoEnDiccionario(campoLimpio);

        if (string.IsNullOrEmpty(campoFinal))
        {
            Debug.LogWarning("Campo no mapeado: [" + campoExtraido + "] → limpio: [" + campoLimpio + "]");
            return;
        }

        Debug.Log("Preparando actualización: campo = '" + campoFinal + "', valor = '" + valor + "'");
        StartCoroutine(ActualizarCampo(campoFinal, valor));
    }

    // 🔥 Quita "(Parte X)"
    string LimpiarCampo(string campo)
    {
        campo = campo.Trim();

        int parentesis = campo.IndexOf("(");
        if (parentesis > 0)
            campo = campo.Substring(0, parentesis);

        return campo.Trim();
    }

    // 🔥 Busca ignorando mayúsculas pero devuelve el valor correcto
    string BuscarCampoEnDiccionario(string campo)
    {
        foreach (var kvp in mapeoCampos)
        {
            if (string.Equals(kvp.Key.Trim(), campo.Trim(), System.StringComparison.OrdinalIgnoreCase))
            {
                return kvp.Value;
            }
        }

        return null;
    }

    public void RellenarCampos_URL_ID()
    {
        var Tipo_E_Id = GPE.ObtenerIdSeleccionado();

        if (Tipo_E_Id.id == null || Tipo_E_Id.tipo == null)
        {
            Debug.LogError("No se pudo obtener id/tipo del elemento a editar");
            return;
        }

        string baseURL = "http://localhost:8080/api/";

        switch (Tipo_E_Id.tipo)
        {
            case "P": apiURL = baseURL + "personajes/" + Tipo_E_Id.id; break;
            case "L": apiURL = baseURL + "lugar/" + Tipo_E_Id.id; break;
            case "C": apiURL = baseURL + "campania/" + Tipo_E_Id.id; break;
            default:
                Debug.LogError("Tipo desconocido: " + Tipo_E_Id.tipo);
                break;
        }
    }

    string ExtraerCampo(string texto)
    {
        int inicio = texto.IndexOf("|") + 1;
        int fin = texto.IndexOf("|", inicio);

        if (inicio < 1 || fin < 0 || fin <= inicio)
        {
            Debug.LogWarning("No se encontró un campo válido en el texto");
            return null;
        }

        return texto.Substring(inicio, fin - inicio).Trim();
    }

    string ExtraerValor(string texto)
    {
        int finTitulo = texto.LastIndexOf("|") + 1;
        return texto.Substring(finTitulo).Trim();
    }

    IEnumerator ActualizarCampo(string campo, string valor)
    {
        CampoValor cv = new CampoValor(campo, valor);
        string json = JsonUtility.ToJson(cv);

        Debug.Log("JSON que se va a enviar: " + json);

        UnityWebRequest request = new UnityWebRequest(apiURL, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Campo actualizado correctamente: " + campo);
            GPE.reload();
        }
        else
        {
            Debug.LogError("Error al guardar: " + request.error);
        }
    }
}