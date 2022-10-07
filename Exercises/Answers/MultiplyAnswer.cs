using Columns.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises.Answers
{
    public class MultiplyAnswer : ExerciseAnswer
    {
        public MultiplyAnswer(int answer) : base(answer)
        {
        }

        public override List<int> CalcIntermediate(Exercise exercise)
        {
            if (exercise.EType != ExerciseType.Multiply)
            {
                throw new WrongExerciseTypeException();
            }

            var result = new List<int>();
            var len = exercise.Number2.ToString().Length;
            for (var i = 0; i < len; i++)
            {
                var digit = (exercise.Number2 / (int) Math.Pow(10, i)) % 10;
                result.Add(digit * exercise.Number1);
            }

            return result;
        }
    }
}
