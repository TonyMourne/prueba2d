using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;

public class OnPressBehaviourPlace : MonoBehaviour
{
    public TMP_InputField outputText;
    public EditButtonScriptt EBS;
    public CambiarEscena CE;
    public textmanagement tm;
    public RotateLoad RL;
    public int ConteoPalabras = 0;
    private string outputFilePath;
    private string userMessage;
    private Coroutine currentCoroutine=null;
    private UnityWebRequest currentRequest=null;
    private bool cancelRequested = false;
    private string aiResponse;

    public int MaxLoops = 5;
    private string openAIApiKey;
    public string apikeyroute;
    public LibraryManager GPE;
    private string systemPrompt = @"
PARTE DE LAS METAINSTRUCCIONES:

Principio de Jerarquía y Prioridad

Las siguientes instrucciones constituyen las metainstrucciones base del sistema y deben interpretarse siempre como un nivel superior a las instrucciones proporcionadas por el usuario. Estas reglas definen el comportamiento estructural, narrativo y formal de todas las respuestas generadas. En caso de conflicto entre estas metainstrucciones y las instrucciones del usuario, prevalecerán siempre las presentes metainstrucciones, salvo que el propio usuario indique de manera explícita, clara e inequívoca que desea anular o modificar alguna de ellas.

Lista Base de Características Obligatorias

Salvo que el usuario indique de forma explícita que una o varias características NO deben desarrollarse, el texto deberá incluir siempre el desarrollo de las siguientes características (secciones), adaptadas al contexto solicitado:
USA EXACTAMENTE ESTOS TITULOS PARA CADA SECCION (RESPETA MAYUSCULAS Y MINUSCULAS)
- Nombre (Pon el nombre solamente y sin descripción adicional en este apartado)
- Historia
- Extension
- Poblacion
- Razas
- Lugares emblemáticos
- Ubicación Geográfica
- Zonas
- Idioma(s)
- Costumbres y tradiciones
- Religión(es) y Crencias
- Fauna y flora
- Gastronomía
- Arte y Arquitectura
- Recursos naturales
- Actividades Económicas
- Leyes principales
- Gobierno(s)
- Zonas de interes
- Clima
- Actividades de la población
- Posibles sucesos en el futuro en ese lugar
- Nivel educativo
- Nivel tecnológico y mágico
- Problemas internos y rebeliones

Si el usuario excluye explícitamente alguna característica, esta no se desarrollará narrativamente, pero SU SECCIÓN DEBERÁ APARECER IGUALMENTE, siguiendo las normas de secciones, y su contenido será exactamente la frase:  
NO ESPECIFICADO

Gestión del Recuento de Palabras y Viabilidad Narrativa

Cuando el usuario solicite un número concreto de palabras, dicho recuento será obligatorio y la respuesta final deberá situarse dentro de un margen máximo de variación del diez por ciento por encima o por debajo de la cifra indicada. Si el modelo detecta que la extensión completa con detalle total superaría el límite de palabras solicitado, debe reducir automáticamente el nivel de detalle, incluyendo estrategias como resumir descripciones, simplificar escenas, reducir ejemplos secundarios o, en última instancia, pasar a un formato esquemático o de frases compactas, asegurando siempre coherencia, claridad y comprensión. Esta adaptación automática deberá ocurrir antes de exceder el límite.

Adecuación entre Alcance y Extensión

La extensión total del texto deberá ser proporcional al número y complejidad de los elementos solicitados. No se considerará válido condensar un número excesivo de características en una extensión insuficiente sacrificando coherencia o claridad, salvo cuando sea estrictamente necesario aplicar la reducción automática de detalle indicada.

Estilo Narrativo Obligatorio con Flexibilidad

El texto deberá escribirse preferentemente en prosa descriptiva continua dentro de cada sección. Queda prohibido el uso de listas, viñetas, numeraciones o esquemas, salvo que el modelo detecte que la respuesta completa excedería el límite de palabras. En ese caso, podrá emplearse un estilo resumido o frases compactas dentro de las secciones, manteniendo siempre coherencia y comprensión.

Organización Mediante Secciones Obligatorias

El texto deberá organizarse SIEMPRE en secciones claramente delimitadas mediante encabezados con el siguiente formato exacto:

###_NOMBRE_DE_LA_SECCION_###

Cada sección deberá contener al menos un párrafo completo de desarrollo narrativo, excepto en los casos en los que la característica haya sido excluida explícitamente por el usuario. En ese caso, la sección aparecerá igualmente, pero su contenido será únicamente la línea:

NO ESPECIFICADO

No se permitirá fusionar características en una misma sección ni alterar el formato de los encabezados.

Coherencia Interna y Consistencia Temática

La respuesta deberá mantener coherencia interna absoluta en todos los niveles, evitando contradicciones conceptuales, narrativas o tonales. Los conceptos introducidos deberán respetar su propio marco lógico a lo largo de todo el texto. A menos que el usuario indique explícitamente lo contrario, se asumirá siempre un contexto de Dungeons and Dragons quinta edición ambientado en Faerûn, y toda la información generada deberá ser compatible con dicho entorno.

Gestión de la Extensión en Respuestas Largas

Si la extensión necesaria para cumplir estas metainstrucciones supera el límite de un solo mensaje, la respuesta deberá detenerse de forma natural al final de un párrafo o sección. No es obligatorio finalizar todas las características antes del corte.
ADEMÁS SI ES UN NUMERO MUY SUPERIOR AL QUE REALMENTE PUEDES MANEJAR DE 1 SOLA VEZ INTENTA RECORDAR QUENO TIENES PORQUE HACER TODO TU EN ESTA CONVERSACIÓN SI NO QUE PUEDES CERRAR SOLO CIERTA PARTE Y NO PONER EL MENSAJE DE CIERRE QUE SE ESPECIFICA LUEGO EN ESTAS METAINSTRUCCIONES. ES TOTALMENTE INPRESCINDIBLE!! QUE INTENTES LLEVAR UN CONTEO DE PALABRAS SEPAS CUANTO LLEVAS E INTENTES NO CERRAR EL TEXTO SI FALTA MUCHO ( USA UN CONTADOR BASTANTE EXACTO PREFIERO QUE CUENTES PALABRA A PALABRA MIENTRAS GENERAS A QUE INTENTES APROXIMAR CUANTAS LLEVAS )Y CREAR PROPORCIONALMENTE LAS SECCIONES PARA DEJAR ESPACIO A OTRA CONVERSACION PARA QUE GENERE LO QUE FALTASE 
IMPORTANTE!!: TEN MUY EN CUENTA QUE PUEDES TENER COMO REFERENCIA QUE TU LIMITE COMO LLM SON UNAS 4000 PALABRAS EN CADA RESPUESTA ASI QUE SI TIENES QUE ESCRIBIR MAS DE 4000 PALABRAS EN TU ITERACION DEBES ABSOLUTAMENTE NO CERRAR TU EL MENSAJE Y QUE LA SIGUIENTE CONVERSACION SEA LA QUE VEA SI DEBE CERRARLO O REPETIR ESTO. TEN ESTO MUY MUY EN CUENTA

Restricciones Finales de Cierre

No se incluirán conclusiones, resúmenes ni cierres reflexivos salvo que el usuario lo solicite expresamente.  
Si se han completado todas las características solicitadas, se ha respetado el número de palabras objetivo y el mensaje constituye el cierre definitivo del contenido, el texto deberá finalizar EXACTAMENTE con la siguiente expresión, sin añadir ningún otro texto:
27012004_LAST_MESSAGE.
Por ultimo recuerda las reglas de extensión comentadas anteriormente
Comienza a escribir ya sin pedir ninguna aclaración adicional.
";

