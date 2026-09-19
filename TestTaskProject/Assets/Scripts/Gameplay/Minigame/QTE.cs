namespace Gameplay.Minigame
{
    public enum Arrow
    {
        Up,
        Down,
        Left,
        Right
    }

    public class ArrowSequence
    {
        public Arrow[] Arrows { get; }

        public ArrowSequence(params Arrow[] arrows)
        {
            Arrows = arrows;
        }
    }

    public enum QteResult
    {
        Effect1,
        Effect2,
        Failed
    }

    public class QteSession
    {
        public ArrowSequence Path1 { get; }
        public ArrowSequence Path2 { get; }
        public int Index1 { get; private set; }
        public int Index2 { get; private set; }
        public bool Path1Alive { get; private set; } = true;
        public bool Path2Alive { get; private set; } = true;
        public bool IsActive { get; private set; } = true;
        public float RemainingTime { get; private set; }

        public QteSession(ArrowSequence path1, ArrowSequence path2, float timeLimit)
        {
            Path1 = path1;
            Path2 = path2;
            RemainingTime = timeLimit;
        }

        public QteResult? Tick(float deltaTime)
        {
            if (!IsActive)
                return null;

            RemainingTime -= deltaTime;
            if (RemainingTime > 0f)
                return null;

            return Finish(QteResult.Failed);
        }

        public QteResult? Submit(Arrow input)
        {
            if (!IsActive)
                return null;

            var match1 = Path1Alive && Index1 < Path1.Arrows.Length && Path1.Arrows[Index1] == input;
            var match2 = Path2Alive && Index2 < Path2.Arrows.Length && Path2.Arrows[Index2] == input;

            if (!match1 && !match2)
                return Finish(QteResult.Failed);

            if (match1)
                Index1++;
            else
                Path1Alive = false;

            if (match2)
                Index2++;
            else
                Path2Alive = false;

            if (match1 && Index1 >= Path1.Arrows.Length)
                return Finish(QteResult.Effect1);

            if (match2 && Index2 >= Path2.Arrows.Length)
                return Finish(QteResult.Effect2);

            return null;
        }

        private QteResult Finish(QteResult result)
        {
            IsActive = false;
            return result;
        }
    }
}