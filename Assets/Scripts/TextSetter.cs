using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextSetter : MonoBehaviour {

    [SerializeField]
    Text target;

	public void SetText (string text) {
        target.text = text;
	}
}