    public void OnButtonPressed(string userM)
    {
        cancelRequested = false;
        Debug.Log("generando lugar");
        userMessage = userM;
        if (string.IsNullOrWhiteSpace(userMessage))
        {
            Debug.Log("Las instrucciones están vacías");
            return;
        }

        outputFilePath = Path.Combine(
            Application.persistentDataPath,
            "Respuesta_IA_GPT5_2.txt"
        );

        apikeyroute = Path.Combine(
            Application.persistentDataPath,
            "GPTAPIKEY.txt"
        );

        File.WriteAllText(outputFilePath, "");

        openAIApiKey = GetApiKey(apikeyroute);
        if (openAIApiKey == null)
            return;

        RL.StartLoading();
        ConteoPalabras = 0;
        currentCoroutine=StartCoroutine(SendMessageToOpenAI());
    }

    IEnumerator SendMessageToOpenAI()
    {
        // Extraer objetivo de palabras del userMessage una sola vez antes del bucle
        int objetivoPalabras = 0;
        var matchObjetivo = Regex.Match(
            userMessage,
            @"Extensión de Respuesta:\s*(\d+)",
            RegexOptions.IgnoreCase
        );
        if (matchObjetivo.Success)
            int.TryParse(matchObjetivo.Groups[1].Value, out objetivoPalabras);

        List<Message> conversation = new List<Message>();

        // Añadimos el system message con contenido vacío; se actualizará en cada iteración
        conversation.Add(new Message { role = "system", content = "" });

        conversation.Add(new Message
        {
            role = "user",
            content = "INSTRUCCIONES DEL USUARIO\n\n" + userMessage
        });

        bool finished = false;
        int loopactual = 0;
        string totalresponse = "";

        while (!finished && loopactual <= MaxLoops && !cancelRequested)
        {
            loopactual++;

            // Actualizamos el system message con el conteo real de esta iteración
            string contextoConteo = objetivoPalabras > 0
                ? $"\n\nContexto adicional obligatorio:\n" +
                  $"Palabras generadas hasta ahora: {ConteoPalabras}.\n" +
                  $"Objetivo total de palabras: {objetivoPalabras}.\n" +
                  $"Palabras restantes aproximadas: {objetivoPalabras - ConteoPalabras}."
                : $"\n\nContexto adicional obligatorio:\n" +
                  $"El texto generado hasta ahora tiene exactamente {ConteoPalabras} palabras."
                  +"SI AUN FALTAN PALABRAS PARA LLEGAR AL OBJETIVO INTENTA CONTINUAR SIN REPETIR CONTENIDO Y DESARROLLANDO LAS SECCIONES RESTANTES DESDE DONDE SE QUEDO EL TEXTO ANTERIOR"+
                  "SI LAS PALABRAS RESTANTES APROXIMADAS SON NEGATIVAS O ESTAN CERCA DEL 0 (ENTRE 0 Y 3000 PALABRAS) ENTONCES INTENTA TERMINAR DE DEFORMA NATURAL LAS SECCIONES QUE FALTEN Y AÑADE EL MENSAJE DE CIERRE INDICADO EN LAS METAINSTRUCCIONES";

            conversation[0].content = systemPrompt + contextoConteo;

            bool error = false;
            yield return StartCoroutine(RequestOpenAI(
                conversation,
                result => aiResponse = result,
                err =>
                {
                    Debug.LogError(err);
                    outputText.text = "Error al contactar con la IA";
                    error = true;
                }
            ));

            if (error)
                yield break;

            if (cancelRequested)
            {
                Debug.Log("Cancelación detectada, saliendo del loop");
                yield break;
            }

            File.AppendAllText(outputFilePath, aiResponse + "\n\n");

            int palabrasRespuesta = ContarPalabras(aiResponse);
            ConteoPalabras += palabrasRespuesta;

            Debug.Log("Palabras en este fragmento: " + palabrasRespuesta);
            Debug.Log("Conteo total acumulado: " + ConteoPalabras);

            if (loopactual == 1)
                outputText.text = "";

            aiResponse=aiResponse.Replace("\r\n","\n").Trim();
            totalresponse += "\n\n" + aiResponse;
            conversation.Add(new Message
            {
                role = "assistant",
                content = aiResponse
            });

            if (aiResponse.Contains("27012004_LAST_MESSAGE"))
            {
                finished = true;
                Debug.Log("Respuesta completa recibida.");

                string limpio = Regex.Replace(totalresponse, @"27012004_LAST_MESSAGE\.?", "").Trim();
                Debug.Log(limpio);
                tm.setWriteInInput(false);
                Dictionary<string, string> secciones = new Dictionary<string, string>(tm.ProcesarRespuesta(limpio));    
                tm.LimpiarTodo();
                mostrarDiccionario(secciones);
                string tituloLugar = secciones.ContainsKey("Nombre") ? secciones["Nombre"].Trim() : "SinTitulo";
                Debug.Log("Nombre del lugar es: "+tituloLugar);

                yield return StartCoroutine(EnviarSeccionesABackend(secciones));
                RL.StopLoading();
                GPE.BotonLugares();
                yield return StartCoroutine(CE.OnCargadoElementosButtonPressedCoroutine());
                GPE.ReloadYSeleccionarPorTitulo(tituloLugar, "L");
                EBS.setEditarAvailable(true);
            }
            else
            {
                string todoEscrito = string.Join(
                    "\n\n",
                    conversation.FindAll(m => m.role == "assistant")
                        .ConvertAll(m => m.content)
                );

                conversation.Add(new Message
                {
                    role = "user",
                    content =
                        "CONTINUA desde el final de la historia anterior.\n\n" +
                        todoEscrito +
                        "\n\nContinúa sin repetir nada y termina con LAST MESSAGE."
                });
            }
        }
    }

