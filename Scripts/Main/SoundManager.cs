using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public enum SoundType{
		UI,
		Explore
	}

	public enum SoundDetailTypeName{
		Steps,
		Map,
		Skill
	}

	public class SoundManager : MonoBehaviour {

		public AudioSource bgmAS;

		public AudioSource effectAS;

		public AudioSource pronunciationAS;


		public List<AudioClip> exploreClips = new List<AudioClip> ();

		public List<AudioClip> UIClips = new List<AudioClip>();

//		public List<AudioClip> skillEffectClips = new List<AudioClip> ();

		public float lowPitchRange = 0.95f;				
		public float highPitchRange = 1.05f;

		public void InitExploreAudioClips(){

			ResourceManager.Instance.LoadAssetWithBundlePath<AudioClip> ("audio/explore", () => {
				CopyClips(ResourceManager.Instance.audioClips,exploreClips,false);
			}, true);

		}


		public void InitUIAudioClips(){
			ResourceManager.Instance.LoadAssetWithBundlePath<AudioClip> ("audio/ui", () => {
				CopyClips(ResourceManager.Instance.audioClips,UIClips,true);
			}, true);
		}
			

		public void PlayClips(SoundType type,SoundDetailTypeName name,string soundDetailName = null){

			string detailType = string.Empty;

			switch (name) {
			case SoundDetailTypeName.Steps:
				detailType = "steps";
				break;
			case SoundDetailTypeName.Map:
				detailType = "map";
				break;
			case SoundDetailTypeName.Skill:
				detailType = "skill";
				break;
			}

			AudioClip clip = null;

			switch (type) {
			case SoundType.Explore:

				if (soundDetailName == string.Empty) {
					break;
				}

				List<AudioClip> clips = exploreClips.FindAll (delegate(AudioClip obj) {
					return obj.name.Contains (detailType);
				});

				if (soundDetailName != null) {
					clips = clips.FindAll (delegate(AudioClip obj) {
						return obj.name.Contains(soundDetailName);
					});
				}

				clip = RandomAudioClip (clips);

				break;
			case SoundType.UI:

				break;

			}

			if (clip == null) {
				return;
			}

			effectAS.clip = clip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();

		}




		private AudioClip RandomAudioClip(List<AudioClip> clips){

			if (clips.Count == 0) {
				Debug.Log ("clip not found");
				return null;
			}

			int index = Random.Range (0, clips.Count);

			return clips [index];

		}


		public void UnloadClips(SoundType soundType){

			switch (soundType) {
			case SoundType.Explore:
				for (int i = 0; i < exploreClips.Count; i++) {
					exploreClips [i].UnloadAudioData();
				}
				exploreClips.Clear ();
				break;
			case SoundType.UI:
				for (int i = 0; i < UIClips.Count; i++) {
					UIClips [i].UnloadAudioData();
				}
				UIClips.Clear ();
				break;
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
			
	}
}
