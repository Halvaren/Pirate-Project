using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class MazePuzle : Puzle
    {
        public Transform referenceTransform;

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

        public float scale;

        //Heredado: protected bool onPuzle = false;

        // Start is called before the first frame update
        void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            ballSpeed *= scale;
            ballAcceleration *= scale;
            finishDistance *= scale;
            InitializePuzle();
        }

        protected override void InitializePuzle()
        {
            transform.position = new Vector3(startPoint.position.x, startPoint.position.y, transform.position.z);
        }

        public override void StartPuzle()
        {
            onPuzle = true;
        }

        protected override void FinishPuzle()
        {
            base.FinishPuzle();
            direction = Vector2.zero;
            rotation = Vector2.zero;

            ExitFromPuzle();
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

                if(Input.GetKeyDown(KeyCode.Z))
                {
                    FinishPuzle();
                }
            }
        }

        void FixedUpdate()
        {
            if(onPuzle)
            {
                direction = Camera.main.transform.TransformDirection(direction);
                rotation = Camera.main.transform.TransformDirection(rotation);

                Vector3 newPositionV3 = direction * ballSpeed * Time.deltaTime;
                rigidbody.MovePosition(rigidbody.position + newPositionV3);
                transform.Rotate(rotation * ballSpeed * Time.deltaTime * (2 * Mathf.PI * transform.localScale.magnitude) * 100, Space.World);
            }
        }

        void OnTriggerStay(Collider other)
        {
            if(other.gameObject.tag == "EndPoint" && onPuzle)
            {
                float distance = (other.gameObject.transform.position - transform.position).magnitude;
                if(distance < finishDistance)
                {
                    endedPuzle = true;
                    PuzleController.PuzleResolved();
                    FinishPuzle();
                }
            }
        }
    }
}