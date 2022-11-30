using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


namespace Dialogue_Scripts
{
    public class ConversationTracker : MonoBehaviour
    {
        [SerializeField] private DialogueList[] activeDialogueLists;
        [SerializeField] private DialogueList[] passiveDialogueLists;
        [SerializeField] private NpcDialogue npcLines;

        // object in top screen holding text
        [SerializeField] private GameObject topPanel;
        [SerializeField] private TextMeshProUGUI currentTxt;

        [SerializeField] private DialogueOption dialoguePrefab;

        // player panels
        [SerializeField] private GameObject player1Panel;
        [SerializeField] private GameObject player2Panel;

        // next story dialogue
        [SerializeField] private GameObject nextButton;

        // the first info sequence;
        [SerializeField] private string[] infoSequence;

        // how far are we in the info sequence
        private int _sequenceTracker = 0;

        private int _nextState; // 0 = info is next, 1 = player action is next, 2 = NPC is next

        // NPC dialogue
        private int _npcSequenceTracker = 0;
        private int _activePlayer = 1;

        public void StartInfoSequence()
        {
            GoToNewInfoLine(infoSequence[_sequenceTracker]);
        }

        public void NextInfo()
        {
            if (_sequenceTracker <
                infoSequence.Length - 1) // if the info dialogue is done, change the state to player action
                GoToNewInfoLine(infoSequence[_sequenceTracker]);
            else
                InfoOver();
        }

        public void ChangeTopText(string newText)
        {
            topPanel.SetActive(true);
            currentTxt.text = newText;
        }

        public void ShowActivePlayer(int player)
        {
            if (player == 1)
            {
                var image = player1Panel.GetComponent<Image>();
                image.color = Color.green;
            }

            if (player == 2)
            {
                var image = player1Panel.GetComponent<Image>();
                image.color = Color.green;
            }
        }

        private void InfoOver()
        {
            nextButton.SetActive(false);
            GoToNewIndex(npcLines.lines[0], 0, 0);
        }

        private void GoToNewInfoLine(string npcLine)
        {
            ChangeTopText(npcLine);

            _sequenceTracker += 1;
            nextButton.SetActive(true);
        }

        private void GoToNewIndex(string npcLine, int activeDialogueIndex, int passiveDialogueIndex)
        {
            ChangeTopText(npcLine);

            _npcSequenceTracker += 1;

            switch (_activePlayer)
            {
                case 1:
                    CreateDialogueOptions(1, player1Panel, activeDialogueLists[activeDialogueIndex]);
                    CreateDialogueOptions(2, player2Panel, passiveDialogueLists[passiveDialogueIndex]);
                    break;
                case 2:
                    CreateDialogueOptions(1, player1Panel, activeDialogueLists[passiveDialogueIndex]);
                    CreateDialogueOptions(2, player2Panel, passiveDialogueLists[activeDialogueIndex]);
                    break;
                default:
                    Debug.Log("Error: active player is not 1 or 2");
                    break;
            }
        }

        private void CreateDialogueOptions(int player, GameObject playerPanel, DialogueList dialogueList)
        {
            playerPanel.SetActive(true);
            
            Button[] existingButtons = playerPanel.GetComponentsInChildren<Button>();
            foreach (var button in existingButtons)
            {
                Destroy(button.gameObject);
            }
            
            foreach (var dialogueLine in dialogueList.dialogueLines)
            {
                var dialogueOption = Instantiate(dialoguePrefab, playerPanel.transform);
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

        public void OnPlayerChose(int player, int caseIndex, string btnText) // what happens when button is pressed
        {
            ChangeTopText(btnText);

            switch (caseIndex)
            {
                case 0:
                    GoToNewIndex(npcLines.lines[3], 1, 1);
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