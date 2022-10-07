using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises.Answers
{
    public class ExerciseAnswer: AnswerInterface
    {
        public int Answer { get; protected set; }

        public ExerciseAnswer(int answer)
        {
            Answer = answer;
        }

        public int Remainder { get; protected set; }

        public virtual List<int> CalcIntermediate(Exercise exercise)
        {
            return new List<int>();
        }
    }
}
