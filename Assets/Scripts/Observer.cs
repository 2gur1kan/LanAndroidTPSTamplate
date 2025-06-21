using Mirror;
using Cinemachine;
using UnityEngine;

public class Observer : NetworkBehaviour
{
    public override void OnStartLocalPlayer()
    {
        CmdRegisterToGameManager(DataBaseManager.Instance.Name);
    }

    [Command]
    void CmdRegisterToGameManager(string playerName)
    {
        GameManager.Instance.RegisterPlayer(connectionToClient, playerName);
    }
}
