using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises.Answers
{
    internal interface AnswerInterface
    {
        int Answer { get; }
        int Remainder { get; }

        List<int> CalcIntermediate(Exercise exercise);
    }
}
