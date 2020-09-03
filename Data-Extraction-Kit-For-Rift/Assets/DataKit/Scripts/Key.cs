using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Key : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI inputTextField;

    private void Start()
    {
        inputTextField.text = "";
    }

    public void onClickKey()
    {
        inputTextField.text += (gameObject.name).ToLower();
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

    public void onClickDone()
    {
        PlayerPrefs.SetString("ID", inputTextField.text);
        inputTextField.text = "";
        SceneManager.LoadScene(1);
    }
}

// End of File.