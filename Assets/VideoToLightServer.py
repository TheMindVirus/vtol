import sys, socket

s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
s.bind(("127.0.0.1", 11711))
print("[INFO]: Waiting...")
while True:
    try:
        msg, conn = s.recvfrom(8)
        print(conn, msg)
        s.sendto(msg, conn)
    except Exception as e:
        print(e, file = sys.stderr)
        s = socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP)
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        s.bind(("127.0.0.1", 11711))
