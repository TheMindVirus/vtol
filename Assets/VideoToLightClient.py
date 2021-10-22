import sys, socket, time

s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
s.settimeout(1.0)

while True:
    try:
        s.sendto("VTOL\x00\x01\x02\x03".encode(), ("127.0.0.1", 11711))
        print("[INFO]: Sent")
        msg, conn = s.recvfrom(1500)
        print("[INFO]: Received")
        print(conn, msg)
        labels = ["red", "green", "blue"]
        if (len(msg) >= 8):
            data = list(msg)[4:7]
            rgb = dict.fromkeys(labels)
            for i in range(0, len(labels)):
                rgb[labels[i]] = data[i]
                #rgb[list(rgb.keys())[i]] = data[i]
            print(rgb)
    except Exception as e:
        print(e, file = sys.stderr)
    time.sleep(1)
