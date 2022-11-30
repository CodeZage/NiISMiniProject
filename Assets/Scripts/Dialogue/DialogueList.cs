using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue/Create Dialogue List")]
    public class DialogueList: ScriptableObject
    {
        public DialogueLine[] dialogueLines;
    }
}