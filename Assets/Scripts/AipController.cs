using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baidu.Aip.Speech;
using System.Net.Http;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

/// <summary>
/// 百度语音识别技术的交互类
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class AipController : MonoBehaviour
{
    private ListenButton listenBtn; // 继承重写的Button类
    private AudioSource recordSource;
    private AudioClip recordClip;

    #region UI面板控件
    public Image tImage;
    public Text recognizeText;
    public Color tokenGotColor;
    #endregion

    private string accessToken; // 访问AIP需要用的Token

    #region 百度语音技术应用
    private string API_KEY = "你的API KEY";
    private string SECRET_KEY = "你的Secret Key";
    private string authHost = "https://aip.baidubce.com/oauth/2.0/token";
    #endregion

    private Asr aipClient;  // 百度语音识别SDK

    void Start()
    {
        aipClient = new Asr(API_KEY, SECRET_KEY);   // 创建SDK的实例
        aipClient.Timeout = 6000;   // 超时时长为6000毫秒
        accessToken = GetAccessToken(); // 保存当前应用的Token

        // 获取自定义Button的实例
        listenBtn = GetComponentInChildren<ListenButton>();
        listenBtn.OnStartRecordEvent += StartRecord;
        listenBtn.OnStopRecordEvent += StopRecord;

        recordSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 点击按下说话开始录音
    /// </summary>
    public void StartRecord()
    {
        if (Microphone.devices.Length > 0)
        {
            string device = Microphone.devices[0];
            AudioClip clip = Microphone.Start(device, true, 60, 16000);
            recordSource.clip = clip;
            recordClip = clip;
        }
        else
        {
            SetRecognizeText(TipsReference.CANT_FIND_MICROPHONE);
            listenBtn.ReleaseClickEvent(TipsReference.RECORD_TYPE.NoMicroPhone);
        }
    }

    /// <summary>
    /// 松开按下说话停止录音并发送识别
    /// </summary>
    public void StopRecord()
    {
        Microphone.End(Microphone.devices[0]);
        StartCoroutine(Recognition(recordClip));
    }

    public void SetRecognizeText(string result)
    {
        recognizeText.text = result;
    }

    IEnumerator Recognition(AudioClip clip2Send)
    {
        float[] sample = new float[recordClip.samples];
        recordClip.GetData(sample, 0);
        short[] intData = new short[sample.Length];
        byte[] byteData = new byte[intData.Length * 2];

        for (int i = 0; i < sample.Length; i++)
        {
            intData[i] = (short)(sample[i] * short.MaxValue);
        }

        Buffer.BlockCopy(intData, 0, byteData, 0, byteData.Length);

        var result = aipClient.Recognize(byteData, "pcm", 16000);
        var speaking = result.GetValue("result");

        if(speaking == null)
        {
            SetRecognizeText(TipsReference.NOTHING_RECORD);
            StopAllCoroutines();
            yield return null;
        }

        string usefulText = speaking.First.ToString();
        SetRecognizeText(usefulText);

        yield return 0;
    }

    private string GetAccessToken()
    {
        HttpClient client = new HttpClient();
        List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>();
        paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
        paraList.Add(new KeyValuePair<string, string>("client_id", API_KEY));
        paraList.Add(new KeyValuePair<string, string>("client_secret", SECRET_KEY));

        HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
        string result = response.Content.ReadAsStringAsync().Result;
        //Debug.Log("result is " + result);
        if (result != null) tImage.color = tokenGotColor;
        return result;
    }

    public void DisplayClip()
    {
        recordSource.Play();
    }

}
