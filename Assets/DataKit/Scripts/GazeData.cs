using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class GazeData : MonoBehaviour
{
    [SerializeField] string csvName;
    private bool startWrite;
    // Start is called before the first frame update
    void Start()
    {
        startWrite = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
        return Application.dataPath + "/CSV/" + csvName + ".csv";
    }
}
