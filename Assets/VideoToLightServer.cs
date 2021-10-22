using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class VideoToLightServer: MonoBehaviour
{
    private int w = 0;
    private int h = 0;

    private WebCamTexture webcam = null;
    private WebCamDevice[] devices = null;

    private Color TopLeft = new Color();
    private Color TopRight = new Color();
    private Color BottomLeft = new Color();
    private Color BottomRight = new Color();
    private Color Average = new Color();

    private Socket server = null;
    private const int MTU = 8; //1500;
    private byte[] inbox = new byte[MTU];
    private byte[] outbox = new byte[MTU];
    private int available = 0;

    private Thread worker = null;
    private bool running = true;

    void Start()
    {
        Application.runInBackground = true;
        devices = WebCamTexture.devices;
        Debug.Log(devices[0].name);
        webcam = new WebCamTexture(devices[0].name);
        GetComponent<Renderer>().material.SetTexture("_EmissionMap", webcam);
        GetComponent<Renderer>().material.SetTextureScale("_EmissionMap", new Vector2(1.0f, 1.0f));
        GetComponent<Renderer>().material.SetTextureOffset("_EmissionMap", new Vector2(0.0f, 0.0f));
        webcam.Play();
    }

    void Update()
    {
        w = webcam.width - 1;
        h = webcam.height - 1;
        TopLeft = webcam.GetPixel(0, h);
        TopRight = webcam.GetPixel(w, h);
        BottomLeft = webcam.GetPixel(0, 0);
        BottomRight = webcam.GetPixel(w, 0);
        Average = new Color((TopLeft.r + TopRight.r + BottomLeft.r + BottomRight.r) / 4.0f,
                            (TopLeft.g + TopRight.g + BottomLeft.g + BottomRight.g) / 4.0f,
                            (TopLeft.b + TopRight.b + BottomLeft.b + BottomRight.b) / 4.0f);
        if ((worker == null) || ((worker != null) && (!worker.IsAlive)))
        {
            worker = new Thread(Procedure);
            worker.Start();
        }
    }

    void Procedure()
    {
        while (running)
        {
            try
            {
                Debug.Log("[INFO]: Waiting...");
                EndPoint client = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
                EndPoint nullClient = client;
                for (int i = 0; i < MTU; ++i) { inbox[i] = 0; outbox[i] = 0; }
                available = server.ReceiveFrom(inbox, ref client);
                if ((client != nullClient) && (available > 0))
                {
                    if ((inbox[0] == 'V') && (inbox[1] == 'T') && (inbox[2] == 'O') && (inbox[3] == 'L'))
                    {
                        outbox[0] = (byte)'V'; outbox[1] = (byte)'T'; outbox[2] = (byte)'O'; outbox[3] = (byte)'L';
                        outbox[4] = (byte)(Average.r * 255.0); //Red
                        outbox[5] = (byte)(Average.g * 255.0); //Green
                        outbox[6] = (byte)(Average.b * 255.0); //Blue
                        outbox[7] = (byte)(0); //Reserved
                        server.SendTo(outbox, 8, SocketFlags.None, client);
                    }
                }
            }
            catch (Exception e) { Debug.Log(e); Rebind(); }
        }
    }

    void Rebind(string hostname = "127.0.0.1", int port = 11711)
    {
        IPAddress address = Dns.GetHostEntry(hostname).AddressList[0];
        IPEndPoint ipe = new IPEndPoint(address, port);
        if (server != null) { server.Close(); }
        server = new Socket(ipe.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
        server.ReceiveTimeout = 1000;
        server.SendTimeout = 1000;
        server.Bind(ipe);
    }

    void OnApplicationQuit()
    {
        running = false;
        server.Close();
        worker.Join();
        webcam.Stop();
    }
}
