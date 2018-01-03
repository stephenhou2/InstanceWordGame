using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class TrapSwitch : MapItem {

		// 开关控制的陷阱
//		public Trap trap;

		private Sprite m_SwitchOffSprite;
		private Sprite switchOffSprite{
			get{
				if (m_SwitchOffSprite == null) {
					m_SwitchOffSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
						return obj.name == "normal_switch_off";
					});
				}
				return m_SwitchOffSprite;
			}
		}

		private Sprite m_SwitchOnSprite;
		private Sprite switchOnSprite{
			get{
				if (m_SwitchOnSprite == null) {
					m_SwitchOnSprite = GameManager.Instance.gameDataCenter.allItemSprites.Find (delegate(Sprite obj) {
						return obj.name == "normal_switch_on";
					});
				}
				return m_SwitchOnSprite;
			}
		}

		private int switchStatusChangeCount;

		/// <summary>
		/// 初始化开关
		/// </summary>
		public override void InitMapItem ()
		{
			bc2d.enabled = true;
			mapItemRenderer.sprite = switchOffSprite;
		}

		/// <summary>
		/// 关闭陷阱
		/// </summary>
		public void ChangeSwitchStatus(){

			switchStatusChangeCount++;

			if (switchStatusChangeCount % 2 == 0) {
				mapItemRenderer.sprite = switchOffSprite;
			} else {
				mapItemRenderer.sprite = switchOnSprite;
			}
				
		}

	}
}
