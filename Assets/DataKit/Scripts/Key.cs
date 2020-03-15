using System;
using UnityEngine;
using TMPro;
using VRTK;

public class Key : MonoBehaviour
{
    [Header("Needs to be Assigned From Every Key!")]
    [SerializeField] TextMeshProUGUI inputTextField;

    [Header("Needs to be Assigned ONLY From the Done Key!")]
    [SerializeField] VRTK_HeightAdjustTeleport heightAdjustTeleport;
    [SerializeField] GameObject dataCollectionPrefabsParent;
    [SerializeField] GameObject keyboardParent;


    private void Start()
    {
        inputTextField.text = "";
    }

    public void onClickKey()
    {
        inputTextField.text += gameObject.name;
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
        heightAdjustTeleport.GetComponent<VRTK_HeightAdjustTeleport>().enabled = true;
        dataCollectionPrefabsParent.SetActive(true);
        keyboardParent.SetActive(false);
    }
}
