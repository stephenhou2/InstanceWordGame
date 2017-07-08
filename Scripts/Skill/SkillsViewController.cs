using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsViewController : MonoBehaviour {

	public SkillsView skillsView;

	private List<Skill> mSkills = new List<Skill>();
	private List<Sprite> mSprites = new List<Sprite>();


	public void OnEnterSkillsView(){

		if(mSkills.Count == 0 || mSprites.Count == 0)
		ResourceManager.Instance.LoadAssetWithFileName ("skills/skills", () => {

			Transform skillsTrans = ContainerManager.NewContainer("Skills",GameObject.Find(CommonData.instanceContainerName).transform);
			foreach(GameObject go in ResourceManager.Instance.gos){
				mSkills.Add(go.GetComponent<Skill>());
				go.transform.SetParent(skillsTrans);
			}

			foreach (Sprite s in ResourceManager.Instance.sprites) {
				mSprites.Add (s);
			}
			skillsView.SetUpSkillsView (mSkills, mSprites);
		});

	}
}
