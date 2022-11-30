using System;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;


namespace Dialogue.LineLogicScripts
{
    public class Comment : MonoBehaviour
    {
        public Button button;
        public String textToShow;
        public TextMeshPro textField;
        private void Awake()
        {
            Initialize();
            button.onClick.AddListener(TaskOnClick);
        }



        private void Initialize()
        {
            //If button is null, assign button to button
            button ??= GetComponent<Button>();
            //WHAT IS IS THIS CALLED???
            textField ??= GameObject.Find("DialogField").GetComponent<TextMeshPro>();
        }

        private void TaskOnClick()
        {
            if(textField != null) textField.text = textToShow;
            Destroy(this);
        }

        public void SetTextToShow(string t)
        {
            textToShow = t;
        }

        public void SetButton(Button b)
        {
            button = b;
        }

        public void SetTextField(TextMeshPro t)
        {
            textField = t;
        }
    
    }
}
