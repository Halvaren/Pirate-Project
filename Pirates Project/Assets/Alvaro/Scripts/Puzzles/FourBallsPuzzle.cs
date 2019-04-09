using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class FourBallsPuzzle : MonoBehaviour
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

        private Vector3 position;
        private Vector3 size;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(inputController.GrabInput && selectedObject != null) 
            {
                float xDifference = Input.mousePosition.x - mousePosition.x;
                float yDifference = Input.mousePosition.y - mousePosition.y;
                if(Mathf.Abs(xDifference) > minMouseDistanceToMove)
                {
                    Bounds bounds = selectedObject.GetComponent<Renderer>().bounds;
                    float direction = Mathf.Sign(xDifference);

                    position = selectedObject.transform.position + direction * Vector3.right * (distanceToMove/2 + bounds.extents.x);
                    size = new Vector3(distanceToMove/2, distanceToMove/2, distanceToMove/2);

                    if(!Physics.BoxCast(selectedObject.transform.position, size, direction * Vector3.right, Quaternion.identity, distanceToMove/2 + bounds.extents.x, blockingLayerMask))
                    {
                        selectedObject.transform.position += Vector3.right * direction * distanceToMove;
                        mousePosition = Input.mousePosition;
                    }
                }
                else if(Mathf.Abs(yDifference) > minMouseDistanceToMove)
                {
                    Bounds bounds = selectedObject.GetComponent<Renderer>().bounds;
                    float direction = Mathf.Sign(yDifference);

                    position = selectedObject.transform.position + direction * Vector3.up * (distanceToMove/2 + bounds.extents.y);
                    size = new Vector3(bounds.extents.x, distanceToMove/2, distanceToMove/2);

                    if(!Physics.BoxCast(selectedObject.transform.position, size, direction * Vector3.up, Quaternion.identity, distanceToMove/2 + bounds.extents.y, blockingLayerMask))
                    {
                        selectedObject.transform.position += Vector3.up * direction * distanceToMove;
                        mousePosition = Input.mousePosition;
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
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, 1f);
            Gizmos.DrawCube(position, size * 2);
        }
    }
}

