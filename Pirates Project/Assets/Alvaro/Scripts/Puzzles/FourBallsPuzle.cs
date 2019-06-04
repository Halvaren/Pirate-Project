using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class FourBallsPuzle : Puzle
    {
        public float distanceToMove;
        public float minMouseDistanceToMove;
        [SerializeField] LayerMask grabbingLayerMask;
        [SerializeField] LayerMask blockingLayerMask;

        private InputController m_InputController;
        public InputController inputController {
            get {
                if(m_InputController == null) m_InputController = GameManager.Instance.InputController;
                return m_InputController;
            }
        }

        private GameObject selectedObject;
        private Vector2 mousePosition;
        
        public GameObject[] balls; //Es importante que el orden de introducción sea Amarillo, rojo, azul, verde, en ambas arrays
        public GameObject[] points;

        private bool[] ballsInCorrectPlace;

        //Heredado: protected bool onPuzle;
        //Heredado: protected bool endedPuzle;

        //private Vector3 position;
        //private Vector3 size;

        public float scaleFactor;

        // Start is called before the first frame update
        void Start()
        {
            distanceToMove *= scaleFactor;
            InitializePuzle();
        }

        private void InitializePuzle()
        {
            for(int i = 0; i < balls.Length; i++)
            {
                int j = i + 2;
                if(j >= balls.Length) j -= balls.Length;
                balls[i].transform.position = new Vector3(points[j].transform.position.x, points[j].transform.position.y, balls[i].transform.position.z);
            }

            ballsInCorrectPlace = new bool[4] {false, false, false, false};
        }

        public override void StartPuzle()
        {
            onPuzle = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                if(inputController.GrabInput && selectedObject != null) 
                {
                    float xDifference = Input.mousePosition.x - mousePosition.x;
                    float yDifference = Input.mousePosition.y - mousePosition.y;
                    if(Mathf.Abs(xDifference) > minMouseDistanceToMove)
                    {
                        Bounds bounds = selectedObject.GetComponent<Renderer>().bounds;
                        float direction = Mathf.Sign(xDifference);

                        //position = selectedObject.transform.position + direction * Vector3.right * (distanceToMove/2 + bounds.extents.x);
                        Vector3 size = new Vector3(distanceToMove/2, bounds.extents.y, distanceToMove/2) * 0.8f;

                        //ExtDebug.DrawBoxCastBox(selectedObject.transform.position, size, Quaternion.identity, direction * Vector3.right, distanceToMove, Color.red, Color.blue, Color.green);
                        if(!Physics.BoxCast(selectedObject.transform.position, size, direction * Vector3.right, Quaternion.identity, distanceToMove, blockingLayerMask))
                        {
                            selectedObject.transform.position += Vector3.right * direction * distanceToMove;
                            mousePosition = Input.mousePosition;

                            DetectWin(selectedObject);
                        }
                    }
                    else if(Mathf.Abs(yDifference) > minMouseDistanceToMove)
                    {
                        Bounds bounds = selectedObject.GetComponent<Renderer>().bounds;
                        float direction = Mathf.Sign(yDifference);

                        //position = selectedObject.transform.position + direction * Vector3.up * (distanceToMove/2 + bounds.extents.y);
                        Vector3 size = new Vector3(bounds.extents.x, distanceToMove/2, distanceToMove/2) * 0.8f;

                        //ExtDebug.DrawBoxCastBox(selectedObject.transform.position, size, Quaternion.identity, direction * Vector3.up, distanceToMove, Color.red, Color.blue, Color.green);
                        if(!Physics.BoxCast(selectedObject.transform.position, size, direction * Vector3.up, Quaternion.identity, distanceToMove, blockingLayerMask))
                        {
                            selectedObject.transform.position += Vector3.up * direction * distanceToMove;
                            mousePosition = Input.mousePosition;   

                            DetectWin(selectedObject);
                        }
                    }
                }
                else if(selectedObject != null) selectedObject = null;

                if(inputController.ShootingInput)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;
                    if(Physics.Raycast(ray, out hit, Mathf.Infinity, grabbingLayerMask))
                    {
                        selectedObject = hit.collider.gameObject;
                        mousePosition = Input.mousePosition;
                    }
                }

                if(Input.GetKeyDown(KeyCode.Z))
                {
                    FinishPuzle();
                }
            }
            
        }

        /* void OnDrawGizmos()
        {
            //Gizmos.color = new Color(1, 0, 0, 1f);
            //Gizmos.DrawCube(position, size * 2);
        }*/

        void DetectWin(GameObject movedObject)
        {
            if(movedObject.tag == "Ball")
            {
                for(int i = 0; i < balls.Length; i++)
                {
                    //Physics.BoxCast(selectedObject.transform.position, size, direction * Vector3.right, Quaternion.identity, distanceToMove, blockingLayerMask)

                    if(movedObject == balls[i]) 
                    {
                        Vector3 ballPosition = balls[i].transform.position;
                        Vector3 pointPosition = points[i].transform.position;
                        print((ballPosition.x - pointPosition.x) + " " + (ballPosition.y - pointPosition.y));
                        if(Mathf.Abs(ballPosition.x - pointPosition.x) < 0.001f && Mathf.Abs(ballPosition.y - pointPosition.y) < 0.001f)
                        {
                            ballsInCorrectPlace[i] = true;
                            print("Una menos");
                            bool win = true;
                            for(int k = 0; k < ballsInCorrectPlace.Length; k++)
                            {
                                win = win && ballsInCorrectPlace[k];
                            }

                            if(win)
                            {
                                PuzleController.PuzleResolved();
                                endedPuzle = true;
                                FinishPuzle();
                            }
                        }
                        else ballsInCorrectPlace[i] = false;

                        print(ballsInCorrectPlace[0] + " " + ballsInCorrectPlace[1] + " " + ballsInCorrectPlace[2] + " " + ballsInCorrectPlace[3]);
                    }
                }
            }
        }

        protected override void FinishPuzle()
        {
            base.FinishPuzle();
            ExitFromPuzle();
        }
    }
}

