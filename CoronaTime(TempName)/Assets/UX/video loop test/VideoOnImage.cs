using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoOnImage : MonoBehaviour
{
    public RawImage rawImage;
    public VideoPlayer videoPlayer;
    public float waitForSecondsTimer;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(PlayVideo());
    }
    IEnumerator PlayVideo()
    {
        videoPlayer.Prepare();
        WaitForSeconds waitForSeconds = new WaitForSeconds(waitForSecondsTimer);
        while (!videoPlayer.isPrepared)
        {
            yield return waitForSeconds;
            break;
        }
        rawImage.texture = videoPlayer.texture;
        videoPlayer.Play();
    }
}
