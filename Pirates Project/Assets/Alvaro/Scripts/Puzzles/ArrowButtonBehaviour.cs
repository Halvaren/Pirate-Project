using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class ArrowButtonBehaviour : MonoBehaviour
    {
        public VisualPuzle visualPuzle;

        public void ButtonBeingPressed()
        {
            visualPuzle.SetButtonBeingPressed(true);
        }

        public void ButtonBeingUnpressed()
        {
            visualPuzle.SetButtonBeingPressed(false);
        }
    }
}