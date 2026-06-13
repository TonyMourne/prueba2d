using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AddElementoAlPrompt : MonoBehaviour
{
    public LibraryManager gpe;
    public OnPressBehaviourCampaign opbc;
    public DropdownColorizer colorizer;
    public void onButtonPressedAddElementoAlPrompt()
    {
        Dictionary<string, string> elemento = gpe.GetElementoRecuperado();
        if (elemento == null || elemento.Count == 0) return;

        var (id, tipo) = gpe.ObtenerIdSeleccionado(); 

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("--- ELEMENTO ---");
        foreach (var kvp in elemento)
            sb.AppendLine($"{kvp.Key}: {kvp.Value}");
        sb.AppendLine("--- FIN ELEMENTO ---\n");

        opbc.addtopromptButton(sb.ToString(), id, tipo);
        colorizer.MarcarId(id);
    }

    public void onButtonPressedResetElementos()
    {
        opbc.ResetElementosPrompt();
        colorizer.LimpiarMarcados(); 
    }
}