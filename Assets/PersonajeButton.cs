using UnityEngine;

public class PersonajeButton : MonoBehaviour
{
    public GenerarOrquest GO;
    public void PersonajeButtonPressed()
    {
        GO.cambiarmodo(1);
    }

}
