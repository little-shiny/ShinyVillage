using UnityEngine;

/*
* Clase que representa elmovimiento del sprite del jugador
* Propiedades: Speed (velocidad)
**/

public class Movement : MonoBehaviour
{
    public float speed;

    // Se necesita obtener información de la entrada del usuario y actualizarse constantemente

    private void Update()
    {   
        // Variable para almacenar el eje horizontal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Variable que almacena la posicion (coordenadas)
        Vector3 direction = new Vector3(horizontal,vertical);

        transform.position += direction * speed * Time.deltaTime;
        // Time.deltaTime es la diferencia de tiempo entre la última vez que se actualizó


    }
}