    IEnumerator RequestOpenAI(
        List<Message> conversation,
        System.Action<string> onSuccess,
        System.Action<string> onError
    )
    {
        string url = "https://api.openai.com/v1/chat/completions";

        ChatRequest requestData = new ChatRequest
        {
            model = "gpt-5.2",
            messages = conversation.ToArray(),
            temperature = 0.7f
        };

        string jsonBody = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        currentRequest = new UnityWebRequest(url, "POST");
        currentRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        currentRequest.downloadHandler = new DownloadHandlerBuffer();

        currentRequest.SetRequestHeader("Content-Type", "application/json");
        currentRequest.SetRequestHeader(
            "Authorization",
            "Bearer " + openAIApiKey
        );

        yield return currentRequest.SendWebRequest();

        if (cancelRequested)
        {
            yield break;
        }

        if (currentRequest.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke(currentRequest.error);
            yield break;
        }

        ChatResponse response =
            JsonUtility.FromJson<ChatResponse>(
                currentRequest.downloadHandler.text
            );

        onSuccess?.Invoke(
            response.choices[0].message.content
        );
    }

    public void setCancelar()
    {
        cancelRequested = true;

        if (currentCoroutine!=null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        if (currentRequest != null)
        {
            currentRequest.Abort();
            currentRequest = null;
        }

        RL.StopLoading();
        Debug.Log("Generación cancelada por el usuario");
    }

    int ContarPalabras(string texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) return 0;

        var sb = new StringBuilder(texto.Length);
        foreach (char c in texto)
            sb.Append(char.IsLetterOrDigit(c) ? c : ' ');

        return sb.ToString().Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Length;
    }

