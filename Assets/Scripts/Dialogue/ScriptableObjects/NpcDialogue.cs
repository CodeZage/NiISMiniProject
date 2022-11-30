using UnityEngine;

namespace Dialogue.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New NPC Dialogue", menuName = "Dialogue/Create New NPC Dialogue")]
    public class NpcDialogue : ScriptableObject
    {
        public string[] lines;
    }
}
