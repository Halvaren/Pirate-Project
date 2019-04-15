using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class Puzle : MonoBehaviour
    {
        protected bool onPuzle;
        protected bool endedPuzle = false;
        public Transform puzleCameraPos;
        protected Transform originalCameraPos;

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

        public void SetPlayer(Player param)
        {
            player = param;
        }

        public void IntroducePuzle(Player player)
        {
            this.player = player;
            this.player.stopInput = true;
            this.player.MakeVisible(false);

            originalCameraPos = Camera.main.transform;
            StartCoroutine(IntroductionCamera(1.0f, Camera.main.transform, puzleCameraPos));
        }

        protected IEnumerator IntroductionCamera(float time, Transform originCameraTrans, Transform destinyCameraTrans)
        {
            yield return StartCoroutine(MoveCamera(time, originCameraTrans, destinyCameraTrans));
            StartPuzle();
        }

        protected void ExitFromPuzle()
        {
            InitializePuzle();

            player.stopInput = false;
            player.MakeVisible(true);
            player = null;
        }

        protected IEnumerator MoveCamera(float time, Transform originCameraTrans, Transform destinyCameraTrans)
        {
            Vector3 initialPos = originCameraTrans.position;
            Quaternion initialRot = originCameraTrans.rotation;

            Vector3 finalPos = destinyCameraTrans.position;
            Quaternion finalRot = destinyCameraTrans.rotation;

            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                
                originCameraTrans.position = Vector3.Lerp(initialPos, finalPos, elapsedTime / time);
                originCameraTrans.rotation = Quaternion.Slerp(initialRot, finalRot, elapsedTime / time);

                yield return null;
            }
            originCameraTrans.position = finalPos;
            originCameraTrans.rotation = finalRot;
        }

        public bool GetEndedPuzle()
        {
            return endedPuzle;
        }
    }
}
