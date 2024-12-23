using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI; // MRTK�� ��ư ���� ���ӽ����̽�
using TMPro; // TextMeshPro ���

public class UdpSender : MonoBehaviour
{
    private UdpClient udpClient;
    public string ipAddress = "192.168.254.26"; // Limo ��Ʈ���� IP �ּ�
    public int port = 2020; // Ubuntu���� ������ ��Ʈ ��ȣ
    public PressableButtonHoloLens2 sendButton; // MRTK�� PressableButtonHoloLens2 ���
    public int inputType; //1: UI, 2: Gesutre

    public int gestureNUM;

    
    private TextMeshPro buttonText;

    void Start()
    {
        inputType = 1;
        // UdpClient �ʱ�ȭ
        udpClient = new UdpClient();

        // ��ư Ŭ�� �� OnSendData ȣ��
        //sendButton.OnButtonPressed.AddListener(OnSendData);

        // ��ư ���� TextMeshPro ������Ʈ�� ã�Ƽ� ����
        buttonText = sendButton.GetComponentInChildren<TextMeshPro>();
    }
    void Update()
    {
        Debug.Log("NUM:" + gestureNUM);
        if(inputType == 2)
        {
            if(gestureNUM == 1)
            {
                try
                {
                    string message = "GO"; // ���� �޽���
                    byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                    // IP �ּ� �� ��Ʈ�� ������ ����
                    udpClient.Send(data, data.Length, ipAddress, port);
                    Debug.Log("Data sent: " + message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending UDP data: " + e.Message);
                }
            }
            else if(gestureNUM == 2)
            {
                try
                {
                    string message = "STOP"; // ���� �޽���
                    byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                    // IP �ּ� �� ��Ʈ�� ������ ����
                    udpClient.Send(data, data.Length, ipAddress, port);
                    Debug.Log("Data sent: " + message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending UDP data: " + e.Message);
                }
            }
            else if(gestureNUM == 3)
            {
                try
                {
                    string message = "ROTATE"; // ���� �޽���
                    byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                    // IP �ּ� �� ��Ʈ�� ������ ����
                    udpClient.Send(data, data.Length, ipAddress, port);
                    Debug.Log("Data sent: " + message);
                }
                catch (Exception e)
                {
                    Debug.LogError("Error sending UDP data: " + e.Message);
                }
            }
        }
    }
    public void OnSendData_Up()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Up"; // ���� �޽���
                byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                // IP �ּ� �� ��Ʈ�� ������ ����
                udpClient.Send(data, data.Length, ipAddress, port);
                Debug.Log("Data sent: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending UDP data: " + e.Message);
            }
        }

    }
    public void OnSendData_Down()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Down"; // ���� �޽���
                byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                // IP �ּ� �� ��Ʈ�� ������ ����
                udpClient.Send(data, data.Length, ipAddress, port);
                Debug.Log("Data sent: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending UDP data: " + e.Message);
            }
        }

    }
    public void OnSendData_Left()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Left"; // ���� �޽���
                byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                // IP �ּ� �� ��Ʈ�� ������ ����
                udpClient.Send(data, data.Length, ipAddress, port);
                Debug.Log("Data sent: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending UDP data: " + e.Message);
            }
        }

    }
    public void OnSendData_Right()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Right"; // ���� �޽���
                byte[] data = Encoding.UTF8.GetBytes(message); // �޽����� ����Ʈ �迭�� ��ȯ

                // IP �ּ� �� ��Ʈ�� ������ ����
                udpClient.Send(data, data.Length, ipAddress, port);
                Debug.Log("Data sent: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("Error sending UDP data: " + e.Message);
            }
        }
    }
    public void OnConvertInput()
    {
        if (inputType == 1)
        {
            inputType = 2;
            Debug.Log("2");
            buttonText.text = "Gesture";
        }
        else if (inputType == 2)
        {
            inputType = 1;
            Debug.Log("1");
            buttonText.text = "UI";
        }
        
        

    }


    private void OnApplicationQuit()
    {
        // UdpClient�� ������ ������ ���ҽ� ������ �߻��� �� ����
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

   
}
