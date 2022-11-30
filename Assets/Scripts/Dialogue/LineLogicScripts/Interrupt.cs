using TMPro;
using UnityEngine;

namespace Dialogue.LineLogicScripts
{
    public class Interrupt : MonoBehaviour
    {
        private float _timer = 0;
        private float _timerEnd = 5;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            gameObject.GetComponentInChildren<TextMeshPro>().text =
                "!! " + gameObject.GetComponentInChildren<TextMeshPro>().text + " !!";
        }

        private void Update()
        {
            _timer += Time.deltaTime;
            if(_timer >= _timerEnd) Destroy(this);
        }
    }
}
