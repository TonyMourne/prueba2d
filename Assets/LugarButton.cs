using System;
using UnityEngine;

public class LugarButton : MonoBehaviour
{
    public GenerarOrquest GO;
    
    public void LugarButtonPressed()
    {
        GO.cambiarmodo(2);
    }

}
