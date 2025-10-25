using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class UnityEventOnVideoEnd : MonoBehaviour
    {
        private VideoPlayer _videoPlayer;
        public UnityEvent UnityEventOnVideoFinsih;
        private void Awake()
        {
            _videoPlayer = GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += VideoPlayerOnloopPointReached;
        }

        private void VideoPlayerOnloopPointReached(VideoPlayer source)
        {
            UnityEventOnVideoFinsih.Invoke();
        }
    }
}