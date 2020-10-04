using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Disable, don't destroy. spawn particles when enabled/disabled. Set new playerstate when disabled.

public class Note : MonoBehaviour
{
    [SerializeField]
    private PlayerState playerStateToEnter = PlayerState.None;
    
    [SerializeField] //Should be paired to the player color. H
    private List<Material> noteMaterials = null;

    [SerializeField]
    private List<AudioClip> audioclip;

    public PlayerState GetPlayerStateToEnter()
    {
        return playerStateToEnter;
    }

    public void SetPlayerStateRequiredToEnter(PlayerState ps)
    {
        playerStateToEnter = ps;
        SetStateFromPS(ps);
    }

    private void SetStateFromPS(PlayerState ps)
    {
        switch (ps)
        {
            case PlayerState.Q:
                GetComponent<Renderer>().material = noteMaterials[0];
                name = "NoteQ";
                break;
            case PlayerState.W:
                GetComponent<Renderer>().material = noteMaterials[1];
                name = "NoteW";
                break;
            case PlayerState.E:
                GetComponent<Renderer>().material = noteMaterials[2];
                name = "NoteE";
                break;
            case PlayerState.R:
                GetComponent<Renderer>().material = noteMaterials[3];
                name = "NoteR";
                break;
            default:
                break;
        }
    }

    private void OnDestroy()
    {
        
    }

}
