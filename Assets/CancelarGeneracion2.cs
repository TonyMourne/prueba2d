using UnityEngine;

public class CancelarGeneracion2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }
    public OnPressBehaviour OPB;
    public OnPressBehaviourPlace OPBL;
    public OnPressBehaviourCampaign OPBC;

    public void pressButtonCancelarGeneracion()
    {
        OPB.setCancelar();
        OPBL.setCancelar();
        OPBC.setCancelar();
    }

}
