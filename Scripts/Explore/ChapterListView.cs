using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChapterListView: MonoBehaviour,IPointerClickHandler {

	public int chapterIndex;

	public void OnPointerClick(PointerEventData data){
		GameObject.Find("ExploreManager").GetComponent<ExploreManager>().OnSelectChapter (chapterIndex);
	}
}
