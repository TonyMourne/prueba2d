using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class LibraryManager : MonoBehaviour
{
    public TMP_Dropdown dropdownResultados;
    public TMP_Dropdown dropdownCampo;
    public TMP_InputField inputFiltro;
    public textmanagement tm;
    public EditButtonScriptt EBS;
    private bool _campaniaLockeada = false;
    private HashSet<string> _idsPersonajesEnCampania = new HashSet<string>();
    private HashSet<string> _idsLugaresEnCampania = new HashSet<string>();
    public GameObject filtroPanel;
    public UnityEngine.UI.Button botonCandado;
    public Sprite spriteCandadoAbierto;
    public Sprite spriteCandadoCerrado;
    public DropdownColorizer colorizer;


    private List<Dictionary<string, object>> personajes = new List<Dictionary<string, object>>();
    private List<Dictionary<string, object>> lugares = new List<Dictionary<string, object>>();
    private List<Dictionary<string, object>> campanias = new List<Dictionary<string, object>>();

    private Dictionary<string, string> elementorecuperado;
    private List<(string id, string tipo)> ids = new List<(string id, string tipo)>();

    private string tipoActual = "";
    private bool datosCargados = false;
    private bool EstamosEnCarga = false;

    // NUEVA FUNCIÓN PARA EXTRAER EL ID REAL DE MONGO
    string ObtenerMongoId(Dictionary<string, object> item)
    {
        if (item["_id"] is JObject idObj)
        {
            if (idObj["$oid"] != null)
                return idObj["$oid"].ToString();

            if (idObj["timestamp"] != null)
                return idObj["timestamp"].ToString();
        }

        return item["_id"].ToString();
    }

    public void BotonPersonajes()
    {
        tipoActual = "P";
        CargarCamposDisponibles();
        filtroPanel.SetActive(true);
        inputFiltro.gameObject.SetActive(true);
        inputFiltro.text="";
        MostrarTodos(tipoActual);
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();
        botonCandado.gameObject.SetActive(false);
    }

    public void BotonLugares()
    {
        tipoActual = "L";
        CargarCamposDisponibles();
        filtroPanel.SetActive(true);
        inputFiltro.gameObject.SetActive(true);
        inputFiltro.text="";
        MostrarTodos(tipoActual);
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();
        botonCandado.gameObject.SetActive(false);
    }

    public void BotonCampanias()
    {
        tipoActual = "C";
        CargarCamposDisponibles();
        filtroPanel.SetActive(true);
        inputFiltro.gameObject.SetActive(true);
        inputFiltro.text="";
        MostrarTodos(tipoActual);
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();
        botonCandado.gameObject.SetActive(true);
    }
    public void ToggleLock()
    {
        if (_campaniaLockeada)
        {
            _campaniaLockeada = false;
            botonCandado.image.sprite = spriteCandadoAbierto;
            colorizer.ClearLockeado();
            Debug.Log("Filtro desbloqueado.");
        }
        else
        {
            if (tipoActual != "C" || _idsPersonajesEnCampania.Count == 0 && _idsLugaresEnCampania.Count == 0)
            {
                Debug.LogWarning("Selecciona una campaña con elementos antes de bloquear.");
                return;
            }
            _campaniaLockeada = true;
            botonCandado.image.sprite = spriteCandadoCerrado;
            colorizer.SetLockeado(ObtenerIdSeleccionado().id);
            Debug.Log("Filtro bloqueado a la campaña actual.");
        }
    }

public bool GetCampaniaLockeada() => _campaniaLockeada;

    void CargarCamposDisponibles()
    {
        dropdownCampo.ClearOptions();
        List<string> campos = new List<string>();

        if (tipoActual == "P")
        {
            campos.Add("NOMBRE_Y_APELLIDOS");
            campos.Add("MOTES");
            campos.Add("EDAD");
            campos.Add("RAZA");
            campos.Add("DESCRIPCION_FISICA_DETALLADA");
            campos.Add("DESCRIPCION_PSICOLOGICA_DETALLADA");
            campos.Add("MORAL");
            campos.Add("CREENCIAS");
            campos.Add("HISTORIA_PERSONAL");
            campos.Add("NIVEL");
            campos.Add("HABILIDADES");
            campos.Add("EQUIPAMIENTO_Y_PERTENENCIAS");
        }

        if (tipoActual == "L")
        {
            campos.Add("Nombre");
            campos.Add("Historia");
            campos.Add("Extension");
            campos.Add("Poblacion");
            campos.Add("Razas");
            campos.Add("Lugares emblemáticos");
            campos.Add("Ubicación Geográfica");
            campos.Add("Zonas");
            campos.Add("Idioma(s)");
            campos.Add("Costumbres y tradiciones");
            campos.Add("Religión(es) y Crencias");
            campos.Add("Fauna y flora");
            campos.Add("Gastronomía");
            campos.Add("Arte y Arquitectura");
            campos.Add("Recursos naturales");
            campos.Add("Actividades Económicas");
            campos.Add("Leyes principales");
            campos.Add("Gobierno(s)");
            campos.Add("Zonas de interes");
            campos.Add("Clima");
            campos.Add("Actividades de la población");
            campos.Add("Posibles sucesos en el futuro en ese lugar");
            campos.Add("Nivel educativo");
            campos.Add("Nivel tecnológico y mágico");
            campos.Add("Problemas internos y rebeliones");
        }

        if (tipoActual == "C")
        {
            campos.Add("TITULO_CAMPANIA");
            campos.Add("NIVEL_RECOMENDADO");
            campos.Add("NUMERO_SESIONES");
            campos.Add("TONO_NARRATIVO");
            campos.Add("TIPO_DE_CAMPANIA");
            campos.Add("GANCHO_INICIAL");
            campos.Add("CONFLICTO_CENTRAL");
        }

        dropdownCampo.AddOptions(campos);
    }
    void ParsearElementosVinculados(Dictionary<string, string> diccionario)
    {
        _idsPersonajesEnCampania.Clear();
        _idsLugaresEnCampania.Clear();

        if (!diccionario.ContainsKey("ELEMENTOS_VINCULADOS")) return;

        try
        {
            JArray array = JArray.Parse(diccionario["ELEMENTOS_VINCULADOS"]);
            foreach (JObject obj in array)
            {
                string id   = obj["id"]?.ToString();
                string tipo = obj["tipo"]?.ToString();
                if (tipo == "P") _idsPersonajesEnCampania.Add(id);
                if (tipo == "L") _idsLugaresEnCampania.Add(id);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Error parseando ELEMENTOS_VINCULADOS: " + e.Message);
        }
    }

    void FiltrarLocalmente(string campo, string valor)
    {
        dropdownResultados.ClearOptions();
        ids.Clear();

        List<string> opciones = new List<string>();
        List<Dictionary<string, object>> lista = null;

        if (tipoActual == "P") lista = personajes;
        if (tipoActual == "L") lista = lugares;
        if (tipoActual == "C") lista = campanias;

        foreach (var item in lista)
        {
            if (!item.ContainsKey(campo))
                continue;

            string texto = item[campo].ToString();

            if (texto.ToLower().Contains(valor.ToLower()))
            {
                string nombre = "Sin nombre";

                if (tipoActual == "P" && item.ContainsKey("NOMBRE_Y_APELLIDOS"))
                    nombre = item["NOMBRE_Y_APELLIDOS"].ToString();

                if (tipoActual == "L" && item.ContainsKey("Nombre"))
                    nombre = item["Nombre"].ToString();

                if (tipoActual == "C" && item.ContainsKey("TITULO_CAMPANIA"))
                    nombre = item["TITULO_CAMPANIA"].ToString();

                string id = ObtenerMongoId(item);
                if (_campaniaLockeada)
                {
                    if (tipoActual == "P" && !_idsPersonajesEnCampania.Contains(id)) continue;
                    if (tipoActual == "L" && !_idsLugaresEnCampania.Contains(id)) continue;
                    // Campañas no se filtran: puedes cambiar de campaña aunque esté el candado
                }

                opciones.Add($"({tipoActual}) {nombre}");
                ids.Add((id, tipoActual));
            }
        }

        dropdownResultados.AddOptions(opciones);
        // para poder volver a seleccionar el mismo
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();

    }

    public void PressedBotonCargar()
    {
        if (ids.Count == 0)
            return;

        int index = dropdownResultados.value;
        var seleccionado = ids[index];

        string url = "";

        if (seleccionado.tipo == "P")
            url = $"http://localhost:8080/api/personajes/{seleccionado.id}";

        if (seleccionado.tipo == "L")
            url = $"http://localhost:8080/api/lugar/{seleccionado.id}";

        if (seleccionado.tipo == "C")
            url = $"http://localhost:8080/api/campania/{seleccionado.id}";

        Debug.Log("URL del elemento completo es: " + url);

        StartCoroutine(CargarElementoCompleto(url));
    }

    IEnumerator CargarElementoCompleto(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Dictionary<string, string> diccionario = TransformarAdiccionario(json);

            if (tipoActual == "C")
                ParsearElementosVinculados(diccionario);

            tm.setWriteInInput(true);
            tm.CargarDesdeDiccionario(diccionario);
            elementorecuperado = diccionario;

            Debug.Log("Elemento completo cargado correctamente.");
            EBS.setEditarAvailable(true);
        }
        else
        {
            Debug.LogError("Error al cargar elemento completo.");
        }
    }

    Dictionary<string, string> TransformarAdiccionario(string json)
    {
        Dictionary<string, string> diccionario = new Dictionary<string, string>();
        JObject objeto = JObject.Parse(json);

        foreach (var propiedad in objeto)
        {
            if (propiedad.Key == "_id" || propiedad.Key == "_class")
                continue;

            if (propiedad.Value.Type == JTokenType.Object)
            {
                foreach (var sub in propiedad.Value.Children<JProperty>())
                {
                    diccionario[sub.Name] = sub.Value.ToString();
                    Debug.Log(sub.Value.ToString());
                }
            }
            else
            {
                diccionario[propiedad.Key] = propiedad.Value.ToString();
                Debug.Log(propiedad.Value.ToString());
            }
        }

        return diccionario;
    }

    public Dictionary<string, string> GetElementoRecuperado()
    {
        return elementorecuperado;
    }
    public List<(string id, string tipo)> GetIds()
    {
        return ids;
    }

    void OnFiltroCambiado(string nuevoTexto)
    {
        if (string.IsNullOrEmpty(tipoActual))
            return;

        if (dropdownCampo.options.Count == 0)
            return;

        string campo = dropdownCampo.options[dropdownCampo.value].text;

        if (string.IsNullOrEmpty(nuevoTexto))
        {
            MostrarTodos(tipoActual);
            return;
        }

        FiltrarLocalmente(campo, nuevoTexto);
    }

    void MostrarTodos(string tipo)
    {
        dropdownResultados.ClearOptions();
        ids.Clear();

        List<string> opciones = new List<string>();
        List<Dictionary<string, object>> lista = null;

        if (tipo == "P") lista = personajes;
        if (tipo == "L") lista = lugares;
        if (tipo == "C") lista = campanias;

        foreach (var item in lista)
        {
            string nombre = "Sin nombre";

            if (tipo == "P" && item.ContainsKey("NOMBRE_Y_APELLIDOS"))
                nombre = item["NOMBRE_Y_APELLIDOS"].ToString();

            if (tipo == "L" && item.ContainsKey("Nombre"))
                nombre = item["Nombre"].ToString();

            if (tipo == "C" && item.ContainsKey("TITULO_CAMPANIA"))
                nombre = item["TITULO_CAMPANIA"].ToString();

            string id = ObtenerMongoId(item);
            if (_campaniaLockeada)
            {
                if (tipo == "P" && !_idsPersonajesEnCampania.Contains(id)) continue;
                if (tipo == "L" && !_idsLugaresEnCampania.Contains(id)) continue;
                // Campañas no se filtran: puedes cambiar de campaña aunque esté el candado
            }

            opciones.Add($"({tipo}) {nombre}");
            ids.Add((id, tipo));
        }
        

        dropdownResultados.AddOptions(opciones);
        
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();
    }

    void Start()
    {
        if(EstamosEnCarga)
        {
            StartCoroutine(CargarDatosDesdeAPI());
        }
        
        inputFiltro.onValueChanged.AddListener(OnFiltroCambiado);

        dropdownResultados.onValueChanged.AddListener((index) =>
        {
            ResaltarCampoPrincipal(dropdownResultados, Color.beige);

            if (datosCargados && ids != null && ids.Count > index)
            {
                PressedBotonCargar();
            }
        });

        // Llamada inicial para que el color del primer elemento ya este resaltado
        ResaltarCampoPrincipal(dropdownResultados, Color.beige);
        

    }

    public IEnumerator StartLuegoDeCambiarEscena(bool autoCargarPrimero = true)
    {
        if (EstamosEnCarga)
        {
            yield return StartCoroutine(CargarDatosDesdeAPI());

            if (autoCargarPrimero && ids != null && ids.Count > 0)
            {
                PressedBotonCargar();
            }

            ResaltarCampoPrincipal(dropdownResultados, Color.beige);
        }
    }


    public void ResaltarCampoPrincipal(TMP_Dropdown dropdown, Color color)
    {
        TMP_Text label = dropdown.captionText;
        label.color = color;
    }

    // boton de reload 
    public void reload()
    {
        StartCoroutine(CargarDatosDesdeAPI());
    }

    string LimpiarTexto(string s)
    {
        // Quita espacios extra, saltos de línea y lo pone en minúsculas
        return Regex.Replace(s.Trim().ToLower(), @"\s+", "");
    }

    
    

    // esto es para reload de lo que se acaba de generar.
    public void ReloadYSeleccionarPorTitulo(string tituloBuscado, string tipo)
    {
        StartCoroutine(ReloadYSeleccionarCoroutine(tituloBuscado, tipo));
    }

    private IEnumerator ReloadYSeleccionarCoroutine(string tituloBuscado, string tipo)
    {
        yield return StartCoroutine(CargarDatosDesdeAPI());

        List<Dictionary<string, object>> lista = null;
        string campoTitulo = "";
        string campoPorDefecto = "";

        if (tipo == "P") { lista = personajes; campoTitulo = "NOMBRE_Y_APELLIDOS"; campoPorDefecto = "NOMBRE_Y_APELLIDOS"; }
        if (tipo == "L") { lista = lugares;    campoTitulo = "Nombre";             campoPorDefecto = "Nombre"; }
        if (tipo == "C") { lista = campanias;  campoTitulo = "TITULO_CAMPANIA";    campoPorDefecto = "TITULO_CAMPANIA"; }

        if (lista == null || lista.Count == 0)
        {
            Debug.LogWarning("No hay elementos para seleccionar.");
            yield break;
        }

        tipoActual = tipo;
        CargarCamposDisponibles();
        int campoIndex = dropdownCampo.options.FindIndex(opt => opt.text == campoPorDefecto);
        if (campoIndex >= 0)
        {
            dropdownCampo.value = campoIndex;
            dropdownCampo.RefreshShownValue();
        }

        var itemEncontrado = lista.Find(item =>
            item.ContainsKey(campoTitulo) &&
            item[campoTitulo].ToString().Trim().ToLower() == tituloBuscado.Trim().ToLower()
        );

        if (itemEncontrado == null)
        {
            Debug.LogWarning($"No se encontró {tipo} con título '{tituloBuscado}'");
            yield break;
        }

        string idBuscado = ObtenerMongoId(itemEncontrado);
        int dropdownIndex = ids.FindIndex(entry => entry.id == idBuscado);

        if (dropdownIndex >= 0)
        {
            dropdownResultados.value = dropdownIndex;
            dropdownResultados.RefreshShownValue();
            Debug.Log($"{tipo} seleccionado: {tituloBuscado}");
            PressedBotonCargar();
        }
        else
        {
            Debug.LogWarning($"Id '{idBuscado}' no encontrado en el dropdown.");
        }
    }
   

    IEnumerator CargarDatosDesdeAPI()
    {
        datosCargados=false;
        dropdownResultados.ClearOptions();
        ids.Clear();

        yield return StartCoroutine(CargarTodo("http://localhost:8080/api/personajes/all", "P"));
        yield return StartCoroutine(CargarTodo("http://localhost:8080/api/lugar/all", "L"));
        yield return StartCoroutine(CargarTodo("http://localhost:8080/api/campania/all", "C"));
        datosCargados=true;
        dropdownResultados.value = -1;
        dropdownResultados.RefreshShownValue();
        ResaltarCampoPrincipal(dropdownResultados, new Color(0.96f, 0.96f, 0.86f)); // beige
    }

    public (string id, string tipo) ObtenerIdSeleccionado()
    {
        int index = dropdownResultados.value;
        return ids[index];
    }

    IEnumerator CargarTodo(string url, string tipo)
    {
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var items = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(request.downloadHandler.text);

            foreach (var item in items)
            {
                string nombre = "Sin nombre";

                if (tipo == "P" && item.ContainsKey("NOMBRE_Y_APELLIDOS"))
                    nombre = item["NOMBRE_Y_APELLIDOS"].ToString();

                if (tipo == "L" && item.ContainsKey("Nombre"))
                    nombre = item["Nombre"].ToString();

                if (tipo == "C" && item.ContainsKey("TITULO_CAMPANIA"))
                    nombre = item["TITULO_CAMPANIA"].ToString();

                string id = ObtenerMongoId(item);

                dropdownResultados.options.Add(
                    new TMP_Dropdown.OptionData($"({tipo}) {nombre}")
                );

                ids.Add((id, tipo));
            }

            if (tipo == "P") personajes = items;
            if (tipo == "L") lugares = items;
            if (tipo == "C") campanias = items;

            dropdownResultados.RefreshShownValue();
        }
    }

    public void SetEstamosEnCarga(bool estado)
    {
        EstamosEnCarga=estado;
        dropdownResultados.interactable = estado;
        dropdownCampo.interactable = estado;
        inputFiltro.interactable = estado;
    }
    public bool GetEstamosEnCarga()
    {
        return EstamosEnCarga;
    }
}