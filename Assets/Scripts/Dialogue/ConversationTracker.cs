using System;
using Dialogue.LineLogicScripts;
using Dialogue.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;


namespace Dialogue
{
    public class ConversationTracker : MonoBehaviour
    {
        private enum EConversation
        {
            CitizenF,
            CitizenE,
            Homeless,
            Cop
        }

        [Header("Conversation")] [SerializeField]
        private EConversation currentConversation;

        [Header("Dialogue")] [SerializeField] private DialogueList[] activeDialogueLists;
        [SerializeField] private DialogueList[] passiveDialogueLists;
        [SerializeField] private NpcDialogue npcLines;
        [SerializeField] private DialogueOption dialoguePrefab;

        // object in top screen holding text
        [Header("User Interface")] [SerializeField]
        private GameObject topPanel;

        [SerializeField] private TextMeshProUGUI currentTxt;

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
        private int _activePlayer = 1;

        private CameraMove _cameraMove;

        private void Start()
        {
            _cameraMove = Camera.main.GetComponent<CameraMove>();
        }

        public void StartConversation()
        {
            StartInfoSequence();
        }

        private void EndConversation()
        {
            Destroy(gameObject);
            // next scene functionality here 
        }

        private void StartEndSequence(string endText)
        {
            ChangeTopText(endText);
            player1Panel.SetActive(false);
            player2Panel.SetActive(false);
            nextButton.SetActive(true);
            nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
            nextButton.GetComponent<Button>().onClick.AddListener(EndConversation);
            ChangeScene();
        }

        private void ChangeScene() // changes scene
        {
            _cameraMove.NextScene();
        }

        private void StartInfoSequence()
        {
            GoToNewInfoLine(infoSequence[_sequenceTracker]);
        }

        public void NextInfo()
        {
            if (_sequenceTracker <
                infoSequence.Length) // if the info dialogue is done, change the state to player action
                GoToNewInfoLine(infoSequence[_sequenceTracker]);
            else
                InfoOver();
        }

        public void ChangeTopText(string newText)
        {
            topPanel.SetActive(true);
            currentTxt.text = newText;
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

        private void SwapActivePlayer()
        {
            _activePlayer = _activePlayer switch
            {
                1 => 2,
                2 => 1,
                _ => 1
            };
            Debug.Log("Swapped active player to " + _activePlayer);
        }

        private void GoToNewIndex(string npcLine, int activeDialogueIndex, int passiveDialogueIndex)
        {
            ChangeTopText(npcLine);
            DestroyOldDialogueOptions(1);
            DestroyOldDialogueOptions(2);

            switch (_activePlayer)
            {
                case 1:
                    Debug.Log("Player 1 is active");
                    if (activeDialogueIndex != -1) CreateDialogueOptions(1, activeDialogueLists[activeDialogueIndex]);
                    if (passiveDialogueIndex != -1)
                        CreateDialogueOptions(2, passiveDialogueLists[passiveDialogueIndex]);
                    break;

                case 2:
                    Debug.Log("Player 2 is active");
                    if (passiveDialogueIndex != -1)
                        CreateDialogueOptions(1, passiveDialogueLists[passiveDialogueIndex]);
                    if (activeDialogueIndex != -1) CreateDialogueOptions(2, activeDialogueLists[activeDialogueIndex]);
                    break;

                default:
                    Debug.Log("Error: active player is not 1 or 2");
                    break;
            }
        }

        private GameObject GetPlayerPanel(int player)
        {
            var playerPanel = player switch
            {
                1 => player1Panel,
                2 => player2Panel,
                _ => throw new NullReferenceException()
            };
            playerPanel.SetActive(true);
            return playerPanel;
        }

        private void DestroyOldDialogueOptions(int player)
        {
            var playerPanel = GetPlayerPanel(player);
            var existingButtons = playerPanel.GetComponentsInChildren<Button>();
            foreach (var button in existingButtons) Destroy(button.gameObject);
        }

        private void CreateDialogueOptions(int player, DialogueList dialogueList) 
        {
            DestroyOldDialogueOptions(player);
            var playerPanel = GetPlayerPanel(player);

            foreach (var dialogueLine in dialogueList.dialogueLines)
            {
                var dialogueOption = Instantiate(dialoguePrefab, playerPanel.transform);
                // her skal vi også tjekke de andre funktionaliteter knapperne har
                dialogueOption.Setup(player, dialogueLine.targetIndex, dialogueLine.lineText, dialogueLine.isComment);
            }
        }

        public void OnPlayerChoice(int player, int caseIndex, string btnText)
        {
            ChangeTopText(btnText);

            switch (currentConversation)
            {
                case EConversation.Homeless:
                    HomelessConversation(player, caseIndex, btnText);
                    break;

                case EConversation.CitizenF:
                    CitizenFConversation(player, caseIndex, btnText);
                    break;

                case EConversation.CitizenE:
                    CitizenEConversation(player, caseIndex, btnText);
                    break;

                case EConversation.Cop:
                    CopConversation(player, caseIndex, btnText);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        #region Conversation Logic

        private void HomelessConversation(int player, int caseIndex, string btnText)
        {
            switch (caseIndex)
            {
                // Hi, did you see the person who just left store?
                case 0:
                    Debug.Log("Player " + player + " chose " + btnText);
                    GoToNewIndex(npcLines.lines[1], 1, -1);
                    break;

                // Hi, did you see someone run by just now?
                case 1:
                    Debug.Log("Player " + player + " chose " + btnText);
                    SwapActivePlayer();
                    GoToNewIndex(npcLines.lines[1], 1, -1);
                    break;

                // OPTIONAL: Should we investigate the mud-puddle?
                case 2:
                    Debug.Log("Player " + player + " chose option 2");
                    SwapActivePlayer();
                    GoToNewIndex(btnText, 2, -1);
                    break;

                // Yes I think we should! There might be clues if we search a bit.
                case 3:
                    Debug.Log("Case 3");
                    ConversationStorage.Instance.AddInformation("Boots", 1);
                    ConversationStorage.Instance.AddInformation("Boots", 2);
                    StartEndSequence("Reward: The players may notice footprints in the mudpuddle." +
                                     "This will gain them knowledge about the criminals footwear which will" +
                                     " unlock future conversation possibilities(boots related options).");
                    break;

                // No, we have to hurry up if we want to catch the thief.
                // No! Come on, the thief will get away!
                case 4:
                    Debug.Log("Case 4");
                    
                    break;
            }
        }

        private void CitizenFConversation(int player, int caseIndex, string btnText)
        {
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

        private void CitizenEConversation(int player, int caseIndex, string btnText)
        {
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

        private void CopConversation(int player, int caseIndex, string btnText)
        {
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

        #endregion
    }
}