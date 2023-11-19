using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/********************
 * NOT WORKING
 * DON'T USE
 * THANK YOU
 ********************/


// Add IPointerClickHandler interface to let Unity know you want to
// catch and handle clicks (or taps on Mobile)
public class Hyperlinks : MonoBehaviour, IPointerClickHandler
{

    [SerializeField, Tooltip("The UI GameObject having the TextMesh Pro component.")]
    private TMP_Text text = default;

    // Callback for handling clicks.
    public void OnPointerClick(PointerEventData eventData)
    {
        // First, get the index of the link clicked. Each of the links in the text has its own index.
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);

        // As the order of the links can vary easily (e.g. because of multi-language support),
        // you need to get the ID assigned to the links instead of using the index as a base for our decisions.
        // you need the LinkInfo array from the textInfo member of the TextMesh Pro object for that.
        var linkId = text.textInfo.linkInfo[linkIndex].GetLinkID();

		// Let's see that web page!
		Application.OpenURL(linkId);
	}
}