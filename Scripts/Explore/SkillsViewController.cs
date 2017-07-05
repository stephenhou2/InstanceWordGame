using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsViewController : MonoBehaviour {

	public SkillsView skillsView;

	private List<Skill> mSkills = new List<Skill>();
	private List<Sprite> mSprites = new List<Sprite>();


	public void OnEnterSkillsView(){
//		skillsView.ctrl = this;
		ResourceManager.Instance.LoadAssetWithName ("skills/skills", () => {
			
			if(mSkills.Count == 0){
				Transform skillsTrans = ContainerManager.NewContainer("Skills",GameObject.Find(CommonData.instanceContainerName).transform);
				foreach(GameObject go in ResourceManager.Instance.gos){
					mSkills.Add(go.GetComponent<Skill>());
					go.transform.SetParent(skillsTrans);
				}
			}
			if(mSprites.Count == 0){
				mSprites = ResourceManager.Instance.sprites;
			}
			skillsView.SetUpSkillsView (mSkills, mSprites);
		});

	}
}
