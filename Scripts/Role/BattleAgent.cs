using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public abstract class BattleAgent : MonoBehaviour {

	public string agentName;
	public string agentIconName;
	public bool isActive;

	private Skill mAttackSkill;
	public Skill attackSkill{
		get{
			if (mAttackSkill == null) {
				ResourceManager.Instance.LoadAssetWithFileName ("skills/skills", () => {
					mAttackSkill = ResourceManager.Instance.gos[0].GetComponent<Skill>();
				}, true,"Attack");
			}
			mAttackSkill.transform.SetParent (transform.FindChild ("Skills"), false);
			return mAttackSkill;
		}
		set{
			mAttackSkill = value;
		}

	}

	private Skill mDefenceSkill;
	public Skill defenceSkill{

		get{
			if (mDefenceSkill == null) {
				ResourceManager.Instance.LoadAssetWithFileName ("skills/skills", () => {
					mDefenceSkill = ResourceManager.Instance.gos[0].GetComponent<Skill>();
				}, true,"Defence");
			}
			mDefenceSkill.transform.SetParent (transform.FindChild ("Skills"), false);
			return mDefenceSkill;
		}
		set{
			mDefenceSkill = value;
		}

	}

	public int agentLevel;


	public bool isAttackEnable = true;
	public bool isSkillEnable = true;
	public bool isItemEnable = true;
	public bool isDefenceEnable = true;

	//*****初始信息********//
	public int originalMaxHealth;
	public int originalMaxStrength;
	public int originalHealth;
	public int originalStrength;
	public int originalAttack;
	public int originalPower;
	public int originalMagic;
	public int originalCrit;
	public int originalAgility;
	public int originalAmour;
	public int originalMagicResist;
	//*****初始信息********//

	public int maxHealth;//最大血量
	public int maxStrength;//最大气力值

	private BattleAgentView mBaView;

	// 角色UIView
	public BattleAgentView baView{
		get{
			if (mBaView == null) {

				if (GetType () == typeof(Player)) {
					mBaView = GetComponent<BattlePlayerView> ();
				} else {
					mBaView = GetComponent<BattleMonsterView> ();
				}
			}
			return mBaView;
		}

	}
		
	[SerializeField]private int mHealth;//实际血量
	public int health{
		get{return mHealth;}
		set{
			if (value < mHealth && baView != null) {
				baView.PlayShakeAnim ();
			}
			if (value <= maxHealth) {
				mHealth = value;
			} else {
				mHealth = maxHealth;
			}
			if (baView != null) {
				baView.UpdateHealthBarAnim (this);
			}
		}

	}
	[SerializeField]private int mStrength;//实际气力值
	public int strength{
		get{ return mStrength; }
		set{ 
			if (value <= maxStrength) {
				mStrength = value;
			} else {
				mStrength = maxStrength;
			}
			if (baView != null) {
				baView.UpdateStrengthBarAnim (this);
			}
		}
	}

	public int attack;//攻击力
	public int power;//力量
	public int magic;//魔法
	public int agility;//敏捷
	public int amour;//护甲
	public int magicResist;//魔抗
	public int crit;//暴击


	public int healthGainScaler;//力量对最大血量的加成系数

	public float strengthGainScaler; //力量对最大气力的加成系数

	public ValidActionType validActionType = ValidActionType.All;// 有效的行动类型

	public float hurtScaler;//伤害系数

	public float critScaler;//暴击伤害系数

	public float healthAbsorbScalser;//回血比例

	public List<Skill> skillsEquiped = new List<Skill>();//技能数组

	private List<Item> mAllEquipedItems = new List<Item>();

	public List<Item> allEquipedItems{
		get{
			if (mAllEquipedItems.Count == 0) {
				for (int i = 0; i < 6; i++) {
					mAllEquipedItems.Add (null);
				}
			}
			return mAllEquipedItems;
		}
		set{
			mAllEquipedItems = value;
		}
	}

	public List<Item> allItems = new List<Item> (); // 所有物品

	public List<StateSkillEffect> states = new List<StateSkillEffect>();//状态数组

	public int attackTime;//攻击次数

	public Skill currentSkill;

	public virtual void Awake(){

		isActive = true; // 角色初始化后默认可以行动

		healthGainScaler = 1;//力量对最大血量的加成系数

		strengthGainScaler = 0.05f; //力量对最大气力的加成系数

		validActionType = ValidActionType.All;// 有效的行动类型

		hurtScaler = 1.0f;//伤害系数

		critScaler = 1.0f;//暴击伤害系数

		healthAbsorbScalser = 0f;//回血比例

		attackTime = 1;//攻击次数

	}


	public void CopyAgentStatus(BattleAgent ba){

		this.originalMaxHealth = ba.originalMaxHealth;
		this.originalMaxStrength = ba.originalMaxStrength;
		this.originalHealth = ba.originalHealth;
		this.originalStrength = ba.originalStrength;
		this.originalAttack = ba.originalAttack;
		this.originalPower = ba.originalPower;
		this.originalMagic = ba.originalMagic;
		this.originalCrit = ba.originalCrit;
		this.originalAgility = ba.originalAgility;
		this.originalAmour = ba.originalAmour;
		this.originalMagicResist = ba.originalMagicResist;

		this.maxHealth = ba.maxHealth;
		this.maxStrength = ba.maxStrength;

		this.health = ba.health;
		this.strength = ba.strength;

		this.attack = ba.attack;//攻击力
		this.power = ba.power;//力量
		this.magic = ba.magic;//魔法
		this.agility = ba.agility;//敏捷
		this.amour = ba.amour;//护甲
		this.magicResist = ba.magicResist;//魔抗
		this.crit = ba.crit;//暴击


		this.attackSkill = ba.attackSkill;
		this.defenceSkill = ba.defenceSkill;

		this.skillsEquiped = ba.skillsEquiped;

		this.allEquipedItems = ba.allEquipedItems;

		this.allItems = ba.allItems;

		this.skillsEquiped = ba.skillsEquiped;

		foreach (StateSkillEffect state in ba.states) {
			state.transform.SetParent (this.transform.FindChild ("States").transform);
		}
		ba.states.Clear ();
		this.states = ba.states;



		this.isActive = ba.isActive;

	}

	public void CopyAgentStatus(BattleAgentModel ba){

		this.agentIconName = ba.agentIconName;

		this.originalMaxHealth = ba.originalMaxHealth;
		this.originalMaxStrength = ba.originalMaxStrength;
		this.originalHealth = ba.originalHealth;
		this.originalStrength = ba.originalStrength;
		this.originalAttack = ba.originalAttack;
		this.originalPower = ba.originalPower;
		this.originalMagic = ba.originalMagic;
		this.originalCrit = ba.originalCrit;
		this.originalAgility = ba.originalAgility;
		this.originalAmour = ba.originalAmour;
		this.originalMagicResist = ba.originalMagicResist;

		this.maxHealth = ba.maxHealth;
		this.maxStrength = ba.maxStrength;
		this.health = ba.health;

		this.strength = ba.strength;


		this.attack = ba.attack;//攻击力
		this.power = ba.power;//力量
		this.magic = ba.magic;//魔法
		this.agility = ba.agility;//敏捷
		this.amour = ba.amour;//护甲
		this.magicResist = ba.magicResist;//魔抗
		this.crit = ba.crit;//暴击

		this.isActive = ba.isActive;

	}

	//添加状态 
	public void AddState(StateSkillEffect sse){
		states.Add (sse);
		ResetBattleAgentProperties (false,false);
	}
	//删除状态
	public void RemoveState(StateSkillEffect sse){
		for(int i = 0;i<states.Count;i++){
			if (sse.effectName == states[i].effectName) {
				states.RemoveAt(i);
				Destroy (sse);
				ResetBattleAgentProperties (false,false);
				return;
			}
		}
	}

	// 状态效果触发执行的方法
	public void OnTrigger(List<BattleAgent> friends,BattleAgent triggerAgent,List<BattleAgent> enemies, TriggerType triggerType,int arg){
			
		foreach(StateSkillEffect sse in states){
			sse.AffectAgents (this,friends,triggerAgent,enemies, sse.skillLevel, triggerType, arg);
		}

	}

	private void ResetPropertiesByEquipment(Item equipment){

		if (equipment.itemName == null) {
			return;
		}

		attack += equipment.attackGain;
//		power += equipment.powerGain;
		magic += equipment.magicGain;
		crit += equipment.critGain;
		amour += equipment.amourGain;
		magicResist += equipment.magicResistGain;
		agility += equipment.agilityGain;

	}

	// 仅根据物品重新计人物的属性，其余属性重置为初始状态
	public void ResetBattleAgentProperties (bool toOriginalState,bool firstEnterBattleOrQuitBattle)
	{
		// 所有属性重置为初始值
		attack = originalAttack;
		power = originalPower;
		magic = originalMagic;
		crit = originalCrit;
		amour = originalAmour;
		magicResist = originalMagicResist;
		agility = originalAgility;

		// 根据装备更新属性

		foreach (Item item in allEquipedItems) {
			if (item != null && item.itemType != ItemType.Consumables) {
				ResetPropertiesByEquipment (item);
			}
		}

		maxHealth = originalMaxHealth + healthGainScaler * power;
		maxStrength = originalMaxStrength + (int)(strengthGainScaler * power);

		hurtScaler = 1.0f;//伤害系数
		critScaler = 1.0f;//暴击伤害系数
		healthAbsorbScalser = 0f;//吸血比例
		attackTime = 1;

		if (toOriginalState) {
			validActionType = ValidActionType.All;
			health = maxHealth;
			strength = maxStrength;
			// 开启血量槽和气力槽的设置动画
			if (baView != null) {
				baView.firstSetHealthBar = false;
				baView.firstSetStrengthBar = false;
			}
			foreach (Skill s in skillsEquiped) {
				s.isAvalible = true;
				s.actionCount = 0;
				foreach (BaseSkillEffect bse in s.skillEffects) {
					bse.actionCount = 0;
				}
			}
		}

		if (firstEnterBattleOrQuitBattle) {
			validActionType = ValidActionType.All;
			foreach (Skill s in skillsEquiped) {
				s.isAvalible = true;
				s.actionCount = 0;
				foreach (BaseSkillEffect bse in s.skillEffects) {
					bse.actionCount = 0;
				}
			}
			#warning 这里暂时设定为每次进入战斗／战斗结束后气力值回满
			strength = maxStrength;
		}

		if (baView != null) {
			baView.UpdateHealthBarAnim (this);
			baView.UpdateStrengthBarAnim (this);
		}
	}

	public void AgentDie(CallBack cb){
		baView.AgentDieAnim (cb);
	}

	public override string ToString ()
	{
		return string.Format ("[agent]:" + agentName +
			"\n[attack]:" + attack + 
			"\n[power]:" + power + 
			"\n[magic]:" + magic +
			"\n[crit]:" + crit +
			"\n[amour]:" + amour +
			"\n[magicResist]:" + magicResist +
			"\n[agiglity]:" + agility +
			"\n[maxHealth]:" + maxHealth +
			"\n[maxStrength]:" + maxStrength);
	}
}



[System.Serializable]
public class BattleAgentModel{

	public string agentName;

	public string agentIconName;

	public bool isActive = true;

	public int agentLevel;

	//*****初始信息********//
	public int originalMaxHealth;
	public int originalMaxStrength;
	public int originalHealth;
	public int originalStrength;
	public int originalAttack;
	public int originalPower;
	public int originalMagic;
	public int originalCrit;
	public int originalAgility;
	public int originalAmour;
	public int originalMagicResist;
	//*****初始信息********//

	public int maxHealth;//最大血量
	public int maxStrength;//最大气力值

	public int health;
	public int strength;

	public int attack;//攻击力
	public int power;//力量
	public int magic;//魔法
	public int agility;//敏捷
	public int amour;//护甲
	public int magicResist;//魔抗
	public int crit;//暴击


}