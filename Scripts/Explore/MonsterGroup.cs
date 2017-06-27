using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MonsterGroup{

	public List<BattleAgent> monsters = new List<BattleAgent>();

	public string monsterGroupName;

	public string monsterGroupDescription;

	public string spriteName;

	public override string ToString ()
	{
		return string.Format ("[MonsterGroup]:" + monsterGroupName + "[\nmonsterGroupDesc:]" + monsterGroupDescription);
	}

}
