using System;
using System.IO.Ports;
using UnityEngine;

namespace Arduino
{
    public class ArduinoInput : MonoBehaviour
    {
        public string[] ports = SerialPort.GetPortNames();
        public SerialPort arduinoPort; //Check for port*
                                        //= new SerialPort("COM5", 115200);

        private string _strRecieved;
        private string[] _strData = new string[4];

        [Header("QuaternionData")]
        private float qw, qx, qy, qz;

        //[Header("Getter/Setter")]
        public Quaternion gyroscopeRotation { get; private set; }
        public bool calibratePressed { get; private set; }


        private void Start()
        {
            //search for a port with active input from an Arduino
            foreach (string port in ports)
            {
                try
                {
                    arduinoPort = new SerialPort(port, 115200)
                    {
                        ReadTimeout = 100
                    };
                    arduinoPort.Open();
                    Debug.Log($"Opened port {port}");
                    break; // Exit the loop once a port is successfully opened
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"Could not open port {port}: {ex.Message}");
                }
            }
        }


        // Update is called once per frame
        private void Update()
        {
            if (arduinoPort.IsOpen)
            {
                ReadArduinoData();
            }
        }


        private void ReadArduinoData()
        {
            try
            {
                _strRecieved = arduinoPort.ReadLine();
                Debug.Log($"Raw data received: {_strRecieved}");

                _strData = _strRecieved.Split(",");

                if (_strData.Length >= 5) // Ensure there are at least 5 values (4 for quaternion, 1 for button state)
                {
                    // Parse the first four quaternion values
                    if (float.TryParse(_strData[0], out qw) &&
                        float.TryParse(_strData[1], out qx) &&
                        float.TryParse(_strData[2], out qy) &&
                        float.TryParse(_strData[3], out qz))
                    {
                        Quaternion currentGyroRotation = new Quaternion(-qx, -qz, -qy, qw);
                        gyroscopeRotation = currentGyroRotation;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid quaternion data format in the first 4 values.");
                    }

                    // Handle button press (5th value) via a Ternary Operator 
                    calibratePressed = _strData[4].Trim() == "0" ? true : false;// "0" is for when the button is pressed this is for the internal pull-up resistor


                }
                else
                {
                    Debug.LogWarning("Insufficient data received. Expected at least 5 values.");
                }
            }
            catch (TimeoutException)
            {
                Debug.LogWarning("ReadLine timed out.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error reading data: {ex.Message}");
            }
        }
    }
}
