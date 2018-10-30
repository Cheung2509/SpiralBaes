using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterData : MonoBehaviour {

    public enum CHARACTER_LIST
    {
        BLUE,
        GREEN,
        RED,
        YELLOW,
        NUMOFTYPES
    }

    public List<CharacterInfo> character_list = new List<CharacterInfo>();

    #region Character Data
    [SerializeField]List<Sprite> character_selection_icon = new List<Sprite>();
    #endregion

    private void Start()
    {
        int index = 0;

        foreach (string candidate_name in Enum.GetNames(typeof(CHARACTER_LIST)))
        {
            CharacterInfo candidate = new CharacterInfo();

            candidate.name = candidate_name;
            candidate.player_selection_icon = character_selection_icon[index];

            character_list.Add(candidate);

            index++;
        }

        
    }
}

public class CharacterInfo
{
    public string name;
    public Sprite player_selection_icon;
}
