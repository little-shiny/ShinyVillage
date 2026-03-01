using UnityEngine;
using TMPro;
//Script empleado para modificar el texto por defecto que aparece en el InputField de TextMeshPro para que sea descriptivo en función de la información que se necesita
public class InputFieldCustomText : MonoBehaviour
{
    public TMP_InputField gameNameField;
    public TMP_InputField playerNameField;

    private const string GAME_NAME_TEXT = "Nombre de la partida...";
    private const string PLAYER_NAME_TEXT = "Tu nombre...";

    void Update() 
    {
     gameNameField.interactable = true;
        playerNameField.interactable = true;

        gameNameField.text = GAME_NAME_TEXT;
        playerNameField.text = PLAYER_NAME_TEXT;   
    }

}
