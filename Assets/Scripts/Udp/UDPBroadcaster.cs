using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPBroadcaster : MonoBehaviour
{
    UdpClient udpClient;
    IPEndPoint endPoint;

    public string broadcastMessage = "HelloClient";
    public int broadcastPort = 8888;

    void Start()
    {
        udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        endPoint = new IPEndPoint(IPAddress.Broadcast, broadcastPort);
    }

    public void StartBoardCaster() => StartCoroutine(caster());

    private IEnumerator caster()
    {
        GetComponent<UDPListener>().Listening = false;

        yield return new WaitForSeconds(1f);

        while (true)
        {
            Broadcast();
            yield return new WaitForSeconds(3f);
        }
    }

    private void Broadcast()
    {
        string message = $"HelloClient::{GetLocalIPAddress()}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        udpClient.Send(data, data.Length, endPoint);
    }

    public static IPAddress GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress fallback = null;

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                string ipStr = ip.ToString();
                if (ipStr.StartsWith("192.168"))
                {
                    return ip; // Öncelik: 192.168.x.x
                }

                // Eðer 192.168 bulunamazsa diðer IPv4'ü yedek olarak tut
                fallback ??= ip;
            }
        }

        if (fallback != null)
            return fallback;

        throw new Exception("IPv4 adresi bulunamadý!");
    }

    void OnApplicationQuit()
    {
        udpClient.Close();
    }
}
