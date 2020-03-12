using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using VRTK;

public class Keyboard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputTextField;

    private void Start()
    {
        inputTextField.text = "";
    }

    public void onClickKey()
    {
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                Debug.Log(go.gameObject.name, go.gameObject);
            }
        } else
        {
            Debug.Log("Nothing!");
        }

        /*
        GameObject pressedObj = EventSystem.current.currentSelectedGameObject;
        if (pressedObj != null)
        {
            Debug.Log(EventSystem.current.currentSelectedGameObject.name);
            string keyName = EventSystem.current.currentSelectedGameObject.name;
            keyName = keyName.ToLower();
            inputTextField.text += keyName;
        } else
        {
            Debug.Log("Null Object!");
        }
        */

        /*
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        pointer.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (var go in raycastResults)
            {
                Debug.Log(go.gameObject.name, go.gameObject);
            }
        }
        */
    }

    public void onClickDelete()
    {
        try
        {
            inputTextField.text = inputTextField.text.Substring(0, inputTextField.text.Length - 1);
        } 
        catch (Exception e)
        {
            print("String length is 0");
            inputTextField.text = "";
        }
    }

    public void onClickClear()
    {
        inputTextField.text = "";
    }
}
