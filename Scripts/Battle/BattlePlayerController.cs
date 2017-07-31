using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class BattlePlayerController : MonoBehaviour {

	[HideInInspector]public Player player;

	private BattlePlayerView mBaPlayerView;



	private List<Sprite> skillIcons = new List<Sprite>();

//	private List<Item> consumables = new List<Item> ();

	// 角色UIView
	public BattlePlayerView baView{

		get{
			if (mBaPlayerView == null) {
				mBaPlayerView = GetComponent<BattlePlayerView> ();
			}
			return mBaPlayerView;
		}

	}


	public void SetUpBattlePlayerView(){

//		for(int i = 3;i<player.allEquipedItems.Count;i++){
//			Item consumable = player.allEquipedItems [i];
//			consumables.Add (consumable);
//		}

		List<Sprite> allItemSprites = GameManager.Instance.allItemSprites;

		if (skillIcons.Count != 0) {
			baView.SetUpUI (player,skillIcons,allItemSprites);
			return;
		}

		ResourceManager.Instance.LoadSpritesAssetWithFileName("skills/skills", () => {

			foreach(Sprite s in ResourceManager.Instance.sprites){
				skillIcons.Add(s);
			}
			baView.SetUpUI (player,skillIcons,allItemSprites);
		},true);
			
	}

	public void OnPlayerSelectSkill(int skillIndex){


		baView.SelectedSkillAnim (player.currentSkill == player.attackSkill,
			player.currentSkill == player.defenceSkill,
			skillIndex);

	}

	public void OnPlayerUseItem(int itemIndex){


		Item item = player.allEquipedItems[itemIndex + 3];

		if (item == null) {
			return;
		}

		item.itemCount--;


		if (item.itemCount <= 0) {
			player.allEquipedItems [itemIndex + 3] = null;
			player.allItems.Remove (item);
			baView.SetUpItemButtonsStatus (player);
		}



		if (item.healthGain != 0 && item.strengthGain != 0) {
			player.health += item.healthGain;
			player.strength += item.strengthGain;
			baView.UpdateHealthBarAnim (player);
			baView.UpdateStrengthBarAnim (player);

			string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>" 
				+ "\t\t\t\t\t" 
				+ "<color=orange>+" + item.strengthGain.ToString() + "气力</color>";
			baView.PlayHurtHUDAnim(hurtText);

		}else if (item.healthGain != 0) {
			player.health += item.healthGain;
			baView.UpdateHealthBarAnim (player);
			string hurtText = "<color=green>+" + item.healthGain.ToString() + "体力</color>";
			baView.PlayHurtHUDAnim(hurtText);

		}else if (item.strengthGain != 0) {
			player.strength += item.strengthGain;
			baView.UpdateStrengthBarAnim (player);
			string hurtText = "<color=orange>+" + item.strengthGain.ToString() + "气力</color>";
			baView.PlayHurtHUDAnim(hurtText);
		}

		baView.SetUpItemButtonsStatus (player);

	}

	public void OnSkillLongPress(int index){
		Skill s = player.skillsEquiped [index];
		baView.ShowSkillDetail (index, s);
	}

	public void OnSkillButtonUp(){

		baView.QuitDetailPlane ();
	}

	public void OnItemLongPress(int index){
		Item i = player.allEquipedItems [3 + index];
		baView.ShowItemDetail (index, i);
	}

	public void OnItemButtonUp(){

		baView.QuitDetailPlane ();
	}

	public Skill DefaultSelectedSkill(){

		// 如果气力大于普通攻击所需的气力值，则默认选中普通攻击
		if (player.strength >= player.attackSkill.strengthConsume && player.validActionType != ValidActionType.PhysicalExcption) {
			player.currentSkill = player.attackSkill;
			baView.SelectedSkillAnim (true, false, -1);
			return player.attackSkill;

		}

		// 如果玩家没有被沉默，默认选中可以第一个可以使用的技能
		if (player.validActionType != ValidActionType.MagicException) {
			foreach (Skill s in player.skillsEquiped) {
				if (s.isAvalible && player.strength >= s.strengthConsume) {
					player.currentSkill = s;
					baView.SelectedSkillAnim (false, false, s.skillId);
					return s;
				}
			}
		}


		// 如果其他技能都无法使用，则默认选中防御
		player.currentSkill = player.defenceSkill;
		baView.SelectedSkillAnim(false,true,-1);
		return player.defenceSkill;

	}


	public void SetUpBattleGainsHUD(List<Item> battleGains){

		baView.SetUpBattleGainsHUD (battleGains);

	}

	public void QuitBattleGainsHUD (){
		baView.QuitBattleGainsHUD ();
	}

//	private void ShowSkillDetail(int index){
//
//
//
//	}

//	private IEnumerator ShowItemDetails(){
//
//	}
//	public void OnItemButtonPressed(int index){
//
//
//	}

}
