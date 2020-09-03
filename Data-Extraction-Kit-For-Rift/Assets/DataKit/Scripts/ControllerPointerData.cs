using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class ControllerPointerData : MonoBehaviour
{
    [Header("Name for the CSV File")]
    [SerializeField] string csvName;

    [Header("VRTK Components")]
    [SerializeField] VRTK_Pointer leftPointer;
    [SerializeField] VRTK_BasePointerRenderer leftBasePointerRenderer;
    [SerializeField] VRTK_Pointer rightPointer;
    [SerializeField] VRTK_BasePointerRenderer rightBasePointerRenderer;

    private float startInteractionLeft;
    private float startInteractionRight;
    private float endInteractionLeft;
    private float endInteractionRight;
    private string objectInteractedLeft;
    private string objectInteractedRight;

    private string participantID;
    private bool startNewWrite;
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        startInteractionLeft = 0f;
        startInteractionRight = 0f;
        endInteractionLeft = 0f;
        endInteractionRight = 0f;
        objectInteractedLeft = "";
        objectInteractedRight = "";

        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startNewWrite = true;
        filePath = GetFilePath();

        if (leftPointer != null && rightPointer != null && leftBasePointerRenderer != null && rightBasePointerRenderer != null)
        {
            RaycastHit leftControllerHit = leftBasePointerRenderer.GetDestinationHit();
            RaycastHit rightControllerHit = rightBasePointerRenderer.GetDestinationHit();

            leftPointer.PointerEnter(leftControllerHit);
            leftPointer.DestinationMarkerEnter += enterLeft;

            rightPointer.PointerEnter(rightControllerHit);
            rightPointer.DestinationMarkerEnter += enterRight;

            leftPointer.PointerExit(leftControllerHit);
            leftPointer.DestinationMarkerExit += exitLeft;

            rightPointer.PointerExit(rightControllerHit);
            rightPointer.DestinationMarkerExit += exitRight;
        }
    }

    private void enterLeft(object sender, DestinationMarkerEventArgs e)
    {
        startInteractionLeft = Time.time;
        objectInteractedLeft = e.target.name;
    }

    private void enterRight(object sender, DestinationMarkerEventArgs e)
    {
        startInteractionRight = Time.time;
        objectInteractedRight = e.target.name;
    }

    private void exitLeft(object sender, DestinationMarkerEventArgs e)
    {
        endInteractionLeft = Time.time;

        addRecord(participantID,
                 objectInteractedLeft,
                 1,
                 0,
                 startInteractionLeft,
                 endInteractionLeft,
                 filePath);

        startInteractionLeft = 0f;
        endInteractionLeft = 0f;
    }

    private void exitRight(object sender, DestinationMarkerEventArgs e)
    {
        endInteractionRight = Time.time;

        addRecord(participantID,
                  objectInteractedRight,
                  0,
                  1,
                  startInteractionRight,
                  endInteractionRight,
                  filePath);

        startInteractionRight = 0f;
        endInteractionRight = 0f;
    }

    private void addRecord(string ID, 
                           string objectName, 
                           int leftControllerPointer, 
                           int rightControllerPointer, 
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
                    file.WriteLine("ID,ObjectName,LeftControllerPoint,RightControllerPoint,StartTime,EndTime");
                }
                startNewWrite = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + 
                                   objectName + "," + 
                                   leftControllerPointer + "," + 
                                   rightControllerPointer + "," + 
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