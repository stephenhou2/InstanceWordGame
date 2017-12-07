using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{

	public enum SoundDetailTypeName{
		Steps,
		Map,
		Skill
	}

	public class SoundManager : MonoBehaviour {

		public AudioSource bgmAS;

		public AudioSource effectAS;

		public AudioSource footSoundAS;

		public AudioSource pronunciationAS;


		public float lowPitchRange = 0.95f;				
		public float highPitchRange = 1.05f;

//		private List<AudioClip> mAllExploreAudioClips = new List<AudioClip>();
//		public List<AudioClip> allExploreAudioClips{
//			get{
//				if (mAllExploreAudioClips.Count == 0) {
//					ResourceLoader exploreAudioLoader = ResourceLoader.CreateNewResourceLoader ();
//					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (exploreAudioLoader, CommonData.allExploreAudioClipsBundleName, () => {
//						CopyClips(exploreAudioLoader.audioClips,mAllExploreAudioClips,false);
//					}, true);
//				}
//				return mAllExploreAudioClips;
//			}
//		}

		private List<AudioClip> mAllFootStepAudioClips = new List<AudioClip>();
		public List<AudioClip> allFootStepAudioClips{
			get{
				if (mAllFootStepAudioClips.Count == 0) {
					ResourceLoader footStepAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (footStepAudioLoader, "audio/foot_step", () => {
						CopyClips (footStepAudioLoader.audioClips, mAllFootStepAudioClips, false);
					}, true);
				}
				return mAllFootStepAudioClips;
			}
		}

		private List<AudioClip> mAllMapEffectAudioClips = new List<AudioClip> ();
		public List<AudioClip> allMapEffectAudioClips{
			get{
				if (mAllMapEffectAudioClips.Count == 0) {
					ResourceLoader mapEffectAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (mapEffectAudioLoader, "audio/map_effect", () => {
						CopyClips(mapEffectAudioLoader.audioClips,mAllMapEffectAudioClips,false);
					},true);
				}
				return mAllMapEffectAudioClips;
			}
		}

		private List<AudioClip> mAllSkillEffectAudioClips = new List<AudioClip> ();
		public List<AudioClip> allSkillEffectAudioClips{
			get{
				if (mAllSkillEffectAudioClips.Count == 0) {
					ResourceLoader skillEffectAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (skillEffectAudioLoader, "audio/skill_effect", () => {
						CopyClips (skillEffectAudioLoader.audioClips, mAllSkillEffectAudioClips, false);
					}, true);
				}
				return mAllSkillEffectAudioClips;
			}
		}

		private List<AudioClip> mAllUIAudioClips = new List<AudioClip> ();
		public List<AudioClip> allUIClips{
			get{
				if (mAllUIAudioClips.Count == 0) {
					ResourceLoader UIAudioLoader = ResourceLoader.CreateNewResourceLoader ();
					ResourceManager.Instance.LoadAssetsWithBundlePath<AudioClip> (UIAudioLoader, CommonData.allUIAudioClipsBundleName, () => {
						CopyClips(UIAudioLoader.audioClips,mAllUIAudioClips,true);
					}, true);
				}
				return mAllUIAudioClips;
			}
		}

		private void CopyClips(List<AudioClip> originClips,List<AudioClip> targetClips,bool dontUnload){

			for(int i = 0;i<originClips.Count;i++){
				targetClips.Add(originClips[i]);
				if (dontUnload) {
					originClips [i].hideFlags = HideFlags.DontUnloadUnusedAsset;
				}
			}

		}


		public void PlayWordPronunciation(AudioClip pronunciation){

			pronunciationAS.clip = pronunciation;

			pronunciationAS.Play ();

		}

		public void PlaySkillEffectClips(string audioClipName){

			AudioClip skillClip = allSkillEffectAudioClips.Find (delegate(AudioClip obj) {
				return obj.name == audioClipName;
			});


			if (skillClip == null) {
				Debug.LogError(string.Format("名字为{0}的音频文件不存在",audioClipName));
			}

			effectAS.clip = skillClip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();

		}

		public void PlayFootStepClips(){

			AudioClip footStepClip = RandomAudioClip (allFootStepAudioClips);

			footSoundAS.clip = footStepClip;

			footSoundAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			footSoundAS.Play ();

		}

		public void PlayMapEffectClips(string audioClipName){

			AudioClip clip = allMapEffectAudioClips.Find (delegate(AudioClip obj) {
				return obj.name == audioClipName;
			});

			if (clip == null) {
				Debug.LogError(string.Format("名字为{0}的音频文件不存在",audioClipName));
			}

			effectAS.clip = clip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();


		}

//		public void PlayEffectClips(List<AudioClip> clips,SoundDetailTypeName name,string soundDetailName = null){
//
//			string detailType = string.Empty;
//
//			switch (name) {
//			case SoundDetailTypeName.Steps:
//				detailType = "steps";
//				break;
//			case SoundDetailTypeName.Map:
//				detailType = "map";
//				break;
//			case SoundDetailTypeName.Skill:
//				detailType = "skill";
//				break;
//			}
//
//			AudioClip clip = null;
//
//			List<AudioClip> detailClips = GameManager.Instance.gameDataCenter.allExploreAudioClips.FindAll (delegate(AudioClip obj) {
//				return obj.name.Contains (detailType);
//			});
//
//			if (soundDetailName != null) {
//				detailClips = detailClips.FindAll (delegate(AudioClip obj) {
//					return obj.name.Contains(soundDetailName);
//				});
//			}
//
//			clip = RandomAudioClip (detailClips);
//
//
//			if (clip == null) {
//				Debug.LogError("未找到音频文件");
//				return;
//			}
//
//			effectAS.clip = clip;
//
//			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);
//
//			effectAS.Play ();
//
//		}




		private AudioClip RandomAudioClip(List<AudioClip> clips){

			if (clips.Count == 0) {
				Debug.Log ("clip not found");
				return null;
			}

			int index = Random.Range (0, clips.Count);

			return clips [index];

		}
			
	}
}
