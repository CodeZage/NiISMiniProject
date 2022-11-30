using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue_Scripts
{
    [RequireComponent(typeof(Button))]
    public class DialogueOption : MonoBehaviour
    {
        private ConversationTracker _conversationTracker;
        private Button _button;
        private string _optionText;
        private bool _comment;

        public void Setup(int targetIndex, string dialogueText, bool isComment)
        {
            _conversationTracker = FindObjectOfType<ConversationTracker>();
            _button = GetComponent<Button>();
            _optionText = dialogueText;
            _button.GetComponentInChildren<TextMeshProUGUI>().text = dialogueText;
            _button.onClick.AddListener(() => OnClick(targetIndex));
            _comment = isComment;
        }

        private void OnClick(int index)
        {
            if (_comment)
            {
                Destroy(gameObject);
                return;
            }

            if (_conversationTracker != null) _conversationTracker.OnPlayerChose(index, _optionText);
        }
    }
}