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
    public string ipAddress = "192.168.254.26"; // Limo 노트북의 IP 주소
    public int port = 2020; // Ubuntu에서 수신할 포트 번호
    public PressableButtonHoloLens2 sendButton; // MRTK의 PressableButtonHoloLens2 사용
    public int inputType; //1: UI, 2: Gesutre

    public int gestureNUM;

    
    private TextMeshPro buttonText;

    void Start()
    {
        inputType = 1;
        // UdpClient 초기화
        udpClient = new UdpClient();

        // 버튼 클릭 시 OnSendData 호출
        //sendButton.OnButtonPressed.AddListener(OnSendData);

        // 버튼 내의 TextMeshPro 컴포넌트를 찾아서 참조
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
                    string message = "GO"; // 보낼 메시지
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
            else if(gestureNUM == 2)
            {
                try
                {
                    string message = "STOP"; // 보낼 메시지
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
            else if(gestureNUM == 3)
            {
                try
                {
                    string message = "ROTATE"; // 보낼 메시지
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
        }
    }
    public void OnSendData_Up()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Up"; // 보낼 메시지
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

    }
    public void OnSendData_Down()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Down"; // 보낼 메시지
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

    }
    public void OnSendData_Left()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Left"; // 보낼 메시지
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

    }
    public void OnSendData_Right()
    {
        if (inputType == 1)
        {
            try
            {
                string message = "Right"; // 보낼 메시지
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
        // UdpClient가 닫히지 않으면 리소스 누수가 발생할 수 있음
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }

   
}
