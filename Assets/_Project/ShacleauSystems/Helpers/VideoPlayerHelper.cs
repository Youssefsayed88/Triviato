using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class VideoPlayerHelper : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;
        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
        }

        public void Play()
        {
            StartCoroutine(PlayVideo());
        }

        IEnumerator PlayVideo()
        {
            _videoPlayer.Stop();
            yield return new WaitForSeconds(0.01f);
            _videoPlayer.Play();
            yield return new WaitForSeconds(0.1f);
            transform.localScale = Vector3.one;
        }
    }
}