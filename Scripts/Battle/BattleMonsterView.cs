using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMonsterView : BattleAgentView {

	public Text monsterNameText;

	public void SetUpMonsterView(Monster monster){
		// 加载怪物头像图片
		ResourceManager.Instance.LoadAssetWithFileName ("battle/monster_icons", () => {
			agentIcon.sprite = ResourceManager.Instance.sprites [0];
			monsterNameText.text = monster.agentName;
			agentIcon.enabled = true;

		}, true, monster.agentIconName);

	}



}
