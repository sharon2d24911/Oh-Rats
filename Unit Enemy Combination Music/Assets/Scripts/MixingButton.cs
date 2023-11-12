using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MixingButton : MonoBehaviour
{
    public DragDropBehaviourScript dragDropBehaviourScript;
    public void OnClick()
    {
        // This function will be called when the button is clicked
        dragDropBehaviourScript.CheckIfCombine();
    }
}
