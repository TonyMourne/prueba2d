using UnityEngine;

public class MusicaFondo : MonoBehaviour
{
    public AudioClip[] playlist;
    private AudioSource audioSource;
    private int indiceActual = 0;

    void Awake()
    {
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // el loop lo gestionamos nosotros
    }

    void Start()
    {
        if (playlist.Length > 0)
            TocarSiguiente();
    }

    void Update()
    {
        // cuando termina una canción, pasa a la siguiente
        if (!audioSource.isPlaying)
            TocarSiguiente();
    }

    void TocarSiguiente()
    {
        // elegir aleatoria que no sea la misma que acaba de sonar
        int nuevo = indiceActual;
        if (playlist.Length > 1)
            while (nuevo == indiceActual)
                nuevo = Random.Range(0, playlist.Length);

        indiceActual = nuevo;
        audioSource.clip = playlist[indiceActual];
        audioSource.Play();
    }
}