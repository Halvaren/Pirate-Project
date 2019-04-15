using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DefinitiveScript
{
    public class VisualPuzle : Puzle
    {
        public int[] secretNumbers;
        public TextMeshProUGUI[] numberTexts;
        private int[] currentNumbers;

        private bool buttonBeingPressed;
        //Heredado: protected bool onPuzle;

        [SerializeField] LayerMask arrowLayer;

        private InputController m_InputController;
        public InputController InputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        public GameObject baseObj;
        private Transform baseTrans;

        public Puzle nextPuzle;

        // Start is called before the first frame update
        void Start()
        {
            baseTrans = baseObj.transform;
            InitializePuzle();
        }

        public override void StartPuzle()
        {
            onPuzle = true;
        }

        protected override void InitializePuzle()
        {
            currentNumbers = new int[] {0, 0, 0};
            for(int i = 0; i < currentNumbers.Length; i++)
            {
                numberTexts[i].text = currentNumbers[i].ToString();
            }
            
            buttonBeingPressed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                if(!buttonBeingPressed && InputController.ShootingInput)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, Mathf.Infinity, arrowLayer))
                    {
                        GameObject arrow = hit.collider.gameObject;

                        GameObject realArrow = arrow.transform.parent.gameObject.name[0] == 'A' ? arrow.transform.parent.gameObject : arrow.transform.parent.parent.gameObject;
                        string[] nameParts = realArrow.name.Split('_');

                        ChangeNumber(int.Parse(nameParts[2]), nameParts[1] == "Up" ? 1 : -1);

                        realArrow.GetComponent<Animator>().SetTrigger("PressedButton");
                        buttonBeingPressed = true;
                    }
                }
                
                if(Input.GetKeyDown(KeyCode.Z))
                {
                    FinishPuzle();
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

            if(CheckNumbers()) 
            {
                StartCoroutine(OpenPuzle(1.0f));
            }
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

        public void SetButtonBeingPressed(bool param)
        {
            buttonBeingPressed = param;
        }

        protected override void FinishPuzle()
        {
            base.FinishPuzle();
            ExitFromPuzle();
        }

        IEnumerator OpenPuzle(float time)
        {
            onPuzle = false;

            Quaternion initialRotation = baseTrans.rotation;
            Vector3 eulerAngles = initialRotation.eulerAngles;
            Quaternion finalRotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y + 90f, eulerAngles.z);

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                baseTrans.rotation = Quaternion.Slerp(initialRotation, finalRotation, elapsedTime / time);
                yield return null;
            }

            baseTrans.rotation = finalRotation;

            endedPuzle = true;

            nextPuzle.SetPlayer(player);
            nextPuzle.StartPuzle();
        }
    }
}
