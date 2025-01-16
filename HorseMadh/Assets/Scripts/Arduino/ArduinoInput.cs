using System;
using System.Collections;
using System.IO.Ports;
using UnityEngine;

namespace Arduino
{
    public class ArduinoInput : ControlInterfaceClass
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
        public bool actionPressed { get; private set; }

        public override PlayerVariables ControlVariables => new PlayerVariables(gyroscopeRotation, calibratePressed, actionPressed);


        private void Start()
        {
            ports = SerialPort.GetPortNames();
           // StartCoroutine(TryConnectToArduino());
        }

        private IEnumerator TryConnectToArduino()
        {
            while (arduinoPort == null || !arduinoPort.IsOpen)
            {
                foreach (string port in ports)
                {
                    try
                    {
                        arduinoPort = new SerialPort(port, 115200)
                        {
                            ReadTimeout = 100
                        };
                        arduinoPort.Open();
                        Debug.Log($"Successfully connected to port {port}");
                        yield break; // Exit the coroutine once connected
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"Failed to connect to port {port}: {ex.Message}");
                    }
                }

                Debug.Log("Retrying to connect in 2 seconds...");
                yield return new WaitForSeconds(2); // Wait for 2 seconds before retrying
            }
        }

        private void OnDestroy()
        {
            // Ensure the port is closed when the object is destroyed
            if (arduinoPort != null && arduinoPort.IsOpen)
            {
                arduinoPort.Close();
                Debug.Log("Port closed.");
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
                //Debug.Log($"Raw data received: {_strRecieved}");

                _strData = _strRecieved.Split(",");

                if (_strData.Length >= 6) // Ensure there are at least 5 values (4 for quaternion, 1 for button state)
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
                    // "0" is for when the button is pressed this is for the internal pull-up resistor
                    calibratePressed = _strData[4].Trim() == "0" ? true : false;

                    actionPressed = _strData[5].Trim() == "0" ? true : false;
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

