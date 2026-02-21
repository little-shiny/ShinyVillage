using UnityEngine;

/*
* Clase que representa elmovimiento del sprite del jugador
* Propiedades: Speed (velocidad)
**/

public class Movement : MonoBehaviour
{
    public float speed;

    public Animator animator; // Representa la animación del sprite

    private Vector3 direction; 
    // se hace global la dirección para poder acceder desde FixedPlayer y se guarde fuera del método
    
    // Se necesita obtener información de la entrada del usuario y actualizarse constantemente

    private void Update()
    {   
        // Variable para almacenar el eje horizontal
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Variable que almacena la posicion (coordenadas)
        direction = new Vector3(horizontal,vertical);

        direction = direction.normalized;

        animateMovement(direction);

        // Ahora va en el FixedUpdate 
        // transform.position += direction * speed * Time.deltaTime;
        // Time.deltaTime es la diferencia de tiempo entre la última vez que se actualizó


    }

    // Se hace el cambio a FixedUpdate porque se genera un efecto "rebote" en el jugador porque se calcula antes el vector movimienmto que 
    // el collider. para ello se puede utilizar este método en su lugar. SOLO se mueve la transformación del movimiento
    private void FixedUpdate()
    {
        // Mover el player
        transform.position += direction * speed * Time.deltaTime;
    }

    void animateMovement(Vector3 direction)
    {
        if(animator != null)
        {
            if(direction.magnitude  > 0) // Siempre debería ser positivo porque es la longitud del vector, y 
            // Si el modulo es 0 significa que no hay movimiento del sprite
            {
                // como es positivo, nos movemos
                animator.SetBool("isMoving", true); // le decimos que hay movimiento a la animación

                // Le decimos la dirección a la que se mueve     
                animator.SetFloat("horizontal", direction.x);
                animator.SetFloat("vertical", direction.y);
            
            } 
            else
            {  // Como es cero, no nos movemos
                animator.SetBool("isMoving", false);
            } 
        }
    }
}
