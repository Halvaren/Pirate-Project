using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DefinitiveScript
{
    public class VisualPuzzle : MonoBehaviour
    {
        public int[] secretNumbers;
        public TextMeshProUGUI[] numberTexts;
        private int[] currentNumbers;

        private bool onPuzle;

        [SerializeField] LayerMask arrowLayer;

        private InputController m_InputController;
            public InputController InputController {
                get {
                    if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                    return m_InputController;
                }
            }

        // Start is called before the first frame update
        void Start()
        {
            StartPuzle();
        }

        public void StartPuzle()
        {
            currentNumbers = new int[] {0, 0, 0};
            for(int i = 0; i < currentNumbers.Length; i++)
            {
                numberTexts[i].text = currentNumbers[i].ToString();
            }
            
            onPuzle = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                if(InputController.ShootingInput)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, Mathf.Infinity, arrowLayer))
                    {
                        GameObject arrow = hit.collider.gameObject;

                        GameObject realArrow = arrow.transform.parent.gameObject.name[0] == 'A' ? arrow.transform.parent.gameObject : arrow.transform.parent.parent.gameObject;
                        string[] nameParts = realArrow.name.Split('_');

                        ChangeNumber(int.Parse(nameParts[2]), nameParts[1] == "Up" ? 1 : -1);
                    }
                }
            }
        }

        void ChangeNumber(int number, int increase)
        {
            int i = number - 1;
            currentNumbers[i] += increase;
            if(currentNumbers[i] > 9) currentNumbers[i] = 0;
            else if(currentNumbers[i] < 0) currentNumbers[i] = 9;

            numberTexts[i].text = currentNumbers[i].ToString();

            if(CheckNumbers()) FinishPuzle();
        }

        IEnumerator PressArrow(float time)
        {
            yield return null;
        }

        bool CheckNumbers()
        {
            bool result = true;
            for(int i = 0; i < currentNumbers.Length; i++)
            {
                result = result && currentNumbers[i] == secretNumbers[i];
            }

            return result;
        }

        void FinishPuzle()
        {
            onPuzle = false;
        }
    }
}
