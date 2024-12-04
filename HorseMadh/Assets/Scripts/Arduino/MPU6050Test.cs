using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;


namespace Arduino
{
    public class MPU6050Test : MonoBehaviour
    {
        public string[] ports = SerialPort.GetPortNames();
        public SerialPort arduinoPort; //Check for port*
                                       //= new SerialPort("COM5", 115200);

        public string strRecieved;
        public string[] strData = new string[4];
        public string[] strDataRecieved = new string[4];

        [Header("QuaternionData")]
        public float qw, qx, qy, qz;
        private Quaternion initialRotation = Quaternion.identity;

        [Header("ButtonData")]
        private bool isCalibrated = false;


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
                strRecieved = arduinoPort.ReadLine();
                Debug.Log($"Raw data received: {strRecieved}");

                strData = strRecieved.Split(",");

                if (strData.Length >= 5) // Ensure there are at least 5 values (4 for quaternion, 1 for button state)
                {
                    // Parse the first four quaternion values
                    if (float.TryParse(strData[0], out qw) &&
                        float.TryParse(strData[1], out qx) &&
                        float.TryParse(strData[2], out qy) &&
                        float.TryParse(strData[3], out qz))
                    {
                        Quaternion currentGyroRotation = new Quaternion(-qx, -qz, -qy, qw);

                        if (!isCalibrated)
                        {
                            // If not calibrated, use the current rotation as the initial reference
                            initialRotation = currentGyroRotation;
                            isCalibrated = true;
                        }

                        // Apply the recalibrated rotation
                        transform.rotation = Quaternion.Inverse(initialRotation) * currentGyroRotation;
                    }
                    else
                    {
                        Debug.LogWarning("Invalid quaternion data format in the first 4 values.");
                    }

                    // Handle button press (5th value)
                    if (strData[4].Trim() == "0") // "0" is for when the button is pressed this is for the internal pull-up resistor
                    {
                        ResetRotation();
                    }
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

        private void ResetRotation()
        {
            Debug.Log("Button pressed: Resetting rotation and recalibrating gyroscope.");
            // Reset the object's rotation
            transform.rotation = Quaternion.identity;

            // Recalibrate by resetting the initial rotation
            isCalibrated = false;
        }
    }

}
