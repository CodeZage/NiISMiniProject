using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class Dialogue
    {
        public string name;
        [TextArea(3, 20)]
        public string[,] sentences;
    }
}
