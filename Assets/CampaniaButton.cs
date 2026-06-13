using UnityEngine;

public class CampaniaButton : MonoBehaviour
{
    public GenerarOrquest GO;
    public void CampaniaButtonPressed()
    {
        GO.cambiarmodo(3);
    }

}
