using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

using System.IO;



public class avatar_girl : MonoBehaviour
{
    // 1. Declare variables

    private Transform av;     // Transform obj
    Thread receiveThread;     // multiple threads obj
    UdpClient client;         // UDP client
    int port;                 // port of IP


    public static int ty = 0; //translate on Y axis
    public static int tx = 0; //translate on X axis

    public static int ry = 0; //rotate angle on Y axis
    public static int rx = 0; //rotate angle on X axis

    public float move_speed = 0.05f;

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

                //Translate value
                double num0 = BitConverter.ToDouble(bytes, 0);                         // get rotate angle on Y axis
                double num1 = BitConverter.ToDouble(bytes, 8);                         // get rotate angle on X axis
                // Rotate value
                double num3 = BitConverter.ToDouble(bytes, 24);                         // get rotate angle on Y axis
                double num4 = BitConverter.ToDouble(bytes, 32);                         // get rotate angle on X axis
                ry = (int)num3;                                                         // convert to integer
                rx = (int)num4;

                tx = (int)num0;
                ty = (int)num1;
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
        port = 6262;                                                                    // set port
        tx = 0;
        ty = 0;
        ry = 0;                                                                         // init rotate angle on Y axis 
        rx = 0;                                                                         // init rotate angle on X axis 

        move_speed = 0.05f;

        InitUDP();                                                                      // init UDP
    }

    // 5. Update position of avatar

    void Update()
    {
        Vector3 trans_pos = new Vector3(tx * 0.01f, 0, -ty * 0.1f);

        av.rotation = Quaternion.Lerp(av.rotation, Quaternion.Euler(rx, -ry, 0), move_speed);   // control avatar with rotation angle on X,Y axis
        av.localPosition = Vector3.Lerp(av.position, trans_pos, move_speed);                    // move avatar 
    }
}