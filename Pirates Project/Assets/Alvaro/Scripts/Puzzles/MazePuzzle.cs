using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class MazePuzzle : MonoBehaviour
    {
        public GameObject ball;
        private CharacterController ballController;
        private Transform ballTransform;
        //private Material ballMaterial;

        private InputController m_BallInputController;
        public InputController ballInputController
        {
            get {
                if(m_BallInputController == null) m_BallInputController = GameManager.Instance.InputController;
                return m_BallInputController;
            }
        }

        public float ballTranslationSpeed = 5f;
        public float ballRotationSpeed = 100f;
        public float ballAcceleration = 2f;
        private Vector2 direction;
        private Vector2 rotation;

        // Start is called before the first frame update
        void Awake()
        {
            ballController = ball.GetComponent<CharacterController>();
            ballTransform = ball.GetComponent<Transform>();
            //ballMaterial = ball.GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            direction.x = Mathf.Lerp(direction.x, ballInputController.Horizontal, 1f / ballAcceleration);
            direction.y = Mathf.Lerp(direction.y, ballInputController.Vertical, 1f / ballAcceleration);

            rotation.x = Mathf.Lerp(rotation.x, ballInputController.Vertical, 1f / ballAcceleration);
            rotation.y = Mathf.Lerp(rotation.y, -ballInputController.Horizontal, 1f / ballAcceleration);

            print(rotation.y + " " + rotation.x);
        }

        void FixedUpdate()
        {
            direction = Camera.main.transform.TransformDirection(direction);
            rotation = Camera.main.transform.TransformDirection(rotation);

            ballController.Move(direction * ballTranslationSpeed * Time.deltaTime);
            ballTransform.Rotate(rotation * ballTranslationSpeed * Time.deltaTime * (2 * Mathf.PI * transform.localScale.magnitude) * 10, Space.World);
            //ballMaterial.SetTextureOffset("_MainTex", ballInput * ballSpeed);
        }
    }
}