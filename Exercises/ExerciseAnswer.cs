using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises
{
    public class ExerciseAnswer
    {
        public readonly int Answer;
        public readonly int Remainder;

        public ExerciseAnswer(int answer, int remainder = 0)
        {
            Answer = answer;
            Remainder = remainder;
        }
    }
}
