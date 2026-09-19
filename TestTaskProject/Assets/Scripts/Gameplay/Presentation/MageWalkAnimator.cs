using Gameplay.Player;
using UnityEngine;

namespace Gameplay.Presentation
{
    public class MageWalkAnimator : MonoBehaviour
    {
        private const float FrameTime = 0.3f;

        private MageController _mage;
        private SpriteRenderer _renderer;
        private Sprite _idle;
        private Sprite[] _down;
        private Sprite[] _up;
        private Sprite[] _left;
        private Sprite[] _right;
        private Sprite[] _current;
        private int _frame;
        private float _elapsed;
        private Vector2 _lastDir = Vector2.down;

        private void Awake()
        {
            _mage = GetComponent<MageController>();
            _renderer = GetComponent<SpriteRenderer>();
            _idle = SpriteLoader.Load("MCstand", new Color(0.45f, 0.35f, 0.72f));
            _down = new[]
            {
                SpriteLoader.Load("MCdown1"),
                SpriteLoader.Load("MCdown1hatup"),
                SpriteLoader.Load("MCdown2"),
                SpriteLoader.Load("MCdown2hatup")
            };
            _up = new[]
            {
                SpriteLoader.Load("MCup"),
                SpriteLoader.Load("MCuphatup")
            };
            _left = new[]
            {
                SpriteLoader.Load("MCleft"),
                SpriteLoader.Load("MClefthatup")
            };
            _right = new[]
            {
                SpriteLoader.Load("MCright"),
                SpriteLoader.Load("MCrighthatup")
            };
            _renderer.sprite = _idle;
        }

        private void Update()
        {
            var input = _mage != null ? _mage.MoveInput : Vector2.zero;
            if (input.sqrMagnitude < 0.01f)
            {
                _renderer.sprite = _idle != null ? _idle : _renderer.sprite;
                _frame = 0;
                _elapsed = 0f;
                return;
            }

            _lastDir = input;
            var frames = FramesFor(_lastDir);
            if (frames != _current)
            {
                _current = frames;
                _frame = 0;
                _elapsed = 0f;
                Apply(frames);
                return;
            }

            _elapsed += Time.deltaTime;
            if (_elapsed < FrameTime)
                return;

            _elapsed = 0f;
            _frame = (_frame + 1) % frames.Length;
            Apply(frames);
        }

        private Sprite[] FramesFor(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return direction.x < 0f ? Usable(_left) : Usable(_right);

            return direction.y > 0f ? Usable(_up) : Usable(_down);
        }

        private Sprite[] Usable(Sprite[] frames)
        {
            var count = 0;
            foreach (var frame in frames)
            {
                if (frame != null)
                    count++;
            }

            if (count == frames.Length)
                return frames;

            var compact = new Sprite[count];
            var index = 0;
            foreach (var frame in frames)
            {
                if (frame != null)
                    compact[index++] = frame;
            }

            return compact.Length > 0 ? compact : new[] { _idle };
        }

        private void Apply(Sprite[] frames)
        {
            if (frames == null || frames.Length == 0)
                return;

            var sprite = frames[_frame % frames.Length];
            if (sprite != null)
                _renderer.sprite = sprite;
        }
    }
}