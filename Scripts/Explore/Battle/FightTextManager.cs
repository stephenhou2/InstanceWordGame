using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	using UnityEngine.UI;
	using DG.Tweening;

	public enum MyTowards{
		Left,
		Right,
		Up,
		Down
	}

	public class FightText{
		public string text;
		public SpecialAttackResult specialResult;
		public int indexInFightTextList;
		public FightText(string text,SpecialAttackResult specialResult){
			this.text = text;
			this.specialResult = specialResult;
		}
	}


	public class FightTextManager : MonoBehaviour {

		private InstancePool fightTextPool;

		private Transform fightTextModel;

		private Transform fightTextContainer;

		private Vector3 basePosition;

		private MyTowards direction;

		private float fightTextInterval = 0.1f;

		private IEnumerator fightTextCoroutine;

		private List<FightText> fightTextList = new List<FightText> ();

		private float lastFightTextPlayTime;

		public void InitFightTextManager(InstancePool fightTextPool,Transform fightTextModel,Transform fightTextContainer){
			this.fightTextPool = fightTextPool;
			this.fightTextModel = fightTextModel;
			this.fightTextContainer = fightTextContainer;
		}

		public void SetUpFightTextManager(Vector3 selfPos,Vector3 enemyPos){

			direction = selfPos.x <= enemyPos.x ? MyTowards.Left : MyTowards.Right;

			basePosition = ToPointInCanvas (selfPos);

		}


		public void AddFightText(FightText fightText){
			fightText.indexInFightTextList = fightTextList.Count;
			fightTextList.Add (fightText);
			StartCoroutine ("ShowANewFightText",fightText);
		}

		private IEnumerator ShowANewFightText(FightText fightText){

			// 如果 fightText 不在显示队列的队首，则一直等待
			while (fightText.indexInFightTextList > 0) {
				yield return null;
			}

			// fightText现在在显示队列的队首，则等待显示间隔事件后显示
			yield return new WaitForSeconds (fightTextInterval);

			ShowFightText (fightText);

			// 将fightText从显示队列中移除
			fightTextList.RemoveAt (0);

			// 显示队列中的其他 fightText 在队列中整体左移（由于fightText的队列序号是在fightText内部存储，所以需要手动移动整个队列中的fightText）
			for (int i = 0; i < fightTextList.Count; i++) {
				fightTextList [i].indexInFightTextList--;
			}

		}


		private void ShowFightText(FightText fightText){

			switch (fightText.specialResult) {
			case SpecialAttackResult.None:
			case SpecialAttackResult.Crit:
			case SpecialAttackResult.Miss:
				PlayHurtTextAnim (fightText);
				break;
			case SpecialAttackResult.Gain:
				PlayGainTextAnim (fightText);
				break;
			}

		}


		// 受到伤害文本动画
		public void PlayHurtTextAnim(FightText ft){

			// 从缓存池获取文本模型
			Text hurtText = fightTextPool.GetInstance<Text> (fightTextModel.gameObject, fightTextContainer);

			Vector3 originHurtPos = Vector3.zero;
			Vector3 firstHurtPos = Vector3.zero;
			Vector3 secondHurtPos = Vector3.zero;
			Vector3 originTintPos = Vector3.zero;
			Vector3 finalTintPos = Vector3.zero;

			switch(direction){
			case MyTowards.Left:
				originHurtPos = basePosition + new Vector3 (-50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (-Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (-Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (-100f, 100f, 0);
				break;
			case MyTowards.Right:
				originHurtPos = basePosition + new Vector3 (50f, 50f, 0);
				firstHurtPos = originHurtPos + new Vector3 (Random.Range(80,100), Random.Range(0,10), 0);
				secondHurtPos = firstHurtPos + new Vector3 (Random.Range(20,30), Random.Range(0,2), 0);
				originTintPos = originHurtPos + new Vector3 (100f, 100f, 0);
				break;
			}

			hurtText.transform.localPosition = originHurtPos;

			hurtText.text = string.Format ("<b>{0}</b>", ft.text);

			hurtText.gameObject.SetActive (true);

			// 根据效果类型播放效果文本动画
			switch (ft.specialResult) {
			case SpecialAttackResult.None:
				break;
			case SpecialAttackResult.Crit:
				string tintStr = "<color=red>暴击</color>";
				PlayTintTextAnim (tintStr, originTintPos);
				break;
			case SpecialAttackResult.Miss:
				tintStr = "<color=gray>Miss</color>";
				PlayTintTextAnim (tintStr, originTintPos);
				return;
			}

			float firstJumpPower = Random.Range (100f, 120f);

			// 伤害文本跳跃动画
			hurtText.transform.DOLocalJump (firstHurtPos, firstJumpPower, 1, 0.35f).OnComplete(()=>{

				float secondJumpPower = Random.Range(20f,30f);

				// 伤害文本二次跳跃
				hurtText.transform.DOLocalJump (secondHurtPos, secondJumpPower, 1, 0.15f).OnComplete(()=>{
					hurtText.gameObject.SetActive(false);
					fightTextPool.AddInstanceToPool(hurtText.gameObject);
//					fightTextList.Remove(hurtText);
				});

			});

		}

		/// <summary>
		/// 吸血文本动画
		/// </summary>
		/// <param name="gainStr">Gain string.</param>
		/// <param name="agentPos">Agent position.</param>
		public void PlayGainTextAnim(FightText fightText){

			Vector3 pos = Vector3.zero;

			switch (direction) {
			case MyTowards.Left:
				pos = basePosition + new Vector3 (-50f, 150f, 0);
				break;
			case MyTowards.Right:
				pos = basePosition + new Vector3 (50f, 150f, 0);
				break;
			}

			Text gainText = fightTextPool.GetInstance<Text> (fightTextModel.gameObject, fightTextContainer);

			gainText.transform.localPosition = pos;

			gainText.text = fightText.text;

			gainText.gameObject.SetActive (true);

			float endY = pos.y + 100f;

			gainText.transform.DOLocalMoveY (endY, 1f).OnComplete(()=>{
				gainText.gameObject.SetActive(false);
				fightTextPool.AddInstanceToPool(gainText.gameObject);
//				fightTextList.Remove(gainText);
			});

		}

		/// <summary>
		/// 效果提示文本动画（暴击，闪避）
		/// </summary>
		/// <param name="tintStr">Tint string.</param>
		/// <param name="originPos">Origin position.</param>
		private void PlayTintTextAnim(string tintStr, Vector3 originPos){

			Text tintText = fightTextPool.GetInstance<Text> (fightTextModel.gameObject, fightTextContainer);

			tintText.transform.localPosition = originPos;

			tintText.text = tintStr;

			tintText.gameObject.SetActive (true);

			tintText.transform.DOScale(new Vector3(1.5f,1.5f,1.5f),0.5f).OnComplete (() => {

				tintText.transform.localScale = Vector3.one;

				tintText.gameObject.SetActive(false);

				fightTextPool.AddInstanceToPool(tintText.gameObject);

//				fightTextList.Remove(tintText);

			});
		}


		/// <summary>
		/// 世界坐标转2D画布中的坐标
		/// </summary>
		/// <returns>The point in canvas.</returns>
		/// <param name="worldPos">World position.</param>
		private Vector3 ToPointInCanvas(Vector3 worldPos){

			Vector3 posInScreen = Camera.main.WorldToScreenPoint (worldPos);

			Vector3 posInCanvas = new Vector3 (posInScreen.x * CommonData.scalerToPresetResulotion, posInScreen.y * CommonData.scalerToPresetResulotion, posInScreen.z);

			return posInCanvas;

		}





	}
		

}
