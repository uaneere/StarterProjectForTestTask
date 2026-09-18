using UnityEngine;

namespace Gameplay.Presentation
{
    public class SpriteAnimator : MonoBehaviour
    {
        public float FrameTime = 0.3f;

        private SpriteRenderer _renderer;
        private Sprite[] _frames = System.Array.Empty<Sprite>();
        private int _index;
        private float _elapsed;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetFrames(Sprite[] frames, float frameTime)
        {
            if (frames == null)
            {
                _frames = System.Array.Empty<Sprite>();
            }
            else
            {
                var count = 0;
                foreach (var frame in frames)
                {
                    if (frame != null)
                        count++;
                }

                _frames = new Sprite[count];
                var index = 0;
                foreach (var frame in frames)
                {
                    if (frame != null)
                        _frames[index++] = frame;
                }
            }

            FrameTime = frameTime;
            _index = 0;
            _elapsed = 0f;
            Apply();
        }

        private void Update()
        {
            if (_frames.Length < 2)
                return;

            _elapsed += Time.unscaledDeltaTime;
            if (_elapsed < FrameTime)
                return;

            _elapsed = 0f;
            _index = (_index + 1) % _frames.Length;
            Apply();
        }

        private void Apply()
        {
            if (_renderer != null && _frames.Length > 0 && _frames[_index] != null)
                _renderer.sprite = _frames[_index];
        }
    }
}