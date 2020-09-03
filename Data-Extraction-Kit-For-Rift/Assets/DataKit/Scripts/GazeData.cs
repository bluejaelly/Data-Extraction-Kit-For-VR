using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class GazeData : MonoBehaviour
{
    [Header("Name for the CSV File")]
    [SerializeField] string csvName;

    [Header("VRTK Components")]
    [SerializeField] VRTK_Pointer pointer;
    [SerializeField] VRTK_BasePointerRenderer pointerRenderer;

    private string participantID;
    private float startInteraction;
    private float endInteraction;
    private string interactedObject;
    private bool startNewWrite;
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startNewWrite = true;
        startInteraction = 0f;
        endInteraction = 0f;
        filePath = GetFilePath();

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
        startInteraction = Time.time;
        interactedObject = e.target.name;
    }

    private void exitGaze(object sender, DestinationMarkerEventArgs e)
    {
        endInteraction = Time.time;
        addRecord(participantID, 
                  interactedObject, 
                  startInteraction, 
                  endInteraction, 
                  filePath);
    }

    private void addRecord(string ID, 
                           string objectName, 
                           float startTime, 
                           float endTime, 
                           string filePath) 
    {
        try
        {
            if (startNewWrite)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("ID,ObjectName,StartTime,EndTime");
                }
                startNewWrite = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + 
                                   objectName + "," +
                                   startTime + "," +
                                   endTime);
                }
            }
        } 
        catch (Exception ex)
        {
            print("Something went wrong! Error: " + ex.Message);
        }
    }

    public string GetFilePath()
    {
        return Application.dataPath + "/" + participantID + "_" + csvName + ".csv";
    }
}

// End of File.