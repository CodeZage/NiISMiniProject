using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;


namespace Dialogue_Scripts
{
    public class ConversationTracker : MonoBehaviour
    {
        [SerializeField] private DialogueList[] player1Dialogue;
        [SerializeField] private DialogueList[] player2Dialogue;

        // object in top screen holding text
        [SerializeField] private GameObject TopPanel;
        [SerializeField] private TextMeshProUGUI CurrentTxt;
    
        // button prefab
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private DialogueOption dialoguePrefab;
    
        // player panels
        [SerializeField] private GameObject player1panel;
        [SerializeField] private GameObject player2panel;

        // next story dialogue
        [SerializeField] private GameObject NextButton;
    
        // next action button
        [SerializeField] private GameObject InteractButton;

        // the first info sequence;
        [SerializeField] private string[] InfoSequence1;
    
        // how far are we in the info sequence
        int sequencetracker = 0;
    
        private int nextState; // 0 = info is next, 1 = player action is next, 2 = NPC is next
    
    
        // player 1 - choices 1
        [SerializeField] private string[] P1Choices1;
    
        // player 2 - choices 1
        [SerializeField] private string[] P2Choices1;

        // NPC dialogue
        [SerializeField] private string[] NPCDialogue;
        private int NPCSequenceTracker = 0;
        int activePlayer = 1;

        public void StartInfoSequence()
        {
            LoadNPCContent(InfoSequence1[sequencetracker], false);
        }

        public void NextInfo()
        {
            if (sequencetracker < InfoSequence1.Length-1) // if the info dialogue is done, change the state to player action
            {
                LoadNPCContent(InfoSequence1[sequencetracker], false);
            }
            else
            {
                InfoOver();
            }
        }

        public void NextNPCLine()
        {
            LoadNPCContent(NPCDialogue[NPCSequenceTracker], true);
        }
    
        public void ChangeTopText(string newText)
        {
            TopPanel.SetActive(true);
            CurrentTxt.text = newText;
        }

        public void ShowActivePlayer(int player)
        {
            if (player == 1)
            {
                var image = player1panel.GetComponent<Image>();
                image.color = Color.green;
            }
            if (player == 2)
            {
                var image = player1panel.GetComponent<Image>();
                image.color = Color.green;
            }
        }

        public void InfoOver()
        {
            NextButton.SetActive(false);
        
            if (InfoSequence1[InfoSequence1.Length-1] == "Players")
            {
                // LoadPlayerContent(player1panel, P1Choices1.Length, P1Choices1);
                // LoadPlayerContent(player2panel, P2Choices1.Length, P2Choices1);
            }
        
            if (InfoSequence1[InfoSequence1.Length-1] == "NPC")
            {
                LoadNPCContent(NPCDialogue[NPCSequenceTracker], true);
            }
        }
    
        void LoadNPCContent(string dialogue, bool NPC)
        {
            ChangeTopText(dialogue);
        
            if (!NPC) // if story we need next button, if npc we do not
            {
                sequencetracker += 1;
                NextButton.SetActive(true);
            }

            if (NPC) // if NPC said something, we need to enable the player options
            {
                NPCSequenceTracker += 1;
            
                // load first player options
                CreateDialogueOptions(1, player1panel, player1Dialogue[0]);
                CreateDialogueOptions(2, player2panel, player2Dialogue[0]);
            }
        }

        void CreateDialogueOptions(int player, GameObject playerPanel, DialogueList dialogueList)
        {
            playerPanel.SetActive(true);
            foreach (var dialogueLine in dialogueList.dialogueLines)
            {
                DialogueOption dialogueOption = Instantiate(dialoguePrefab, playerPanel.transform);
                dialogueOption.Setup(player, dialogueLine.targetIndex, dialogueLine.lineText, dialogueLine.isComment);
            }
        }

        /*
        void LoadPlayerContent(GameObject playerPanel, int responses, string[] responseText)
        {
            playerPanel.SetActive(true);
            for (int i = 0; i < responses; i++)
            {
                GameObject dialogueButton = Instantiate(buttonPrefab, playerPanel.transform);
                var text = dialogueButton.GetComponentInChildren<TextMeshProUGUI>().text = responseText[i];
            
                var btn = playerPanel.GetComponentInChildren<UnityEngine.UI.Button>();
                btn.onClick.AddListener((() => OnPlayerChose(1, text)));
            }
        }
        */

        public void OnPlayerChose(int player, int newIndex, string btnText) // what happens when button is pressed
        {
            ChangeTopText(btnText);

            switch (newIndex)
            {
                case 0:
                    Debug.Log("Player " + player + " chose option 0");
                    break;
                case 1:
                    Debug.Log("Player " + player + " chose option 1");
                    break;
                case 2:
                    Debug.Log("Player " + player + " chose option 2");
                    break;
            }
        }
    }
}
