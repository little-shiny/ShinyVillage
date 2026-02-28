using UnityEngine;

//Clase separada de Collectable para todo lo referente a los datos del item en sí.  

[RequireComponent(typeof(Rigidbody2D))]
public class Item : MonoBehaviour
{
    public ItemData data;
    [HideInInspector] public Rigidbody2D rb2d;


    private void Awake()
    {   
        rb2d= GetComponent<Rigidbody2D>(); //Se asocia la propiedad del prefab con la variable
    }
}
