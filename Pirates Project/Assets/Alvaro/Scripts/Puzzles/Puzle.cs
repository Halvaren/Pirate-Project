using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace DefinitiveScript
{
    public class Puzle : MonoBehaviour
    {
        protected bool onPuzle;
        protected bool endedPuzle = false;

        public CinemachineVirtualCamera puzleCamera;
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

            GameManager.Instance.CursorController.UnlockCursor();            
            
            puzleCamera.m_Priority = 12;

            StartPuzle();

            /* originalCameraLocalPosition = Camera.main.transform.localPosition;
            originalCameraLocalRotation = Camera.main.transform.localRotation;
            StartCoroutine(MoveCameraIntoPuzle(0.5f));*/
        }

        /* protected IEnumerator MoveCameraIntoPuzle(float time)
        {
            yield return StartCoroutine(Camera.main.GetComponent<ThirdPersonCamera>().MoveCameraTo(time, puzleCameraTrans.position, puzleCameraTrans.rotation));

            
        }*/

        protected void ExitFromPuzle()
        {
            puzleCamera.Priority = 0;

            InitializePuzle();

            GameManager.Instance.CursorController.LockCursor();  

            player.stopInput = false;
            player.MakeVisible(true);
            player = null;
        }

        /* protected IEnumerator MoveCameraOutOfPuzle(float time)
        {
            yield return StartCoroutine(Camera.main.GetComponent<ThirdPersonCamera>().ReturnCameraToLastPosition(time, originalCameraLocalPosition, originalCameraLocalRotation, false));

            
        }*/

        public bool GetEndedPuzle()
        {
            return endedPuzle;
        }
    }
}
