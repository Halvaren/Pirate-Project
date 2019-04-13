using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript {
    public class ArrowButtonBehaviour : MonoBehaviour
    {
        public VisualPuzzle visualPuzzle;

        public void ButtonBeingPressed()
        {
            visualPuzzle.SetButtonBeingPressed(true);
        }

        public void ButtonBeingUnpressed()
        {
            visualPuzzle.SetButtonBeingPressed(false);
        }
    }
}