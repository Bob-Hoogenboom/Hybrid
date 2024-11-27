using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;


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
        public float qw, qx, qy, qz;


        // Start is called before the first frame update
        void Start()
        {
            //search for a port with active input from an Arduino
            foreach (string port in ports)
            {
                if (port.Contains("COM")) //string search is inefficient
                {
                    Debug.Log(port);
                    arduinoPort = new SerialPort(port, 115200);
                    arduinoPort.Open();
                }
                else
                {
                    Debug.LogWarning("No Arduino Found");
                }
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (arduinoPort.IsOpen)
            {
                ReadGyroScope();
            }
        }


        private void ReadGyroScope()
        {
            strRecieved = arduinoPort.ReadLine();
            //Debug.Log(strRecieved);
            strData = strRecieved.Split(",");
            if (strData[0] != "" && strData[1] != "" && strData[2] != "" && strData[3] != "") //Makes sure all quaternion data is ready (Values: W,X,Y,Z)
            {
                strDataRecieved[0] = strData[0];
                strDataRecieved[1] = strData[1];
                strDataRecieved[2] = strData[2];
                strDataRecieved[3] = strData[3];

                qw = float.Parse(strDataRecieved[0]);
                qx = float.Parse(strDataRecieved[1]);
                qy = float.Parse(strDataRecieved[2]);
                qz = float.Parse(strDataRecieved[3]);

                transform.rotation = new Quaternion(-qx, -qz, -qy, qw);
            }
        }
    }

}
