using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class Puzle : MonoBehaviour
    {
        protected bool onPuzle;
        protected bool endedPuzle = false;

        public Transform puzleCameraTrans;
        protected Vector3 originalCameraLocalPosition;
        protected Quaternion originalCameraLocalRotation;

        protected Player player;

        public virtual void StartPuzle()
        {

        }

        protected virtual void FinishPuzle()
        {
            onPuzle = false;
        }

        protected virtual void InitializePuzle()
        {

        }

        public void SendInfo(Player param0, Vector3 param1, Quaternion param2)
        {
            player = param0;
            originalCameraLocalPosition = param1;
            originalCameraLocalRotation = param2;
        }

        public void IntroducePuzle(Player player)
        {
            this.player = player;
            this.player.stopInput = true;
            this.player.MakeVisible(false);

            originalCameraLocalPosition = Camera.main.transform.localPosition;
            originalCameraLocalRotation = Camera.main.transform.localRotation;
            StartCoroutine(MoveCameraIntoPuzle(0.5f));
        }

        protected IEnumerator MoveCameraIntoPuzle(float time)
        {
            yield return StartCoroutine(Camera.main.GetComponent<ThirdPersonCamera>().MoveCameraTo(time, puzleCameraTrans.position, puzleCameraTrans.rotation));

            StartPuzle();
        }

        protected void ExitFromPuzle()
        {
            StartCoroutine(MoveCameraOutOfPuzle(0.5f));
        }

        protected IEnumerator MoveCameraOutOfPuzle(float time)
        {
            yield return StartCoroutine(Camera.main.GetComponent<ThirdPersonCamera>().ReturnCameraToLastPosition(time, originalCameraLocalPosition, originalCameraLocalRotation, false));

            InitializePuzle();

            player.stopInput = false;
            player.MakeVisible(true);
            player = null;
        }

        public bool GetEndedPuzle()
        {
            return endedPuzle;
        }
    }
}
