using UnityEngine;

namespace Dialogue.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue/Create Dialogue Line")]
    public class DialogueLine : ScriptableObject
    {
        public int targetIndex;
        public string lineText;
        public string informationToCheckFor;
        public string informationToAdd;
        public string contextToCheckFor;
        public string contextToAdd;
        public bool isInterrupt;
        public bool isComment;
    }
}
