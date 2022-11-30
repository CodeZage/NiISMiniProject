using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Options", menuName = "Player Dialogue Options")]
public class PlayerDialogueOptions : ScriptableObject
{
    public string[] dialogueOptions;
}
