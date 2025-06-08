using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_InputField NameInput;

    [SerializeField] private GameObject OptionsPanel;

    private string hostIp = "192.168.1.100";

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        SetOptionPanel(false);
    }

    public void OnJoinOrHostClicked()
    {
        if (!NetworkClient.isConnected)
        {
            NetworkManager.singleton.networkAddress = hostIp;
            NetworkManager.singleton.StartClient();
            Debug.Log("Client bağlanmayı deniyor...");

            StartCoroutine(TryConnectThenHost());
        }
    }

    private IEnumerator TryConnectThenHost()
    {
        float timeout = 1f;
        float timer = 0f;

        while (!NetworkClient.isConnected && timer < timeout)
        {
            timer += .5f;
            yield return new WaitForSeconds(.5f);
        }

        if (!NetworkClient.isConnected)
        {
            Debug.LogWarning("Client bağlantısı başarısız. Host başlatılıyor...");
            NetworkManager.singleton.StopClient();
        }

        SetHostGame();
    }

    public void SetHostGame()
    {
        if (NetworkClient.isConnected) return;

        SetName();

        NetworkManager.singleton.networkAddress = hostIp;

        NetworkManager.singleton.StartHost();

        NetworkManager.singleton.ServerChangeScene("SampleScene");

        hostIp = GetLocalIPAddress();
        Debug.Log("Host IP: " + hostIp);
    }

    private string GetLocalIPAddress()
    {
        var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
        string fallbackIP = "127.0.0.1"; 

        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                string ipStr = ip.ToString();
                if (ipStr.StartsWith("192.168"))
                {
                    return ipStr;
                }
                else
                {
                    fallbackIP = ipStr;
                }
            }
        }

        return fallbackIP;
    }

    public void FoundHost(string ip)
    {
        hostIp = ip;
        Debug.Log("Otomatik bulunan host IP: " + hostIp);

        SetName();

        // Otomatik bağlan
        if (!NetworkClient.isConnected)
        {
            NetworkManager.singleton.networkAddress = hostIp;
            NetworkManager.singleton.StartClient();
            Debug.Log("Client otomatik bağlanmayı deniyor...");
        }
    }

    private void SetName()
    {
        if (NameInput == null) return;

        if (string.IsNullOrWhiteSpace(NameInput.text)) return;

        DataBaseManager.Instance.Name = NameInput.text;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetOptionPanel(bool flag)
    {
        if (OptionsPanel == null) return;

        OptionsPanel.SetActive(flag);
    }
}
