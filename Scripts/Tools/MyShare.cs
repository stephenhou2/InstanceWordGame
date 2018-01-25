using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cn.sharesdk.unity3d;
using UnityEngine.UI;


namespace WordJourney
{

	public enum ShareResult{
		Success,
		Failed,
		Cancel
	}

	public class MyShare : MonoBehaviour {

	    //定义分享对象
		private ShareSDK ssdk;

		public TintHUD tintHUD;


		// Use this for initialization
		void Start () {
			ssdk = Camera.main.GetComponent<ShareSDK> ();
	        //处理回调函数
	        ssdk.shareHandler = ShareResultHandler;

		}


	    //分享的回调函数
		void ShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	    {
	        if (state == ResponseState.Success)
	        {
				string tintStr = "分享成功，获得水晶x30";
				tintHUD.SetUpTintHUD (tintStr, null);
				return;
	        }
	        else if (state == ResponseState.Fail)
			{
				string tintStr = "打开客户端失败";
				tintHUD.SetUpTintHUD (tintStr, null);
	        }
	        else if (state == ResponseState.Cancel)
	        {
				return;
	        }
	    }

	    //按钮点击截屏分享到微信
	    public void BtnShareToWechatOnClick(){

	        Application.CaptureScreenshot("screenshot.png");
	        StartCoroutine(ShareToWeChat(0.5f));

	    }

	    private IEnumerator ShareToWeChat(float time){
	        yield return new WaitForSeconds(time);

	        //存储路径
	        string imagePath = Application.persistentDataPath;
	        imagePath = imagePath + "/screenshot.png";

	        ShareContent content = new ShareContent();

	        content.SetImagePath(imagePath);

	        //设置分享的类型
	        content.SetShareType(ContentType.Image);

	        //直接分享
	        ssdk.ShareContent(PlatformType.WeChatMoments, content);


	    }

	    //按钮点击分享到微博-截屏的操作可以与微信的合并
	    public void BtnShareToWeiboOnClick()
	    {

	        Application.CaptureScreenshot("screenshot2.jpg");
	        StartCoroutine(ShareToWeibo(0.5f));

	    }

	    private IEnumerator ShareToWeibo(float time)
	    {
	        yield return new WaitForSeconds(time);

	        //存储路径
	        string imagePath = Application.persistentDataPath;
	        imagePath = imagePath + "/screenshot2.jpg";

	        ShareContent content = new ShareContent();



	        content.SetImagePath(imagePath);

	        //设置分享的类型
	        content.SetShareType(ContentType.Image);

	        //直接分享
	        ssdk.ShareContent(PlatformType.SinaWeibo, content);


	    }


	}
}
