using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections.Generic;

public class UDPListener : MonoBehaviour
{
    public int listenPort = 8888;
    private UdpClient client;
    private bool listening = true;

    public bool Listening { set => listening = value; }

    private void Start()
    {
        try
        {
            client = new UdpClient(listenPort);
        }
        catch (SocketException e)
        {
            Debug.LogWarning("Port already in use. UDP listener not started. (" + e.Message + ")");
            client = null; // veya alternatif çözüm yolu
        }
    }

    public void startListen() => client.BeginReceive(OnUdpData, new object());

    private void OnUdpData(IAsyncResult result)
    {
        if (!listening) return;

        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, listenPort);
        byte[] data = client.EndReceive(result, ref endPoint);

        string received = Encoding.UTF8.GetString(data);

        if (received.StartsWith("HelloClient::"))
        {
            string ipAddress = received.Split("::")[1];
            Debug.Log("Host bulundu: " + ipAddress);

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                UIManager.Instance.FoundHost(ipAddress);
            });

            return;
        }

        // Tekrar dinle
        client.BeginReceive(OnUdpData, new object());
    }


    private void OnApplicationQuit()
    {
        listening = false;
        client?.Close();
    }
}

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance()
    {
        if (!_instance)
        {
            var obj = new GameObject("MainThreadDispatcher");
            _instance = obj.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(obj);
        }

        return _instance;
    }

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }
}