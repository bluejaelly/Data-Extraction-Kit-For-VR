using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VRTK;

public class ControllerPointerData : MonoBehaviour
{
    [SerializeField] string csvName;

    [Header("VRTK Components")]
    [SerializeField] VRTK_Pointer leftPointer;
    [SerializeField] VRTK_BasePointerRenderer leftBasePointerRenderer;
    [SerializeField] VRTK_InteractGrab leftGrab;
    [SerializeField] VRTK_Pointer rightPointer;
    [SerializeField] VRTK_BasePointerRenderer rightBasePointerRenderer;
    [SerializeField] VRTK_InteractGrab rightGrab;

    private float startInteractionLeft;
    private float startInteractionRight;
    private float endInteractionLeft;
    private float endInteractionRight;

    private float startClickLeft;
    private float startClickRight;
    private float endClickLeft;
    private float endClickRight;

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

    private string objectInteractedLeft;
    private string objectInteractedRight;
    private string participantID;
    private bool startWriting;

    private string filePath;

    // Start is called before the first frame update
    void Start()
    {
        startInteractionLeft = 0f;
        startInteractionRight = 0f;
        endInteractionLeft = 0f;
        endInteractionRight = 0f;

        startClickLeft = 0f;
        startClickRight = 0f;
        endClickLeft = 0f;
        endClickRight = 0f;

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

        objectInteractedLeft = "";
        objectInteractedRight = "";
        participantID = PlayerPrefs.GetString("ID", "INVALID");
        startWriting = true;

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

        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
        {
            startClickLeft = Time.time;

            if (e.target.gameObject.CompareTag("Marked"))
            {
                e.target.gameObject.tag = "Untagged";
            }
            else
            {
                e.target.gameObject.tag = "Marked";
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
        {
            endClickLeft = Time.time;

            bool marked = false;

            if (e.target.gameObject.CompareTag("Marked"))
                marked = true;

            addRecord(participantID, objectInteractedLeft, startClickLeft, endClickLeft, e.target.position.x, e.target.position.x, e.target.position.y, e.target.position.y, e.target.position.z, e.target.position.z, filePath, 1, 0, 1, 0, 0, 0, (marked ? 1 : 0));

            startClickLeft = 0f;
            endClickLeft = 0f;
        }
    }

    private void enterRight(object sender, DestinationMarkerEventArgs e)
    {
        startInteractionRight = Time.time;
        objectInteractedRight = e.target.name;

        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            startClickRight = Time.time;

            if (e.target.gameObject.CompareTag("Marked"))
            {
                e.target.gameObject.tag = "Untagged";
            }
            else
            {
                e.target.gameObject.tag = "Marked";
            }
        }

        if (OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger))
        {
            endClickRight = Time.time;

            bool marked = false;

            if (e.target.gameObject.CompareTag("Marked"))
                marked = true;

            addRecord(participantID, objectInteractedRight, startClickRight, endClickRight, e.target.position.x, e.target.position.x, e.target.position.y, e.target.position.y, e.target.position.z, e.target.position.z, filePath, 0, 1, 0, 0, 1, 0, (marked ? 1 : 0));

            startClickRight = 0f;
            endClickRight = 0f;
        }
    }

    private void exitLeft(object sender, DestinationMarkerEventArgs e)
    {
        endInteractionLeft = Time.time;
        addRecord(participantID, objectInteractedLeft, startInteractionLeft, endInteractionLeft, e.target.position.x, e.target.position.x, e.target.position.y, e.target.position.y, e.target.position.z, e.target.position.z, filePath, 1);

        startInteractionLeft = 0f;
        endInteractionLeft = 0f;
    }

    private void exitRight(object sender, DestinationMarkerEventArgs e)
    {
        endInteractionRight = Time.time;
        addRecord(participantID, objectInteractedRight, startInteractionRight, endInteractionRight, e.target.position.x, e.target.position.x, e.target.position.y, e.target.position.y, e.target.position.z, e.target.position.z, filePath, 0, 1);

        startInteractionRight = 0f;
        endInteractionRight = 0f;
    }

    public void grabLeft()
    {
        grabbedObjectLeft = leftGrab.GetGrabbedObject();

        if (grabbedObjectLeft != null)
        {
            startGrabPosLeft = grabbedObjectLeft.transform.position;
        }

        startGrabLeft = Time.time;
    }

    public void grabRight()
    {
        grabbedObjectRight = rightGrab.GetGrabbedObject();

        if (grabbedObjectRight != null)
        {
            startGrabPosRight = grabbedObjectRight.transform.position;
        }

        startGrabRight = Time.time;
    }

    public void ungrabLeft()
    {
        endGrabLeft = Time.time;
        endGrabPosLeft = grabbedObjectLeft.transform.position;

        addRecord(participantID, grabbedObjectLeft.name, startGrabLeft, endGrabLeft, startGrabPosLeft.x, endGrabPosLeft.x,
                  startGrabPosLeft.y, endGrabPosLeft.y, startGrabPosLeft.z, endGrabPosLeft.z, filePath,
                  0, 0, 0, 1, 0, 0, 0);

        grabbedObjectLeft = null;
    }

    public void ungrabRight()
    {
        endGrabRight = Time.time;
        endGrabPosRight = grabbedObjectRight.transform.position;

        addRecord(participantID,grabbedObjectRight.name, startGrabRight, endGrabRight, startGrabPosRight.x, endGrabPosRight.x,
                  startGrabPosRight.y, endGrabPosRight.y, startGrabPosRight.z, endGrabPosRight.z, filePath,
                  0, 0, 0, 0, 0, 1, 0);

        grabbedObjectRight = null;
    }

    private void addRecord(string ID, string objectName, float startTime, float endTime, float objXInt, float objXEnd,
                           float objYInit, float objYEnd, float objZInit, float objZEnd, string filePath,
                           int leftControllerPointer = 0, int rightControllerPointer = 0, int primaryIndexTrigger = 0,
                           int primaryHandTrigger = 0, int secondaryIndexTrigger = 0, int secondaryHandTrigger = 0, int isMarked = 0)
    {
        print("Writing to file");
        try
        {
            if (startWriting)
            {
                using (StreamWriter file = new StreamWriter(@filePath, false))
                {
                    file.WriteLine("ID,ObjectName,LeftControlPoint,RightControlPoint,PrimaryIndexTrigger,PrimaryHandTrigger,SecondaryIndexTrigger,SecondaryHandTrigger,IsMarked,InterInit,InterEnd,ObjXInit,ObjXEnd,ObjYInit,ObjYEnd,ObjZInit,ObjZEnd");
                }
                startWriting = false;
            }
            else
            {
                using (StreamWriter file = new StreamWriter(@filePath, true))
                {
                    file.WriteLine(ID + "," + objectName + "," + leftControllerPointer + "," + rightControllerPointer + ","
                                   + primaryIndexTrigger + "," + primaryHandTrigger + "," + secondaryIndexTrigger + ","
                                   + secondaryHandTrigger + "," + isMarked + "," + startTime + "," + endTime + ","
                                   + objXInt + "," + objXEnd + "," + objYInit + "," + objYEnd + "," + objZInit + "," + objZEnd);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Something went wrong! Error: " + ex.Message);
        }
    }

    string GetFilePath()
    {
        return Application.dataPath + "/" + participantID + "_" + csvName + ".csv";
    }
}

// End of File.