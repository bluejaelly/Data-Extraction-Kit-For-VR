using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class ControllerGrabData : MonoBehaviour
{
    [Header("Name for the CSV File")]
    [SerializeField] string csvName;

    [Header("VRTK Components")]
    [SerializeField] VRTK_InteractGrab leftGrab;
    [SerializeField] VRTK_InteractGrab rightGrab;

    private float startGrabLeft;
    private float startGrabRight;
    private float endGrabLeft;
    private float endGrabRight;
    private Vector3 startGrabPosLeft;
    private Vector3 startGrabPosRight;
    private Vector3 endGrabPosLeft;
    private Vector3 endGrabPosRight;
    private GameObject grabbedObjectLeft;
    private GameObject grabbedObjectRight;

    private string participantID;
    private bool startNewWrite;
    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        startGrabLeft = 0f;
        startGrabRight = 0f;
        endGrabLeft = 0f;
        endGrabRight = 0f;
        startGrabPosLeft = new Vector3(0, 0, 0);
        startGrabPosRight = new Vector3(0, 0, 0);
        endGrabPosLeft = new Vector3(0, 0, 0);
        endGrabPosRight = new Vector3(0, 0, 0);
        grabbedObjectLeft = null;
        grabbedObjectRight = null;

        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startNewWrite = true;
        filePath = GetFilePath();

        if (leftGrab != null && rightGrab != null)
        {
            leftGrab.ControllerGrabInteractableObject += grabLeft;
            rightGrab.ControllerGrabInteractableObject += grabRight;
            leftGrab.ControllerUngrabInteractableObject += ungrabLeft;
            rightGrab.ControllerUngrabInteractableObject += ungrabRight;
        }
    }

    private void grabLeft(object sender, ObjectInteractEventArgs e)
    {
        grabbedObjectLeft = e.target;

        if (grabbedObjectLeft != null)
        {
            startGrabPosLeft = grabbedObjectLeft.transform.position;
        }

        startGrabLeft = Time.time;
    }

    public void grabRight(object sender, ObjectInteractEventArgs e)
    {
        grabbedObjectRight = e.target;

        if (grabbedObjectRight != null)
        {
            startGrabPosRight = grabbedObjectRight.transform.position;
        }

        startGrabRight = Time.time;
    }

    public void ungrabLeft(object sender, ObjectInteractEventArgs e)
    {
        endGrabLeft = Time.time;
        endGrabPosLeft = grabbedObjectLeft.transform.position;

        addRecord(participantID, 
                  grabbedObjectLeft.name, 
                  1, 
                  0, 
                  startGrabLeft, 
                  endGrabLeft, 
                  startGrabPosLeft.x, 
                  startGrabPosLeft.y, 
                  startGrabPosLeft.z, 
                  endGrabPosLeft.x, 
                  endGrabPosLeft.y, 
                  endGrabPosLeft.z, 
                  filePath);
    }

    public void ungrabRight(object sender, ObjectInteractEventArgs e)
    {
        endGrabRight = Time.time;
        endGrabPosRight = grabbedObjectRight.transform.position;

        addRecord(participantID,
                  grabbedObjectRight.name,
                  0,
                  1,
                  startGrabRight,
                  endGrabRight,
                  startGrabPosRight.x,
                  startGrabPosRight.y,
                  startGrabPosRight.z,
                  endGrabPosRight.x,
                  endGrabPosRight.y,
                  endGrabPosRight.z,
                  filePath);
    }

    private void addRecord(string ID, 
                           string objectName, 
                           int leftControllerGrab, 
                           int rightControllerGrab, 
                           float startTime, 
                           float endTime, 
                           float objXStart,
                           float objYStart,
                           float objZStart,
                           float objXEnd,
                           float objYEnd,
                           float objZEnd,
                           string filePath)
    {
        try
        {
            if (startNewWrite)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("ID,ObjectName,LeftControllerGrab,RightControllerGrab,StartTime,EndTime,objXStart,objYStart,objZStart,objXEnd,objYEnd,objZEnd");
                }
                startNewWrite = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + 
                                   objectName + "," + 
                                   leftControllerGrab + "," + 
                                   rightControllerGrab + "," + 
                                   startTime + "," + 
                                   endTime + "," + 
                                   objXStart + "," + 
                                   objYStart + "," + 
                                   objZStart + "," + 
                                   objXEnd + "," + 
                                   objYEnd + "," + 
                                   objZEnd);
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