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

    public float lerpSpeed = 0.1f;

    public float camScale = 0.01f;

    public float pX = 0;
    public float pY = 0;
    public float pZ = 0;
    public float ry = 0; //rotate angle on Y axis
    public float rx = 0; //rotate angle on X axis
    public float rz = 0;

    public float pXOffset = 0;
    public float pYOffset = 0;
    public float pZOffset = 0;
    public float ryOffset = 0;
    public float rxOffset = 0;
    public float rzOffset = 0;

    public Vector3 smoothedPosition;

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
        //client.Connect();
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);  // create a IPEndPoint, port = 6262  
                byte[] bytes = client.Receive(ref anyIP);                               // receive data from port
                                                                                        //rZ rY rX

                //pX = BitConverter.ToDouble(bytes, 0);
                //pY = BitConverter.ToDouble(bytes, 8);
                //pZ = BitConverter.ToDouble(bytes, 16);
                ry = (float)BitConverter.ToDouble(bytes, 24);
                rx = (float)BitConverter.ToDouble(bytes, 32);
                //rz = (float)BitConverter.ToDouble(bytes, 40);

                ry = 360.0f + -(float)BitConverter.ToDouble(bytes, 24);
                rx = 360.0f + (float)BitConverter.ToDouble(bytes, 32);
                rz = 0;//(float)BitConverter.ToDouble(bytes, 16);
                ry = Mathf.Repeat(ry, 360.0f);
                rx = Mathf.Repeat(rx, 360.0f);
                rz = Mathf.Repeat(rz, 360.0f);

                pX = (float)BitConverter.ToDouble(bytes, 0);
                pY = (float)BitConverter.ToDouble(bytes, 8);
                pZ = (float)BitConverter.ToDouble(bytes, 16);

                // Apply offsets
                pX += pXOffset;
                pY += pYOffset;
                pZ += pZOffset;
                //ry += ryOffset;
                //rx += rxOffset;
                //rz += rzOffset;
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
       // baseTransform = av.localToWorldMatrix;
        port = 4242;                                                                    // set port
        ry = 0;                                                                         // init rotate angle on Y axis 
        rx = 0;                                                                         // init rotate angle on X axis 
        rz = 0;
        pY = 0;
        pX = 0;
        pZ = 0;

        InitUDP();                                                                      // init UDP
    }

    void OnDestroy()
    {
        if(receiveThread != null)
        receiveThread.Abort();

        if(client != null)
        client.Close();
    }

    void SetZeroes()
    {
        pXOffset = -pX;
        pYOffset = -pY;
        pZOffset = -pZ;
        ryOffset = -ry;
        rxOffset = -rx;
        rzOffset = -rz;
    }

    // 5. Update position of avatar
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetZeroes();
        }

        smoothedPosition = Vector3.Lerp(smoothedPosition, new Vector3((float)pX * camScale, (float)pY * camScale, (float)pZ * camScale), lerpSpeed);
      
       Quaternion rot = Quaternion.Euler((float)rx, (float)ry, (float)rz);   // control avatar with rotation angle on X,Y axis
       av.localRotation = Quaternion.Lerp(av.localRotation, rot, lerpSpeed);
      
       av.localPosition = smoothedPosition;
    }
}
