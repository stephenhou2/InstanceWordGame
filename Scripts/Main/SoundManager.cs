using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace WordJourney
{
	public class SoundManager : MonoBehaviour {

		public AudioSource bgmAS;

		public AudioSource effectAS;

		public AudioSource pronunciationAS;


		private List<AudioClip> stepClips;

		private List<AudioClip> mapItemClips;

		private List<AudioClip> skillEffectClips;

		public float lowPitchRange = 0.95f;				
		public float highPitchRange = 1.05f;



		public void InitAudioClips(){

			ResourceManager.Instance.LoadAssetWithBundlePath<AudioClip> ("audio/steps", () => {
				stepClips = CopyClips(ResourceManager.Instance.audioClips);
			}, true);

			ResourceManager.Instance.LoadAssetWithBundlePath<AudioClip> ("audio/map_items", () => {
				mapItemClips = CopyClips(ResourceManager.Instance.audioClips);
			}, true);

			ResourceManager.Instance.LoadAssetWithBundlePath <AudioClip>("audio/skill_effects",()=>{
				skillEffectClips = CopyClips(ResourceManager.Instance.audioClips);
			},true);
		}
			

		public void PlayStepClips(){

			AudioClip clip = RandomAudioClip (stepClips);

			if (clip == null) {
				return;
			}

			effectAS.clip = clip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();

		}

		public void PlayMapItemClip(MapItem mapItem){

			AudioClip clip = mapItemClips.Find (delegate(AudioClip obj) {
				return obj.name.Contains(mapItem.mapItemName);
			});

			if (clip == null) {
				return;
			}

			effectAS.clip = clip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();

		}

		public void PlaySkillEffectClip(Skill skill){

			AudioClip clip = skillEffectClips.Find (delegate(AudioClip obj) {
				return obj.name.Contains(skill.skillEngName);
			});

			if (clip == null) {
				return;
			}

			effectAS.clip = clip;

			effectAS.pitch = Random.Range (lowPitchRange, highPitchRange);

			effectAS.Play ();

		}


		private AudioClip RandomAudioClip(List<AudioClip> clips){

			int index = Random.Range (0, clips.Count);

			return clips [index];

		}

		private List<AudioClip> CopyClips(List<AudioClip> clips){
			List<AudioClip> copiedClips = new List<AudioClip> ();
			for(int i = 0;i<clips.Count;i++){
				copiedClips.Add(clips[i]);
			}
			return copiedClips;
		}
	}
}
