using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class RPC : NetworkBehaviour
{
    [Rpc (RpcSources.All, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
    public void RPC_RestartReady(RpcInfo info = default)
    {
        if(NetworkController.Instance._runner.LocalPlayer.PlayerId == info.Source.PlayerId) GameProcess.Instance._restartButton.GetComponent<Button>().interactable = false;
        NetworkController.Instance._restartReady++;
        
        if (NetworkController.Instance._restartReady == 2)
        {
            GameProcess.Instance.Restart();
            GameProcess.Instance._restartButton.GetComponent<Button>().interactable = true;
            GameProcess.Instance._restartButton.SetActive(false);
            NetworkController.Instance._restartReady = 0;
        }
        GameProcess.Instance._restartText.text = $"{NetworkController.Instance._restartReady}/2 READY";
    }
}
