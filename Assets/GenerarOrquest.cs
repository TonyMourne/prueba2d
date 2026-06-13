using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GenerarOrquest : MonoBehaviour
{

    public List<TMP_Text> outputText = new List<TMP_Text>();
    public List<TMP_InputField> inputField = new List<TMP_InputField>();
    public List<Toggle> inputToggles = new List<Toggle>(); // Nueva lista de toggles
    public OnPressBehaviour opb;
    public OnPressBehaviourCampaign opbc;
    public OnPressBehaviourPlace opbp;
    private int modo=0; //modo 1 = personaje ; modo 2 = lugar ; modo 3 = campaña
    private string UserMessage;
    private string[] lugaresField =
    {
        "Nombre :",
        "Extension :",
        "Poblacion :",
        "Razas :",
        "Ubicación Geográfica :",
        "Idioma(s) :",
        "Religión(es) y Creencias:",
        "Recursos naturales :",
        "Actividades Económicas :",
        "Gobierno(s) :",
        "Clima :",
        "Nivel educativo :",
        "Nivel tecnológico/mágico:",
        "Extensión de Respuesta:"
    };
    private string[]personajesField = 
    {
        "Nombre y Apellidos :",
        "Motes :",
        "Edad :",
        "Raza :",
        "Descripción Física :",
        "Descripción Psicológica :",
        "Moral :",
        "Creencias :",
        "Historia Personal :",
        "Nivel :",
        "Habilidades :",
        "Equipamiento/Pertenencias:",
        "Extensión de Respuesta:"
    };

    private string[]campaniaField =
    {
      "TITULO CAMPAÑA :",
      "NIVEL RECOMENDADO :",
      "NUMERO DE SESIONES :",
      "TIPO DE CAMPAÑA :",
      "GANCHO INICIAL :",
      "CONFLICTO CENTRAL :",
      "Extensión de Respuesta:"  
    };

    public void Start()
    {
        cambiarmodo(1);
    }


    // este metodo es el que se debe conectar a los 3 botones de tipo 
    public void cambiarmodo(int modoN) 
    {
        //modo 1 = personaje ; modo 2 = lugar ; modo 3 = campaña

        modo=modoN;
        GenerarFields();
    }

private void GenerarFields()
{
    // Activamos todos primero
    foreach (TMP_InputField field in inputField)
    {
        field.gameObject.SetActive(true);
        field.text = ""; // opcional: limpiar texto
    }

    // Seleccionamos el array correcto según el modo
    string[] campos = null;

    switch (modo)
    {
        case 1:
            campos = personajesField;
            break;

        case 2:
            campos = lugaresField;
            break;

        case 3:
            campos = campaniaField;
            break;

        default:
            return;
    }

    // Asignamos textos y activamos/desactivamos lo necesario
    for (int i = 0; i < outputText.Count; i++)
    {
        if (i < campos.Length)
        {
            outputText[i].text = campos[i];

            if (i < inputField.Count)
            {
                inputField[i].gameObject.SetActive(true);
                inputToggles[i].gameObject.SetActive(true);
            }

        }
        else
        {
            outputText[i].text = "";
            if (i < inputField.Count)
            {
                inputField[i].gameObject.SetActive(false);
                inputToggles[i].gameObject.SetActive(false);
            }
        }
    }
}

    public void GenerarOnButtonPressed()
    {
        // añadir aqui funcion privada crear userMessage y pasarlo como variable
        // al OnButtonPressed()

       UserMessage = generarUserMessage();
       Debug.Log(UserMessage);
       Debug.Log("plantilla");
       // luego añadir aqui la funcion de añadir
        //modo 1 = personaje ; modo 2 = lugar ; modo 3 = campaña
        if (modo==1)
        {
            opb.OnButtonPressed(UserMessage);
            
        }
        else if (modo==2)
        {
            opbp.OnButtonPressed(UserMessage);
        }
        else if (modo==3)
        {
           opbc.OnButtonPressed(UserMessage); 
        }
        else
        {
            Debug.Log("Porfavor, presione uno de los botones de plantilla antes de generar");
        }
    }

    private string generarUserMessage()
    {
        string result = "";

        if (modo == 1)
        {
            result = "Crea un personaje con los siguientes detalles específicos:\n\n";

            for (int i = 0; i < personajesField.Length && i < inputField.Count; i++)
            {
                if (inputToggles[i].isOn)
                {
                    if (string.IsNullOrEmpty(inputField[i].text))
                    {
                        result += $"{personajesField[i]} GENERA ESTA SECCION A TU CRITERIO CREATIVO\n"; //deberia cambiar estos mensajes?
                    }
                    else
                    {
                        if (personajesField[i] == "Extensión de Respuesta:")
                        {
                            result += $"La respuesta debe tener aproximadamente {inputField[i].text} palabras. Debes asegurarte de acercarte lo máximo posible a esta cantidad y ajustar el nivel de detalle del contenido para intentar cumplir dicho objetivo de extensión.\n";
                        }
                        else
                        {
                            result += $"{personajesField[i]} {inputField[i].text}\n";
                        }
                    }
                    
                }
                else
                {
                    result += $"{personajesField[i]} NO ESPECIFIQUES ESTE CAMPO\n";
                }
            }
        }
        else if (modo == 2)
        {
            result = "Crea un lugar con los siguientes detalles específicos:\n\n";

            for (int i = 0; i < lugaresField.Length && i < inputField.Count; i++)
            {
                if (inputToggles[i].isOn)
                {
                    if (string.IsNullOrEmpty(inputField[i].text))
                    {
                        result += $"{lugaresField[i]} GENERA ESTA SECCION A TU CRITERIO CREATIVO\n"; //deberia cambiar estos mensajes?
                    }
                    else
                    {
                        if (lugaresField[i] == "Extensión de Respuesta:")
                        {
                            result += $"La respuesta debe tener aproximadamente {inputField[i].text} palabras. Debes asegurarte de acercarte lo máximo posible a esta cantidad y ajustar el nivel de detalle del contenido para intentar cumplir dicho objetivo de extensión.\n";
                        }
                        else
                        {
                            result += $"{lugaresField[i]} {inputField[i].text}\n";
                        }
                    }
                    
                }
                else
                {
                    result += $"{lugaresField[i]} NO ESPECIFIQUES ESTE CAMPO\n";
                }
            }
        }
        else if (modo == 3)
        {
            result = "Crea una campaña con los siguientes detalles específicos:\n\n";

            for (int i = 0; i < campaniaField.Length && i < inputField.Count; i++)
            {
                if (inputToggles[i].isOn)
                {
                    if (string.IsNullOrEmpty(inputField[i].text))
                    {
                        result += $"{campaniaField[i]} GENERA ESTA SECCION A TU CRITERIO CREATIVO\n"; //deberia cambiar estos mensajes?
                    }
                    else
                    {
                        if (campaniaField[i] == "Extensión de Respuesta:")
                        {
                            result += $"La respuesta debe tener aproximadamente {inputField[i].text} palabras. Debes asegurarte de acercarte lo máximo posible a esta cantidad y ajustar el nivel de detalle del contenido para intentar cumplir dicho objetivo de extensión.\n";
                        }
                        else
                        {
                            result += $"{campaniaField[i]} {inputField[i].text}\n";
                        }
                    }
                    
                }
                else
                {
                    result += $"{campaniaField[i]} NO ESPECIFIQUES ESTE CAMPO\n";
                }
            }
        }

        return result;
    }

}
