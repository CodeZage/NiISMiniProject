using UnityEngine;

namespace Dialogue_Scripts
{
    [CreateAssetMenu(fileName = "New Dialogue Line", menuName = "Dialogue/Create Dialogue Line")]
    public class DialogueLine : ScriptableObject
    {
        public string lineText;
        public int targetIndex;
        public bool isComment;
    }
}