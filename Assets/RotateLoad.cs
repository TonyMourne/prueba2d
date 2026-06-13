using UnityEngine;
using UnityEngine.UI;

public class RotateLoad : MonoBehaviour
{
    public Image load;
    public Button cancelar;
    public float speed = 200f;

    void Start()
    {
        StopLoading();
    }

    public void StopLoading()
    {
        gameObject.SetActive(false);
        cancelar.gameObject.SetActive(false);
    }

    public void StartLoading()
    {
        gameObject.SetActive(true);
        cancelar.gameObject.SetActive(true);
    }

    void Update()
    { 
        load.transform.Rotate(0, 0, speed * Time.deltaTime); 
    }
}