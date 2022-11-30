using UnityEngine;

namespace Dialogue_Scripts
{
    [CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue/Create Dialogue List")]
    public class DialogueList: ScriptableObject
    {
        public DialogueLine[] dialogueLines;
    }
}