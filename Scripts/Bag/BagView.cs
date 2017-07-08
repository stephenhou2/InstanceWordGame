using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BagView : MonoBehaviour {


	public Slider healthBar;
	public Slider strengthBar;

	public Text healthText;
	public Text strengthText;

	public Text playerLevelText;
	public Text progressText;

	public Text attackText;
	public Text magicText;
	public Text amourText;
	public Text magicResistText;
	public Text critText;
	public Text agilityText;

	public Image weaponImage;
	public Image amourImage;
	public Image shoesImage;
	public Image[] consumablesImages;

	public Button[] bags;

	public Button[] allItemsBtns;



	public Image itemIcon;
	public Text itemName;
	public Text propertyText;

	private Player player;

	private List<Sprite> sprites = new List<Sprite> ();

	public GameObject bagPlane;
	public GameObject specificTypeItemPlane;
	public GameObject itemDetailHUD;



	public void SetUpBagView(List<Sprite> sprites){



		this.sprites = sprites;
		this.player = Player.mainPlayer;


		SetUpPlayerStatusPlane ();

		SetUpEquipedItemPlane ();

		SetUpAllItemsPlane ();

		this.GetComponent<Canvas> ().enabled = true;

	}


	private void SetUpPlayerStatusPlane(){

		healthBar.maxValue = player.maxHealth;
		healthBar.value = player.health;

		strengthBar.maxValue = player.maxStrength;
		strengthBar.value = player.strength;

		healthText.text = player.health.ToString () + "/" + player.maxHealth.ToString ();
		strengthText.text = player.strength.ToString () + "/" + player.maxStrength.ToString ();

		playerLevelText.text = "Lv." + player.agentLevel;
		#warning 进度文字未设置

		attackText.text = "基础攻击:" + player.attack.ToString ();
		magicText.text = "基础魔法:" + player.magic.ToString ();
		amourText.text = "基础护甲:" + player.amour.ToString();
		magicResistText.text = "基础抗性:" + player.magicResist.ToString();
		critText.text = "基础暴击:" + (player.crit / (1 + 0.01f * player.crit)).ToString("F0") + "%";
		agilityText.text = "基础闪避:" + (player.agility / (1 + 0.01f * player.agility)).ToString("F0") + "%";

	}

	private void SetUpEquipedItemPlane(){

		SetIcon (player.weaponEquiped, weaponImage);
		SetIcon (player.amourEquiped, amourImage);
		SetIcon (player.shoesEquiped, shoesImage);

		for(int i = 0;i<player.consumablesEquiped.Count;i++){
			Item item = player.consumablesEquiped[i];
			SetIcon (item, consumablesImages [i]);
		}

	}

	private void SetUpAllItemsPlane(){
		for (int i = 0; i < player.allItems.Count; i++) {
			Image itemIcon = allItemsBtns [i].transform.FindChild("ItemIcon").GetComponent<Image>();
			SetIcon (player.allItems [i], itemIcon);
		}

	}


	public void OnEquipedItemButtonsClick(int index){

		specificTypeItemPlane.SetActive (true);

	}

	public void OnItemButtonClick(int index){

		itemDetailHUD.SetActive (true);
	}

	public void OnItemDetailHUDClick(){
		itemDetailHUD.SetActive (false);
	}

	private void SetIcon(Item item,Image image){
		if (item.itemName != "") {
			image.enabled = true;
			image.sprite = sprites.Find (delegate(Sprite obj) {
				return obj.name == item.spriteName;
			});
		}
	}

	public void OnQuitBagPlane(){
		bagPlane.transform.DOLocalMoveY (-Screen.height, 0.5f).OnComplete (() => {
			Destroy (GameObject.Find ("BagCanvas"));
		});
	}
	public void OnQuitSpecificTypePlane(){
		specificTypeItemPlane.SetActive (false);
	}
}
