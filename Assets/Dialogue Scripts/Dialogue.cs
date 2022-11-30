using UnityEngine;

namespace Dialogue_Scripts
{
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        [TextArea(3, 20)]
        public string[,] sentences;
    }
}
