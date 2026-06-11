using UnityEngine;
using System.IO.Ports;
using System;

public class SerialController : MonoBehaviour
{
    SerialPort serialPort;

    public static int soundLevel;
    public static int lightLevel;

    [SerializeField] string portName = "/dev/cu.usbmodem48CA433FDF242";
    [SerializeField] int baudRate = 57600;

    void Start()
    {
        serialPort = new SerialPort(portName, baudRate);
        serialPort.ReadTimeout = 100;
        serialPort.NewLine = "\n";
        try
        {
            serialPort.Open();
            serialPort.DtrEnable = true;
            serialPort.RtsEnable = true;
        }
        catch (Exception e)
        {
            Debug.Log("Failed to open port: " + e.Message);
        }
    }

    void Update()
    {
        if (serialPort == null || !serialPort.IsOpen) return;
        try
        {
            string data = serialPort.ReadLine().Trim();
            if (data.Length == 0) return;

            string[] values = data.Split(',');

            if (values.Length >= 7)
            {
                int.TryParse(values[3], out soundLevel); // 1kHz 频段
            }
            if (values.Length >= 8)
            {
                int.TryParse(values[7], out lightLevel); // 光线值
            }
        }
        catch (TimeoutException) { }
        catch (Exception e) { Debug.Log("Read Error: " + e.Message); }
    }

    void OnDestroy()
    {
        if (serialPort != null && serialPort.IsOpen)
            serialPort.Close();
    }
}