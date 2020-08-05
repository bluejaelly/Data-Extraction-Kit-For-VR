using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using VRTK;

public class ControllerPositionData : MonoBehaviour
{
    [SerializeField] string csvName;

    // Controllers
    private GameObject rightController;
    private GameObject leftController;

    // Data values
    private Vector3 leftHandPosition;
    private Vector3 rightHandPosition;
    private float timeStamp;
    // private int index;

    private bool startNewWrite;

    private string participantID;

    private int dropRate;
    private int dropIndex = 0;
    string filePath;

    private void Start()
    {
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        rightController = null;
        leftController = null;
        leftHandPosition = Vector3.zero;
        rightHandPosition = Vector3.zero;
        timeStamp = 0f;
        // index = 0;
        dropRate = 3;

        filePath = GetFilePath();
        startNewWrite = true;
    }

    void Update()
    {
        if (dropIndex == 0)
        {
            UpdateData();
            UpdateRun();
        }

        dropIndex++;
        dropIndex = dropIndex % dropRate;
    }

    void UpdateData()
    {
        if (rightController == null)
        {
            // Check if actual
            rightController = VRTK_DeviceFinder.GetControllerRightHand(true);
        }

        if (leftController == null)
        {
            // Check if actual
            leftController = VRTK_DeviceFinder.GetControllerLeftHand(true);
        }

        if (rightController != null)
        {
            rightHandPosition = rightController.transform.localPosition;
        }

        if (leftController != null)
        {
            leftHandPosition = leftController.transform.localPosition;
        }

        timeStamp = Time.time;
    }

    void UpdateRun()
    {
        string[] leftData = new string[6];
        // leftData[0] = index.ToString();
        leftData[0] = participantID;
        leftData[1] = "Left";
        leftData[2] = leftHandPosition.x.ToString();
        leftData[3] = leftHandPosition.y.ToString();
        leftData[4] = leftHandPosition.z.ToString();
        leftData[5] = timeStamp.ToString();

        string[] rightData = new string[6];
        // rightData[0] = index.ToString();
        rightData[0] = participantID;
        rightData[1] = "Right";
        rightData[2] = rightHandPosition.x.ToString();
        rightData[3] = rightHandPosition.y.ToString();
        rightData[4] = rightHandPosition.z.ToString();
        rightData[5] = timeStamp.ToString();

        WriteDataLine(leftData);
        WriteDataLine(rightData);
    }

    public void WriteDataLine(string[] line)
    {
        print("Writing to file");
        try
        {
            if (startNewWrite)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("ID" + "," + "Hand" + "," + "XPos" + "," + "YPos" +
                        "," + "ZPos" + "," + "Time");
                }
                startNewWrite = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(line[0] + "," + line[1] + "," + line[2] + "," + line[3]
                        + "," + line[4] + "," + line[5]);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Something went wrong! Error: " + ex.Message);
        }
    }

    public string GetFilePath()
    {
        return Application.dataPath + "/" + participantID + "_" + csvName + ".csv";
    }
}