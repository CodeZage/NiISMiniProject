using System;
using System.Linq;
using System.Collections;
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

        #region Serialized Private Fields

        [Header("Conversation")] [SerializeField]
        private EConversation currentConversation;

        [Header("Dialogue")] [SerializeField] private DialogueList[] activeDialogueLists;
        [SerializeField] private DialogueList[] passiveDialogueLists;
        [SerializeField] private NpcDialogue npcLines;
        [SerializeField] private DialogueOption dialoguePrefab;

        // object in top screen holding text
        [Header("User Interface")] [SerializeField]
        private GameObject topPanel;

        [SerializeField] private GameObject topPanelImage;

        [SerializeField] private TextMeshProUGUI currentTxt;

        // player panels
        [SerializeField] private GameObject player1Panel;
        [SerializeField] private GameObject player1Image;

        [SerializeField] private GameObject player2Panel;
        [SerializeField] private GameObject player2Image;

        // next story dialogue
        [SerializeField] private GameObject nextButton;

        // the first info sequence;
        [SerializeField] private string[] infoSequence;

        [SerializeField] private Sprite[] characterSprites;

        [SerializeField] private ConversationTracker nextConversation;

        #endregion

        #region Non-serialized Private Fields

        // how far are we in the info sequence
        private int _sequenceTracker = 0;

        private int _nextState; // 0 = info is next, 1 = player action is next, 2 = NPC is next

        // NPC dialogue
        private int _activePlayer = 1;

        private CameraMove _cameraMove;

        private bool _changeSceneHasHappened = false;

        #endregion

        #region Unity Event Functions

        private void Awake()
        {
            _cameraMove = GameObject.FindWithTag("MainCamera").GetComponent<CameraMove>();
        }

        #endregion

        #region Public Functions

        private void StartInfoSequence()
        {
            SwapImage(0);
            GoToNewInfoLine(infoSequence[_sequenceTracker]);
        }


        public void NextInfo()
        {
            if (_changeSceneHasHappened == false)
            {
                ChangeScene();
                _changeSceneHasHappened = true;
            }
            
            if (_sequenceTracker <
                infoSequence.Length) // if the info dialogue is done, change the state to player action
                GoToNewInfoLine(infoSequence[_sequenceTracker]);
            else
                InfoOver();
        }

        private void InfoOver()
        {
            nextButton.SetActive(false);
            BeginConversation(currentConversation);
        }

        private void EndConversation()
        {
            Destroy(gameObject);
        }

        private void StartEndSequence(string endText)
        {
            ChangeTopText(endText);
            player1Panel.SetActive(false);
            player1Image.SetActive(false);
            player2Panel.SetActive(false);
            player2Image.SetActive(false);
            nextButton.SetActive(true);
            nextButton.GetComponent<Button>().onClick.RemoveAllListeners();
            nextButton.GetComponent<Button>().onClick.AddListener(EndConversation);
        }


        public void ChangeTopText(string newText)
        {
            topPanel.SetActive(true);
            currentTxt.text = newText;
        }

        #endregion

        #region Private Functions

        private void OnEndConversation()
        {
            Destroy(gameObject);
            // next scene functionality here 
        }

        private void ChangeScene() // changes scene
        {
            _cameraMove.NextScene();
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

        private void SwapActivePlayerTriangle(int player)
        {
            if (player == _activePlayer)
            {
                return;
            }
            else
            {
                _activePlayer = _activePlayer switch
                {
                    1 => 2,
                    2 => 1,
                    _ => 1
                };
            }

            Debug.Log("Swapped active player to " + _activePlayer);
        }

        private void SwapImage(int imageIndex)
        {
            topPanelImage.GetComponentInChildren<Image>().sprite = characterSprites[imageIndex];
        }

        #endregion

        #region Core Conversation Logic

        private void BeginConversation(EConversation conversation)
        {
            switch (conversation)
            {
                case EConversation.Homeless:
                    HomelessConversation(1, 0, "");
                    break;
                case EConversation.CitizenF:
                    _changeSceneHasHappened = true;
                    CitizenFConversation(1, 0, "");
                    break;
                case EConversation.CitizenE:
                    _changeSceneHasHappened = true;
                    CitizenEConversation(1, 0, "");
                    break;
                case EConversation.Cop:
                    _changeSceneHasHappened = true;
                    CopConversation(1, 0, "");
                    break;
            }
        }

        public void OnPlayerChoice(int player, int caseIndex, string btnText)
        {
            ChangeTopText(btnText);
            SwapImage(player);

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

        private IEnumerator FinishConversationWithInfo(string npcLine, string playerLine, int player, int npc,
            float wait, float endWait)
        {
            if (player != -1) SwapImage(player);
            ChangeTopText(playerLine);

            DestroyOldDialogueOptions(1);
            DestroyOldDialogueOptions(2);

            yield return new WaitForSeconds(wait);

            if (npc != -1) SwapImage(npc);

            ChangeTopText(npcLine);

            yield return new WaitForSeconds(endWait);

            if (currentConversation != EConversation.Cop)
            { 
                nextConversation.gameObject.SetActive(true);
                nextConversation.enabled = true;
                nextConversation.BeginConversation(nextConversation.currentConversation);
                ChangeScene();
                Destroy(gameObject); 
            }
        }

        private IEnumerator GoToNewIndex(string npcLine, string playerLine, int player, int npc,
            int activeDialogueIndex, int passiveDialogueIndex, float wait)
        {
            if (player != -1) SwapImage(player);
            ChangeTopText(playerLine);

            DestroyOldDialogueOptions(1);
            DestroyOldDialogueOptions(2);

            yield return new WaitForSeconds(wait);

            if (npc != -1) SwapImage(npc);

            ChangeTopText(npcLine);

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
            var playerImage = player switch
            {
                1 => player1Image,
                2 => player2Image,
                _ => throw new NullReferenceException()
            };

            playerPanel.SetActive(true);
            playerImage.SetActive(true);
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
                if (!CheckForRequiredContextAndInformation(dialogueLine, player)) continue;

                var dialogueOptionButton = Instantiate(dialoguePrefab, playerPanel.transform);
                dialogueOptionButton.Setup(player, dialogueLine.targetIndex, dialogueLine.lineText,
                    dialogueLine.isComment, dialogueLine.isInterrupt);

                var strings = CheckForAddedContextAndInformation(dialogueLine);

                // Checks for and adds context and info to the player when the button is clicked.
                if (!string.IsNullOrEmpty(strings[0]))
                    dialogueOptionButton.GetComponent<Button>().onClick.AddListener(() =>
                        ConversationStorage.Instance.AddContext(strings[0], player));

                if (!string.IsNullOrEmpty(strings[1]))
                    dialogueOptionButton.GetComponent<Button>().onClick.AddListener(() =>
                        ConversationStorage.Instance.AddInformation(strings[1], player));
            }
        }

        private void HomelessConversation(int player, int caseIndex, string btnText)
        {
            switch (caseIndex)
            {
                case 0:
                    ChangeScene();
                    StartCoroutine(GoToNewIndex(npcLines.lines[0], npcLines.lines[0], -1, 3, 0, 0, 1));
                    break;

                // Hi, did you see the person who just left store?
                case 1:
                    Debug.Log("Player " + player + " chose " + btnText);
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, 1, -1, 2));
                    break;

                // Hi, did you see someone run by just now?
                case 2:
                    Debug.Log("Player " + player + " chose " + btnText);
                    SwapActivePlayer();
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, 1, -1, 2));
                    break;

                // Should we investigate the mud puddle?
                case 3:
                    SwapActivePlayer();
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, -1, 2, -1, 2));
                    break;

                // Yes we should.
                case 4:
                    StartCoroutine(FinishConversationWithInfo(
                        "As they start to inspect the mud-puddle, the two suspects discovers footprints from a pair of heavy boots. This information may be relevant in their further search for the criminal.",
                        btnText, player, 0, 4, 5));
                    ConversationStorage.Instance.AddInformation("Footprints", 1);
                    ConversationStorage.Instance.AddInformation("Footprints", 2);
                    break;

                // No, we have to hurry up if we want to catch the thief.
                case 5:
                    StartCoroutine(FinishConversationWithInfo(
                        "The suspects proceed to the shop further down the street...",
                        btnText, player, 0, 4, 3f));
                    break;
            }
        }

        private void CitizenEConversation(int player, int caseIndex, string btnText)
        {
            switch (caseIndex)
            {
                // Start of conversation: Hello there! How can I help you?
                case 0:
                    StartCoroutine(GoToNewIndex(npcLines.lines[0], npcLines.lines[0], -1, 3, 0, 0, 1));
                    break;

                // We are here to find out who stole the XY-72 drone. We have heard that you have seen the criminal. 
                // You can help US find out who stole the XY-72 drone. We heard that you have seen the criminal. 
                case 1:
                    StartCoroutine(GoToNewIndex(npcLines.lines[2], btnText, player, 3, 1, 4, 2));
                    break;

                // Don't play innocent, fool! Someone just ran through your store, did they not? (Bad Cop)
                case 2:
                    // No need to be rude
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, 5, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, -1, 2, 5));
                    StartCoroutine(GoToNewIndex(npcLines.lines[2], btnText, player, 3, 1, 4, 8));
                    break;

                // What did the person look like?
                case 3:
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 2, 6, 2));
                    break;

                // Did the person carry an item?
                case 4:
                    StartCoroutine(GoToNewIndex(npcLines.lines[5], btnText, player, 3, 3, 3, 2));
                    break;

                // Did the person wear boots?
                case 5:
                    StartCoroutine(GoToNewIndex(npcLines.lines[4], btnText, player, 3, 1, -1, 2));
                    break;

                // Were there anything distinct about the person's look?
                case 6:
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 1, 3, 2));
                    break;

                // BAD COP: This is not helping!! You better start talking! (Bad Cop)
                case 7:
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 1, -1, 2));
                    break;

                // Which direction did the person go?
                case 8:
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "After getting directions from the shop owner, the two suspects hurried in the given direction.",
                        btnText, player, 0, 4, 5));
                    break;

                // So it was a man?
                case 9:
                    SwapActivePlayer();
                    StartCoroutine(GoToNewIndex(npcLines.lines[7], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[7], btnText, player, 3, 2, -1, 2));
                    break;

                // Yeah man, no need to be rude!
                case 10:
                    StartCoroutine(GoToNewIndex(npcLines.lines[3], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 2, -1, 5));
                    break;

                case 13:
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "After getting directions from the shop owner, the two suspects hurried in the given direction.",
                        btnText, player, 0, 4, 5));
                    break;

                case 15:
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[2], btnText, player, 3, 1, 4, 5));

                    break;

                case 16:
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 1, 3, 2));
                    break;
            }
        }

        private void CitizenFConversation(int player, int caseIndex, string btnText)
        {
            switch (caseIndex)
            {
                // Conversation start
                case 0:
                    StartCoroutine(GoToNewIndex(npcLines.lines[0], npcLines.lines[0], -1, 3, 0, 0, 1));
                    break;

                // We are here to find the criminal + we are looking for a criminal
                case 1:
                    Debug.Log("Player " + player + " chose option 1");
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, 1, 1, 2));
                    SwapActivePlayer();
                    break;

                // You seem awfully nervous
                case 2:
                    Debug.Log("Player " + player + " chose option 2");
                    StartCoroutine(GoToNewIndex(npcLines.lines[3], btnText, player, 3, 3, 3, 4));
                    break;

                // That's just what a person who is hiding something would say! BAD COP!
                case 3:
                    StartCoroutine(GoToNewIndex(npcLines.lines[2], btnText, player, 3, 2, 2, 2));
                    // adds the Bad Cop context to the player who said the Bad Cop line :)
                    ConversationStorage.Instance.AddContext("BadCop", player);
                    SwapActivePlayerTriangle(player);
                    break;

                // Look, you are really starting to get on my nerves. BAD COP
                case 4:
                    // interrupt from other player
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, -1, -1, 15, 0));

                    // normal NPC dialogue
                    StartCoroutine(GoToNewIndex(npcLines.lines[13], btnText, player, 3, 13, 13, 4));
                    SwapActivePlayerTriangle(player);
                    break;

                // Whoa, whoa!
                case 5:
                    StartCoroutine(GoToNewIndex(npcLines.lines[10], btnText, player, 3, 10, -1, 2));
                    SwapActivePlayer();
                    break;

                // Good, so which direction did he run
                case 7:
                    StartCoroutine(GoToNewIndex(npcLines.lines[14], btnText, player, 3, 14, -1, 2));
                    SwapActivePlayerTriangle(player);
                    break;

                // Well, actually we are not sure about the gender + boots
                case 8:
                    StartCoroutine(GoToNewIndex(npcLines.lines[16], btnText, -1, 3, -1, -1, 2));
                    ConversationStorage.Instance.AddInformation("Boot", 1);
                    ConversationStorage.Instance.AddInformation("Boot", 2);
                    StartCoroutine(FinishConversationWithInfo(
                        "The two suspects hurry to the plaza in front of the SYN CORP head quarters.",
                        btnText, player, 0, 6, 3f));
                    break;

                // Well, actually gender - boots
                case 9:
                    StartCoroutine(GoToNewIndex(npcLines.lines[16], btnText, -1, 3, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "The two suspects hurry to the plaza in front of the SYN CORP head quarters.",
                        btnText, player, 0, 6, 3f));
                    break;

                // So you do know the person
                case 10:
                    StartCoroutine(GoToNewIndex(npcLines.lines[11], btnText, player, 3, 11, 11, 2));
                    break;

                // Where is she now
                case 11:
                    StartCoroutine(GoToNewIndex(npcLines.lines[15], btnText, -1, 3, -1, -1, 2));
                    SwapActivePlayerTriangle(player);
                    StartCoroutine(FinishConversationWithInfo(
                        "The two suspects hurry to the plaza in front of the SYN CORP head quarters.",
                        btnText, player, 0, 6, 3f));
                    break;

                // Used to be?
                case 12:
                    StartCoroutine(GoToNewIndex(npcLines.lines[12], btnText, player, 3, 12, 12, 2));
                    break;

                // Interrupt: You are clearly hiding something!
                case 13:
                    SwapActivePlayer();
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, -1, 18, -1, 2));
                    break;

                // Where were you
                case 14:
                    StartCoroutine(GoToNewIndex(npcLines.lines[4], btnText, player, 3, 4, 4, 2));
                    break;

                // Of course it runs by itself
                case 15:
                    StartCoroutine(GoToNewIndex(npcLines.lines[5], btnText, player, 3, 5, 5, 2));
                    SwapActivePlayer();
                    break;

                // Look, I'm sorry
                case 16:
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 6, 6, 2));
                    break;

                // Please?
                case 17:
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 8, -1, 2));
                    break;

                // You heard me
                case 18:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, -1, -1, 16, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, 9, -1, 4));
                    break;
                
                case 19:
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, 9, -1, 4));
                    break;
                
                // No, of course
                case 20:
                    StartCoroutine(GoToNewIndex(npcLines.lines[7], btnText, player, 3, 7, -1, 2));
                    SwapActivePlayer();
                    break;

                // Do you know
                case 21:
                    StartCoroutine(GoToNewIndex(npcLines.lines[11], btnText, player, 3, 11, 11, 2));
                    break;
            }
        }

        private void CopConversation(int player, int caseIndex, string btnText)
        {
            switch (caseIndex)
            {
                case 0:
                    StartCoroutine(GoToNewIndex(npcLines.lines[0], btnText, player, 3, 0, 0, 3));
                    break;
                case 1:
                    Debug.Log("Player " + player + " chose option 1");
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 8, 8, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 2:
                    Debug.Log("Player " + player + " chose option 2");
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 6, -1, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 3:
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 6, -1, 2));
                    SwapActivePlayer();
                    break;
                case 4:
                    StartCoroutine(GoToNewIndex(npcLines.lines[1], btnText, player, 3, 1, 1, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 5:
                    StartCoroutine(GoToNewIndex(npcLines.lines[3], btnText, player, 3, 3, 3, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 6:
                    StartCoroutine(GoToNewIndex(npcLines.lines[7], btnText, player, 3, 7, -1, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 7:
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, 9, 9, 2));
                    break;
                case 8:
                    StartCoroutine(GoToNewIndex(npcLines.lines[8], btnText, player, 3, 8, 8, 2));
                    break;
                case 9:
                    StartCoroutine(GoToNewIndex(npcLines.lines[9], btnText, player, 3, 9, 9, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 10:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, -1, 2));
                    SwapActivePlayer();
                    StartCoroutine(GoToNewIndex(npcLines.lines[10], btnText, player, 3, 10, -1, 4));
                    break;
                case 11:
                    StartCoroutine(GoToNewIndex(npcLines.lines[11], btnText, player, 3, 11, 11, 2));
                    break;
                case 12:
                    StartCoroutine(GoToNewIndex(npcLines.lines[13], btnText, player, 3, -1, -1, 2));
                    SwapActivePlayerTriangle(player);
                    StartCoroutine(GoToNewIndex(npcLines.lines[14], btnText, player, 3, 14, -1, 5));
                    break;
                case 13:
                    StartCoroutine(GoToNewIndex(npcLines.lines[2], btnText, player, 3, 2, 2, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 15:
                    // COP LINE: NPC = 4 for right image
                    ChangeScene();
                    StartCoroutine(GoToNewIndex(npcLines.lines[15], btnText, player, 4, 15, -1, 2));
                    break;
                case 16:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, 19, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[13], btnText, player, 3, -1, -1, 4));
                    break;
                case 17:
                    StartCoroutine(GoToNewIndex(npcLines.lines[16], btnText, player, 3, -1, 16, 2));
                    break;
                case 19:
                    StartCoroutine(GoToNewIndex(npcLines.lines[17], btnText, player, 3, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "And so the criminal was finally captured and justice was put forth. Another successful case closed by the SYN CORP cops. Thanks for playing!",
                        btnText, player, 0, 5, 10));
                    break;
                case 21:
                    ChangeScene();
                    // COP
                    StartCoroutine(GoToNewIndex(npcLines.lines[18], btnText, player, 4, -1, -1, 2));

                    // CRIMINAL
                    StartCoroutine(GoToNewIndex(npcLines.lines[12], btnText, player, 3, 12, 12, 5));
                    break;
                case 22:
                    ChangeScene();
                    // COP
                    StartCoroutine(GoToNewIndex(npcLines.lines[18], btnText, player, 4, -1, -1, 2));
                    SwapActivePlayer();
                    // CRIMINAL
                    StartCoroutine(GoToNewIndex(npcLines.lines[12], btnText, player, 3, 12, 12, 5));
                    break;

                case 23:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "And so the criminal was finally captured and justice was put forth. Another successful case closed by the SYN CORP cops. Thanks for playing!",
                        btnText, player, 0, 5, 3));
                    break;
                case 24:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "And so the criminal was finally captured and justice was put forth. Another successful case closed by the SYN CORP cops. Thanks for playing!",
                        btnText, player, 0, 5, 3));
                    break;
                case 25:
                    StartCoroutine(GoToNewIndex(npcLines.lines[17], btnText, player, 3, -1, -1, 2));
                    break;
                case 26:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, -1, 2));
                    StartCoroutine(FinishConversationWithInfo(
                        "And so the criminal was finally captured and justice was put forth. Another successful case closed by the SYN CORP cops. Thanks for playing!",
                        btnText, player, 0, 5, 3));
                    break;
                case 27:
                    StartCoroutine(GoToNewIndex(npcLines.lines[5], btnText, player, 3, 5, -1, 2));
                    SwapActivePlayerTriangle(player);
                    break;
                case 28:
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 6, -1, 2));
                    break;
                case 29:
                    StartCoroutine(GoToNewIndex(npcLines.lines[6], btnText, player, 3, 6, -1, 2));
                    SwapActivePlayer();
                    break;
                case 30:
                    StartCoroutine(GoToNewIndex(btnText, btnText, player, player, -1, -1, 2));
                    StartCoroutine(GoToNewIndex(npcLines.lines[4], btnText, player, 3, 4, 4, 5));
                    SwapActivePlayerTriangle(player);
                    break;
                case 33:
                    ChangeScene();
                    // COP
                    StartCoroutine(GoToNewIndex(npcLines.lines[18], btnText, player, 4, -1, -1, 2));
                    SwapActivePlayer();
                    // CRIMINAL
                    StartCoroutine(GoToNewIndex(npcLines.lines[12], btnText, player, 3, 12, 12, 5));
                    break;
            }
        }

        #endregion

        #region Static Check Functions

        private static bool CheckForRequiredContextAndInformation(DialogueLine dialogueLine, int player)
        {
            var bools = new bool[2];

            if (string.IsNullOrEmpty(dialogueLine.contextToCheckFor))
                bools[0] = true;
            else
                bools[0] = ConversationStorage.Instance.CheckContext(dialogueLine.contextToCheckFor, player);

            if (string.IsNullOrEmpty(dialogueLine.informationToCheckFor))
                bools[1] = true;
            else
                bools[1] = ConversationStorage.Instance.CheckInformation(dialogueLine.informationToCheckFor, player);

            // Returns true if both checks are true.
            return !bools.Contains(false);
        }

        private static string[] CheckForAddedContextAndInformation(DialogueLine dialogueLine)
        {
            var add = new string[2];

            if (string.IsNullOrEmpty(dialogueLine.contextToAdd))
                add[0] = null;
            else
                add[0] = dialogueLine.contextToAdd;

            if (string.IsNullOrEmpty(dialogueLine.informationToAdd))
                add[1] = null;
            else
                add[1] = dialogueLine.informationToAdd;

            return add;
        }

        /*private static void CheckDialogueOptionTags(DialogueLine dialogueLine, int player)
        {
            CheckInformation(dialogueLine, player);
            CheckContext(dialogueLine, player);
            CheckInterrupt(dialogueLine);
        }

        private static void CheckInformation(DialogueLine dialogueLine, int player)
        {
            if (string.IsNullOrEmpty(dialogueLine.informationToAdd))
            {
                var info = dialogueLine.AddComponent<Information>();
                info.SetInformationType(false); //true = getInformation, false = setInformation
                info.SetInformation(dialogueLine.informationToAdd);
                info.SetPlayer(player - 1); // 0 = p1, 1 = p2. Player is 1 or 2, so we reduce by one. 
            }

            if (string.IsNullOrEmpty(dialogueLine.informationToCheckFor))
            {
                var info = dialogueLine.AddComponent<Information>();
                info.SetInformationType(true); //true = getInformation, false = setInformation
                info.SetInformation(dialogueLine.informationToCheckFor);
                info.SetPlayer(player - 1); // 0 = p1, 1 = p2. Player is 1 or 2, so we reduce by one. 
            }
        }

        private static void
            CheckContext(DialogueLine dialogueLine,
                int player) // Checks if the line should add or read context. Context can only be given to one player with this method
        {
            if (string.IsNullOrEmpty(dialogueLine.contextToAdd)) //Adds context if there is something in the string
            {
                var context = dialogueLine.AddComponent<Context>();
                context.SetContextType(false); //true = getContext, false = setContext
                context.SetContext(dialogueLine.contextToAdd);
                context.SetPlayer(player - 1); // 0 = p1, 1 = p2. Player is 1 or 2, so we reduce by one. 
            }

            if (string.IsNullOrEmpty(dialogueLine
                    .contextToCheckFor)) //Checks for context if there is something in the string
            {
                var context = dialogueLine.AddComponent<Context>();
                context.SetContextType(true); //true = getContext, false = setContext
                context.SetContext(dialogueLine.contextToCheckFor);
                context.SetPlayer(player - 1); // 0 = p1, 1 = p2. Player is 1 or 2, so we reduce by one. 
            }
        }

        private static void CheckInterrupt(DialogueLine dialogueLine) // Checks if the line should have an interupt. 
        {
            if (!dialogueLine.isInterrupt) return;
            dialogueLine.AddComponent<Interrupt>();
        }*/

        #endregion
    }
}