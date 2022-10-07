using Columns.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises.Answers
{
    public class DivideAnswer : ExerciseAnswer
    {
        public DivideAnswer(int answer, int remainder = 0) : base(answer)
        {
            Remainder = remainder;
        }

        public override List<int> CalcIntermediate(Exercise exercise)
        {
            if (exercise.EType != ExerciseType.Divide)
            {
                throw new WrongExerciseTypeException();
            }

            var result = new List<int>();
            var answer = exercise.GetAnswer().Answer;
            var len = answer.ToString().Length;
            var subNumber = new DivideAnswer(0, exercise.Number1);
            for (var i = 0; i < len; i++)
            {
                var answerDigit = (answer / (int)Math.Pow(10, len - i - 1)) % 10;
                var intermediateValue = answerDigit * exercise.Number2;

                // todo: вычислить подномер
                subNumber = CalcSubNumber(subNumber, intermediateValue);

                if (i > 0 && subNumber.Answer > 0)
                {
                    result.Add(subNumber.Answer);
                }
                if (intermediateValue > 0)
                {
                    result.Add(intermediateValue);
                }
                if (i == len - 1)
                {
                    result.Add(subNumber.Answer - intermediateValue);
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevSubNumber"></param>
        /// <param name="intermediateValue"></param>
        /// <param name="stepIndex"></param>
        /// <returns>в Answer находится взятое подчисло от делимого, в Remainder находится новое делимое после каждого этапа деления</returns>
        private DivideAnswer CalcSubNumber(DivideAnswer prevSubNumber, int intermediateValue)
        {
            var result = 0;
            var number = prevSubNumber.Remainder;
            var ivLen = intermediateValue.ToString().Length;
            var numberLen = number.ToString().Length;

            var i = 0;
            while (result < intermediateValue && i < numberLen)
            {
                result = result * 10 + number / (int)Math.Pow(10, numberLen - i - 1) % 10;
                i++;
            }
            i = (int)Math.Pow(10, numberLen - i);
            number = (result - intermediateValue) * i + number % i;

            return new DivideAnswer(result, number);
        }
    }
}
