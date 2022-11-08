using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

public class ScreenShot : MonoBehaviour
{
    bool onCapture = false;
    public Image SaveNoticeImage;

    [SerializeField]
    private GameObject[] UIObjects;//스크린샷 촬영시 UI 없어지도록

    private void Awake()//Permission Chack
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
    }

    public void Start()
    {
        SaveNoticeImage.gameObject.SetActive(false);
    }

    public void PressBtnCapture()
    {
        if (onCapture == false)
        {

            foreach (GameObject obj in UIObjects)
            {
                obj.SetActive(false);
            }

            StartCoroutine("CRSaveScreenshot");
        }
    }

    IEnumerator CRSaveScreenshot()
    {
        onCapture = true;

        yield return new WaitForEndOfFrame();

        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);

            yield return new WaitForSeconds(0.2f);
            yield return new WaitUntil(() => Application.isFocused == true);
            if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false)
            {

                onCapture = false;
                yield break;
            }

        }
        else
        {
            string filename = Application.productName + "_" + System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";//확장자변경

            byte[] imageByte; //스크린샷을 Byte로 저장.Texture2D use 
            Texture2D tex = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
            tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, true);
            tex.Apply();

            imageByte = tex.EncodeToJPG();

            Debug.Log("Permission result: " + NativeGallery.SaveImageToGallery(tex, "WITHCUBE", filename));

            DestroyImmediate(tex);

            StartCoroutine("SaveImageFadeOut");

        }

    }

    IEnumerator SaveImageFadeOut()
    {
        SaveNoticeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.6f);
        SaveNoticeImage.CrossFadeAlpha(1.0f, 1.0f, true);
        yield return new WaitForSeconds(1.0f);
        SaveNoticeImage.gameObject.SetActive(false);

        foreach (GameObject obj in UIObjects)
        {
            obj.SetActive(true);
        }

        onCapture = false;

    }

    private void OpenAppSetting()
    {
        try
        {
#if UNITY_ANDROID
            using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                string packageName = currentActivityObject.Call<string>("getPackageName");

                using (var uriClass = new AndroidJavaClass("android.net.Uri"))
                using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
                using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
                {
                    intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                    intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
                    currentActivityObject.Call("startActivity", intentObject);
                }
            }
#endif
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

}
