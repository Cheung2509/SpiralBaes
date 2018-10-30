using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;



public class CharacterSelectionManager : MonoBehaviour
{

    List<PlayerData> players = new List<PlayerData>();

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerData candidate in players)
        {
            if (candidate.player_control_input.GetAxis("MoveHorizontal") > 0.1f || candidate.player_control_input.GetButton("Left"))
            {
                candidate.setCharacter(-1);
            }
            if (candidate.player_control_input.GetAxis("MoveHorizontal") < -0.1f || candidate.player_control_input.GetButton("Right"))
            {
                candidate.setCharacter(1);
            }
            if (candidate.player_control_input.GetButtonDown("Jump"))
            {
                candidate.lockPlayerSelection(players, candidate, true);
            }
        }
    }
}
