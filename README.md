# vtol
Video To Light Server made with Unity C# .NET Sockets, interoperable with Python/Unix/C Sockets

Demo: https://themindvirus.github.io/vtol/

![screenshot](https://github.com/themindvirus/vtol/blob/main/screenshot.png)

```
VToL now not only stands for Vertical Take-Off and Landing for Harrier Jump-Jets,
but it now also stands for Video-To-Light: A technology converting live video streams
into single pixel colour data to be sent directly to various lighting equipment.

This implementation uses the Unity 2021.1.13f1 (64-bit) WebCamTexture class to
pull in streams from not only webcams but also HDMI capture cards for consoles
and other devices. You can also use it to capture and decode a secondary display.

The decoding is done by first streaming the input device onto a texture in memory.
4x RGB pixel colours are extracted, one from each corner of the capture rectangle.
The average (mean) pixel colour is calculated as soon as possible in the main thread.
It cannot run in the background network thread, Unity will simply not allow it.

Separate from that on the network thread, the average pixel colours are scaled and
converted to bytes in an encoding that starts with "VTOL" and can be reconfigured.
The in-app server uses User Datagram Protocol to transfer the data to localhost:11711.
This only occurs when another app on the same PC and same port sends "VTOL" to the server.

User Datagram Protocol is connectionless for the client and needs binding in the server app.
To expand it beyond the host PC and onto the network, Python can forward the packet
to IoT bridge software such as Node-RED. Alternatively, "127.0.0.1" could be changed
for a more direct connection, but this would have a mild impact on capture performance.

Please feel free to customise the Unity Scripts to suit your application
and extend it to whichever proprietary or non-proprietary equipment you may have.
ToDo: Amplify the colour values based on amplitude of HDMI-encoded audio samples...
```
