using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class MazePuzzle : MonoBehaviour
    {
        private Rigidbody rigidbody;

        public Transform startPoint;

        private InputController m_BallInputController;
        public InputController ballInputController
        {
            get {
                if(m_BallInputController == null) m_BallInputController = GameManager.Instance.InputController;
                return m_BallInputController;
            }
        }

        public float ballSpeed = 5f;
        public float ballAcceleration = 2f;
        private Vector2 direction;
        private Vector2 rotation;

        public float finishDistance = 0.5f;

        private bool onPuzle = false;

        // Start is called before the first frame update
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            ballSpeed *= transform.parent.localScale.x;
            ballAcceleration *= transform.parent.localScale.x;
            finishDistance *= transform.parent.localScale.x;
            StartPuzle();
        }

        public void StartPuzle()
        {
            transform.position = new Vector3(startPoint.position.x, startPoint.position.y, transform.position.z);
            onPuzle = true;
        }

        public void FinishPuzle()
        {
            onPuzle = false;
            direction = Vector2.zero;
            rotation = Vector2.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if(onPuzle)
            {
                direction.x = Mathf.Lerp(direction.x, ballInputController.Horizontal, 1f / ballAcceleration);
                direction.y = Mathf.Lerp(direction.y, ballInputController.Vertical, 1f / ballAcceleration);

                rotation.x = Mathf.Lerp(rotation.x, ballInputController.Vertical, 1f / ballAcceleration);
                rotation.y = Mathf.Lerp(rotation.y, -ballInputController.Horizontal, 1f / ballAcceleration);
            }
        }

        void FixedUpdate()
        {
            direction = Camera.main.transform.TransformDirection(direction);
            rotation = Camera.main.transform.TransformDirection(rotation);

            Vector2 newPositionV2 = direction * ballSpeed * Time.deltaTime;
            Vector3 newPositionV3 = new Vector3(newPositionV2.x, newPositionV2.y, 0f);
            rigidbody.MovePosition(rigidbody.position + newPositionV3);
            transform.Rotate(rotation * ballSpeed * Time.deltaTime * (2 * Mathf.PI * transform.localScale.magnitude) * 20, Space.World);
        }

        void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "EndPoint")
            {
                float distance = (other.gameObject.transform.position - transform.position).magnitude;
                print(distance);
                if(distance < finishDistance)
                    FinishPuzle();
            }
        }
    }
}