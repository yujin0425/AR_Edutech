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
    public string ipAddress = "192.168.0.115"; // Limo ��Ʈ���� IP �ּ�
    public int port = 2020; // Ubuntu���� ������ ��Ʈ ��ȣ
    public PressableButtonHoloLens2 sendButton; // MRTK�� PressableButtonHoloLens2 ���

    public int gestureNUM;

    private TextMeshPro buttonText;

    void Start()
    {
        // UdpClient �ʱ�ȭ
        udpClient = new UdpClient();

        // ��ư ���� TextMeshPro ������Ʈ�� ã�Ƽ� ����
        buttonText = sendButton.GetComponentInChildren<TextMeshPro>();
    }

    void Update()
    {
        Debug.Log("NUM:" + gestureNUM);
    }

    public void OnSendData_Up()
    {
        SendUdpMessage("section1"); // �κ��� section1�� �̵�
    }

    public void OnSendData_Down()
    {
        SendUdpMessage("section3"); // �κ��� section3�� �̵�
    }

    public void OnSendData_Right()
    {
        SendUdpMessage("section2"); // �κ��� section2�� �̵�
    }

    private void SendUdpMessage(string message)
    {
        try
        {
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

    private void OnApplicationQuit()
    {
        // UdpClient�� ������ ������ ���ҽ� ������ �߻��� �� ����
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
