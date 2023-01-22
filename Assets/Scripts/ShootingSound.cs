using UnityEngine;
using Mirror;

public class ShootingSound : NetworkBehaviour
{
    [SerializeField] private AudioSource shootingAudio;

    [Command(requiresAuthority = false)]
    public void CmdShootSound()
    {
        RpcShootSound();
    }
    
    [ClientRpc]
    public void RpcShootSound()
    {
        if (!isLocalPlayer)
            shootingAudio.Play();
    }
}