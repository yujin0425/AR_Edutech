using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI; // MRTK의 버튼 관련 네임스페이스
using TMPro; // TextMeshPro 사용

public class UdpSender : MonoBehaviour
{
    private UdpClient udpClient;
    public string ipAddress = "192.168.0.115"; // Limo 노트북의 IP 주소
    public int port = 2020; // Ubuntu에서 수신할 포트 번호
    public PressableButtonHoloLens2 sendButton; // MRTK의 PressableButtonHoloLens2 사용

    public int gestureNUM;

    private TextMeshPro buttonText;

    void Start()
    {
        // UdpClient 초기화
        udpClient = new UdpClient();

        // 버튼 내의 TextMeshPro 컴포넌트를 찾아서 참조
        buttonText = sendButton.GetComponentInChildren<TextMeshPro>();
    }

    void Update()
    {
        Debug.Log("NUM:" + gestureNUM);
    }

    public void OnSendData_Up()
    {
        SendUdpMessage("section1"); // 로봇이 section1로 이동
    }

    public void OnSendData_Down()
    {
        SendUdpMessage("section3"); // 로봇이 section3로 이동
    }

    public void OnSendData_Right()
    {
        SendUdpMessage("section2"); // 로봇이 section2로 이동
    }

    private void SendUdpMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message); // 메시지를 바이트 배열로 변환

            // IP 주소 및 포트로 데이터 전송
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
        // UdpClient가 닫히지 않으면 리소스 누수가 발생할 수 있음
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
}
