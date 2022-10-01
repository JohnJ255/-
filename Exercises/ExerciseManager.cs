using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Columns.Exercises
{
    public class ExerciseManager
    {
        private List<ExerciseType> ExerciseTypesSet = new List<ExerciseType> { ExerciseType.Plus, ExerciseType.Multiply, ExerciseType.Minus, ExerciseType.Divide};
        private List<ExerciseType> ExerciseTypesCurrentSet = new List<ExerciseType>();
        private Random Rand = new Random();

        public Exercise NextExercise()
        {
            if (ExerciseTypesCurrentSet.Count == 0) {
                ExerciseTypesCurrentSet.AddRange(ExerciseTypesSet);
            }

            var exercise = new Exercise();
            var index = Rand.Next(0, ExerciseTypesCurrentSet.Count);
            var eType = ExerciseTypesCurrentSet[index];
            ExerciseTypesCurrentSet.RemoveAt(index);
            exercise.InitAs(eType);
            Console.WriteLine(eType);

            return exercise;
        }
    }
}
