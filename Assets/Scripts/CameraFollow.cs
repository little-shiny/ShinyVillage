using UnityEngine;

// Clase que representa la cámara que ve el usuario del juego que persigue al jugador
public class CameraFollow : MonoBehaviour
{
    //Quweremos que se vea en el Ispector pero no sea accesible desde ningun otro sceript
    [SerializeField] private Transform target;

    Vector3 camOffset; // distancia que se mantiene desde el jugador

    void Start()
    {
        camOffset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        transform.position = target.position + camOffset;
    }
}