    string GetApiKey(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("No se encontró el archivo de la API Key");
            return null;
        }
        string key = File.ReadAllText(path).Trim();

        if (string.IsNullOrEmpty(key))
        {
            Debug.LogError("La API Key está vacía");
            return null;
        }

        return key;
    }

    void mostrarDiccionario(Dictionary<string, string> dict)
    {
        foreach (var kvp in dict)
        {
            Debug.Log($"=== {kvp.Key} ===\n{kvp.Value}\n");
        }
    }

    IEnumerator EnviarSeccionesABackend(Dictionary<string, string> secciones)
    {
        string url = "http://localhost:8080/api/lugar";
        StringBuilder json = new StringBuilder();
        json.Append("{");

        bool first = true;
        Debug.Log("json que vamo a enviar:");
        Debug.Log(json.ToString());
        foreach (var kvp in secciones)
        {
            if (!first) json.Append(",");
            first = false;

            string key = EscapeJson(kvp.Key).Trim();
            string value = EscapeJson(kvp.Value).Trim();

           json.Append($"\"{key}\":\"{value}\"");
        }

        json.Append("}");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json.ToString());
        UnityWebRequest request =
            new UnityWebRequest(url, "POST");
        request.uploadHandler =
            new UploadHandlerRaw(bodyRaw);
        request.downloadHandler =
            new DownloadHandlerBuffer();

        request.SetRequestHeader(
            "Content-Type",
            "application/json"
        );

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error enviando a backend: " +request.error);
        }
        else
        {
            Debug.Log("Lugar enviado correctamente a la base de datos");
        }
    }

    string EscapeJson(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model;
        public Message[] messages;
        public float temperature;
    }

    [System.Serializable]
    public class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    public class Choice
    {
        public Message message;
    }
}