using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework
{
	public class DownloadImageHandler : MonoBehaviour
	{
		public void StartDownload(string url, Action<Sprite> callBack)
		{
			StartCoroutine(DownloadImage(url, callBack));
		}

		private IEnumerator DownloadImage(string url, Action<Sprite> result)
		{
			Sprite cardSprite = null;

			using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
			{
				yield return uwr.SendWebRequest();

				if (uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log(uwr.error);
				}
				else
				{
					// Get downloaded asset bundle
					var texture = DownloadHandlerTexture.GetContent(uwr);
					cardSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
				}
			}

			result.Invoke(cardSprite);
		}
	}
}