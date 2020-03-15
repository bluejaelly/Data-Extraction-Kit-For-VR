using System;
//using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public enum Operation { Include, Ignore };

public class GazeData : MonoBehaviour
{
    [Header("VRTK")]
    [SerializeField] VRTK_Pointer pointer;
    [SerializeField] VRTK_BasePointerRenderer pointerRenderer;

    [Header("Name for the CSV File.")]
    [SerializeField] string csvName;

    [Header("Objects to Track with Gaze.")]
    [SerializeField] Operation operation;
    [SerializeField] List<string> tags;

    private string participantID;
    private float startInteraction;
    private float endInteraction;
    private string interactedObject;
    private bool startWrite;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start");
        operation = Operation.Include;
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startWrite = false;
        startInteraction = 0f;
        endInteraction = 0f;

        if (pointer != null)
        {
            RaycastHit gazeHit = pointerRenderer.GetDestinationHit();
            pointer.PointerEnter(gazeHit);
            pointer.DestinationMarkerEnter += enterGaze;
            pointer.PointerExit(gazeHit);
            pointer.DestinationMarkerExit += exitGaze;
        }
    }

    private void enterGaze(object sender, DestinationMarkerEventArgs e)
    {
        print("Started enter gaze!");
        startInteraction = Time.time;
        if (operation == Operation.Include)
        {
            if (tags.Contains(e.target.tag))
            {
                interactedObject = e.target.name;
            }
        } else
        {
            if (!tags.Contains(e.target.tag))
            {
                interactedObject = e.target.name;
            }
        }
    }

    private void exitGaze(object sender, DestinationMarkerEventArgs e)
    {
        print("Started exit gaze!");
        endInteraction = Time.time;
        string filePath = GenerateFilePath();

        if (operation == Operation.Include)
        {
            if (tags.Contains(e.target.tag))
            {
                addRecord(participantID, interactedObject, startInteraction, endInteraction, filePath);
            }
        }
        else
        {
            if (!tags.Contains(e.target.tag) || e.target.tag == "Untagged")
            {
                addRecord(participantID, interactedObject, startInteraction, endInteraction, filePath);
                Debug.Log("Writing stuff!");
            }
        }
    }

    private void addRecord(string ID, string objectName, float start, float end, string filePath) 
    {
        try
        {
            if (!startWrite)
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + objectName + "," + start + "," + end);
                }
                startWrite = true;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine(ID + "," + objectName + "," + start + "," + end);
                }
            }
        } 
        catch (Exception e)
        {
            throw new ApplicationException("Error: ", e);
        }
    }

    private string GenerateFilePath()
    {
        return Application.dataPath + "/" + csvName + ".csv";
    }
}
