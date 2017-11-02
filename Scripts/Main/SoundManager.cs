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

		public AudioSource pronunciationAS;


		public float lowPitchRange = 0.95f;				
		public float highPitchRange = 1.05f;


			

		public void PlayClips(List<AudioClip> clips,SoundDetailTypeName name,string soundDetailName = null){

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

			List<AudioClip> detailClips = GameManager.Instance.gameDataCenter.allExploreAudioClips.FindAll (delegate(AudioClip obj) {
				return obj.name.Contains (detailType);
			});

			if (soundDetailName != null) {
				detailClips = detailClips.FindAll (delegate(AudioClip obj) {
					return obj.name.Contains(soundDetailName);
				});
			}

			clip = RandomAudioClip (detailClips);


			if (clip == null) {
				Debug.LogError("未找到音频文件");
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
			
	}
}
