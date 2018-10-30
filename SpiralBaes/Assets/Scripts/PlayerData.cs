using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerData : MonoBehaviour
{
    static int new_id = 1;
    int id;
    public Player player_control_input;
    CharacterData.CHARACTER_LIST character;
    CharacterData character_information;
    bool locked_in = false;

    [SerializeField] List<GameObject> character_image = new List<GameObject>();

    public PlayerData()
    {
        player_control_input = ReInput.players.GetPlayer(new_id);
        id = new_id;
        new_id++;
    }

    public void setLockIn(bool value)
    {
        locked_in = value;
    }

    #region Character Handeling
    public void setCharacter(int i)
    {
        if ((character + i) > 0)
        {
            character = CharacterData.CHARACTER_LIST.NUMOFTYPES - 1;
        }
        else if ((character + i) < CharacterData.CHARACTER_LIST.NUMOFTYPES)
        {
            character = 0;
        }
        else
        {
            character = character + i;
        }

        character_image[id].GetComponent<SpriteRenderer>().sprite = character_information.character_list[(int)character].player_selection_icon;

    }

    public CharacterData.CHARACTER_LIST getCharacter()
    {
        return character;
    }

    #endregion

    public bool lockPlayerSelection(List<PlayerData> player_list, PlayerData current_player, bool value)
    {
        switch (value)
        {
            case true:
                {
                    foreach (PlayerData candidate in player_list)
                    {
                        if (current_player.getCharacter() == candidate.getCharacter())
                        {
                            return false;
                        }
                    }

                    current_player.setLockIn(true);
                    return true;
                }
            case false:
                {
                    current_player.setLockIn(true);
                    return true;
                }
        }

        return false;
    }
}
