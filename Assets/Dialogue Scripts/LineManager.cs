using UnityEngine;

namespace Dialogue_Scripts
{
    public class LineManager : MonoBehaviour
    {
        public bool activePlayer;
        public bool inactivePlayer;
        public bool bothPlayers;
        public bool
            LineType,
            Normal,
            Interrupt,
            InformationBased,
            ContextDependant,
            ContextEnable,
            Comment,
            Ending;

        public string Information;
        public string Context;
    }
}
