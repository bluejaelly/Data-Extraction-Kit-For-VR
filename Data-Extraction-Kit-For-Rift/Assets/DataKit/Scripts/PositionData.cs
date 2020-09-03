using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PositionData : MonoBehaviour
{
    [Header("Name for the CSV File")]
    [SerializeField] string csvName;

    [Header("Oculus Components")]
    [SerializeField] GameObject player;

    [Header("Recording Parameters")]
    [SerializeField] float recordEverySeconds;

    private string participantID;
    private string filePath;
    private bool startNewWrite;
    private bool canRecord;

    private void Start()
    {
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startNewWrite = true;
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

    private void addRecord(string ID, 
                           float time, 
                           float x, 
                           float z, 
                           string filePath)
    {
        try
        {
            if (startNewWrite)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("UserID,Time,XPos,ZPos");
                }
                startNewWrite = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + 
                                   time + "," + 
                                   x + "," + 
                                   z);
                }
            }
        }
        catch (Exception ex)
        {
            print("Something went wrong! Error: " + ex.Message);
        }
    }

    private IEnumerator delayRecord()
    {
        canRecord = false;
        yield return new WaitForSeconds(recordEverySeconds);
        canRecord = true;
    }

    public string GetFilePath()
    {
        return Application.dataPath + "/" + participantID + "_" + csvName + ".csv";
    }
}

// End of File.