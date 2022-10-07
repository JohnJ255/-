using Columns.Exercises.Answers;
using System;

namespace Columns.Exercises
{
    public class Exercise
    {
        public int Number1;
        public int Number2;
        public ExerciseType EType;
        private Random Rand;

        public Exercise()
        {
            Rand = new Random();
            EType = (ExerciseType)Rand.Next(0, 4);
            InitAs(EType);
        }

        public void InitAs(ExerciseType eType)
        {
            EType = eType;
            Number1 = Rand.Next(10, 10000);
            Number2 = Rand.Next(10, 10000);
            if (EType == ExerciseType.Multiply)
            {
                Number2 = Rand.Next(10, 1000);
            }
            if (EType == ExerciseType.Divide)
            {
                Number2 = Rand.Next(1, 100);
            }
            if (Number1 < Number2)
            {
                var temp = Number1;
                Number1 = Number2;
                Number2 = temp;
            }
        }

        public ExerciseAnswer GetAnswer()
        {
            switch (EType)
            {
                case ExerciseType.Multiply: return new MultiplyAnswer(Number1 * Number2);
                case ExerciseType.Plus: return new ExerciseAnswer(Number1 + Number2);
                case ExerciseType.Minus: return new ExerciseAnswer(Number1 - Number2);
                default: return new DivideAnswer(Number1 / Number2, Number1 % Number2);
            }
        }
    }
}
