using System.Collections;
using Framework;



namespace Framework
{
public interface ILoadingScreen
{
	void Initialize(LoadingScreenConfig config);
	void Show();
	IEnumerator FadeIn();
	IEnumerator FadeOut();
	void SetProgress(float value);
	void SetStatus(string message);
	void HideInstant();
}
}