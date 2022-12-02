using Dialogue.LineLogicScripts;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    [RequireComponent(typeof(Button))]
    public class DialogueOption : MonoBehaviour
    {
        private ConversationTracker _conversationTracker;
        private ConversationStorage _conversationStorage;

        private string _informationString;
        private bool _requiresInformation, _hasRequiredInformation;

        private Button _button;
        private int _player;
        private string _optionText;
        private bool _comment;

        public void Setup(int player, int targetIndex, string dialogueText, bool isComment, bool isInterrupt)
        {
            _conversationTracker = FindObjectOfType<ConversationTracker>();
            _player = player;
            _button = GetComponent<Button>();
            _optionText = dialogueText;
            _button.GetComponentInChildren<TextMeshProUGUI>().text = dialogueText;
            _button.onClick.AddListener(() => OnClick(targetIndex));
            _comment = isComment;

            if (isInterrupt)
            {
                _button.AddComponent<Interrupt>();
            }
        }

        public void Setup(int player, int targetIndex, string dialogueText, bool requiresInformation,
            string information, bool isComment, bool isInterrupt)
        {
            Setup(player, targetIndex, dialogueText, isComment, isInterrupt);
            _requiresInformation = requiresInformation;
            
            if (_requiresInformation)
                _hasRequiredInformation =
                    _conversationStorage.CheckInformation(player: _player, information: information);
        }

        private void OnClick(int index)
        {
            if (_comment)
            {
                _conversationTracker.ChangeTopText(_optionText);
                Destroy(gameObject);
                return;
            }

            if (_requiresInformation && !_hasRequiredInformation)
            {
                Destroy(gameObject);
                return;
            }

            if (_conversationTracker != null) _conversationTracker.OnPlayerChoice(_player, index, _optionText);
        }
    }
}