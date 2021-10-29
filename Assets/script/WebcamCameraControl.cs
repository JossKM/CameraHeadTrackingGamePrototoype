using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System;

public class WebcamCameraControl : MonoBehaviour
{
    // 1. Declare variables

    private Transform av;     // Transform obj
    private Matrix4x4 baseTransform;     // Calibrated zeros

    Thread receiveThread;     // multiple threads obj
    UdpClient client;         // UDP client
    int port;                 // port of IP


    public float camScale = 0.01f;

    public double ry = 0; //rotate angle on Y axis
    public double rx = 0; //rotate angle on X axis
    public double rz = 0;
    public double pX = 0;
    public double pY = 0;
    public double pZ = 0;
    // 2. InitUDP

    private void InitUDP()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));  // init thread 
        receiveThread.IsBackground = true;                         // set the thread run on background
        receiveThread.Start();                                     // run the thread
    }

    // 3. Receive Data

    private void ReceiveData()
    {
        client = new UdpClient(port);                             // bind port
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);  // create a IPEndPoint, port = 6262  
                byte[] bytes = client.Receive(ref anyIP);                               // receive data from port
                                                                                        //rZ rY rX

                pX = BitConverter.ToDouble(bytes, 0);
                pY = BitConverter.ToDouble(bytes, 8);
                pZ = BitConverter.ToDouble(bytes, 16);
                ry = BitConverter.ToDouble(bytes, 24);
                rx = BitConverter.ToDouble(bytes, 32);
                rz = BitConverter.ToDouble(bytes, 40);
            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
    }

    // 4. Init variables

    void Start()
    {
        av = gameObject.GetComponent<Transform>();                                      // get Transform obj
        baseTransform = av.localToWorldMatrix;
        port = 6262;                                                                    // set port
        ry = 0;                                                                         // init rotate angle on Y axis 
        rx = 0;                                                                         // init rotate angle on X axis 
        rz = 0;
        InitUDP();                                                                      // init UDP
    }

    // 5. Update position of avatar

    void Update()
    {
        Quaternion rot = baseTransform.rotation * Quaternion.Euler((float)rx, (float)ry, (float)rz);   // control avatar with rotation angle on X,Y axis
        av.rotation = Quaternion.Lerp(av.rotation, rot, 0.05f);
        av.position = baseTransform.MultiplyVector(new Vector3((float)pX * camScale, (float)pY * camScale, (float)pZ * camScale));
    }
}
