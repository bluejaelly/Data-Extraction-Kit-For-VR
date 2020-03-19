using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionData : MonoBehaviour
{
    [Header("CSV File Name")]
    [SerializeField] string csvName;

    [Header("OVRCameraRig")]
    [SerializeField] GameObject player;

    private string participantID;
    private string filePath;
    private bool startWriting;
    private bool canRecord;

    private void Start()
    {
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startWriting = false;
        canRecord = true;
        filePath = GetFilePath();
    }

    private void Update()
    {
        if (canRecord)
        {
            addRecord(participantID, Time.time, player.transform.position.x, player.transform.position.z, filePath);
            StartCoroutine(delayRecord());
        }
    }

    private void addRecord(string ID, float time, float x, float z, string filePath)
    {
        print("Writing to file");
        try
        {
            if (!startWriting)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("UserID" + "," + "Time" + "," + "XPos" + "," + "ZPos");
                }
                startWriting = true;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + time + "," + x + "," + z);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Something went wrong! Error: " + ex.Message);
        }
    }

    private IEnumerator delayRecord()
    {
        canRecord = false;
        yield return new WaitForSeconds(0.2f);
        canRecord = true;
    }

    string GetFilePath()
    {
        return Application.dataPath + "/" + participantID + "_" + csvName + ".csv";
    }
}