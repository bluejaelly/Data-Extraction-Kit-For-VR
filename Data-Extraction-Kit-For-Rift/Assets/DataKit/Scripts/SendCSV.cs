using System;
// using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UIElements;

public class SendCSV : MonoBehaviour
{
    [Header("Email Credentials")]
    [SerializeField] string fromAddress;
    [SerializeField] string fromAddressPassword;
    [SerializeField] string toAddress;

    [Header("Email Components")]
    [SerializeField] string emailTitle;
    [SerializeField] string emailBody;

    [Header("File Generators")]
    [SerializeField] GazeData gazeData;
    [SerializeField] PositionData positionData;
    [SerializeField] ControllerPositionData controllerPositionData;
    [SerializeField] ControllerPointerData controllerPointerData;

    private List<string> fileNames;

    private void Start()
    {
        fileNames = new List<string>();

        fileNames.Add(gazeData.GetFilePath());
        fileNames.Add(positionData.GetFilePath());
        fileNames.Add(controllerPositionData.GetFilePath());
        fileNames.Add(controllerPointerData.GetFilePath());
    }

    public void OnApplicationQuit()
    {
        SendSmtpMail();
    }
    private void SendSmtpMail()
    {
        print("Sending Mail");
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(fromAddress);
        mail.To.Add(toAddress);
        mail.Subject = emailTitle;
        mail.Body = emailBody;

        print("Attaching Files");
        foreach (string filePath in fileNames)
        {
            Attachment data = new Attachment(filePath, MediaTypeNames.Application.Octet);
            // Add time stamp information for the file.
            ContentDisposition disposition = data.ContentDisposition;
            disposition.CreationDate = System.IO.File.GetCreationTime(filePath);
            disposition.ModificationDate = System.IO.File.GetLastWriteTime(filePath);
            disposition.ReadDate = System.IO.File.GetLastAccessTime(filePath);
            // Add the file attachment to this email message.
            mail.Attachments.Add(data);
        }
        print("Attached Files");

        // you can use others too.
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(fromAddress, fromAddressPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
        delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        { return true; };

        print("Attemping to Send Files");
        try
        {
            smtpServer.Send(mail);
            Debug.Log("Email Sent!");
        }
        catch (Exception ex)
        {
            Debug.Log("Something went wrong!");
            Debug.Log(ex.ToString());
        }
    }
}
