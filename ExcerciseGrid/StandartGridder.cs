using Columns.Exercises;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Columns.ExcerciseGrid
{
    public class StandartGridder: GridderBase
    {
        public StandartGridder(DataGridView dataGrid): base(dataGrid)
        {
        }

        public override void Print(Exercise exercise)
        {
            BoldLines.Clear();
            PrintNumber(0, 0, exercise.Number1);
            PrintNumber(0, 1, exercise.Number2);
            DrawBoldLine(1, exercise.Number1.ToString().Length + 2);
            if (exercise.EType == ExerciseType.Multiply)
            {
                AnswerRowIndex = exercise.Number2.ToString().Length + 2;
                DrawBoldLine(AnswerRowIndex - 1, exercise.Number1.ToString().Length + 3);
            }
            else
            {
                AnswerRowIndex = 2;
            }
            DataGrid.CurrentCell = DataGrid.Rows[2].Cells[0];
        }

        public override IEnumerable<SpecialText> GetSpecialTexts(Exercise exercise)
        {
            var types = new Dictionary<ExerciseType, string> {
                { ExerciseType.Multiply, "X" },
                { ExerciseType.Plus, "+" },
                { ExerciseType.Minus, "-" },
            };

            var len = exercise.Number1.ToString().Length;
            var offset = new Point(27, -12);
            if (exercise.EType == ExerciseType.Minus)
            {
                offset.Y = -14;
            }
            var result = new List<SpecialText>();
            result.Add(new SpecialText(len + 2, 1, offset, types[exercise.EType]));
            return result;
        }

        private void DrawBoldLine(int y, int toX)
        {
            BoldLines.Add(new BoldLine(new Point(0, y), new Point(toX, y), BoldLineType.Bottom));
        }

        public override void UpdateAnswer(Exercise currentExercise, Label labelAnswer, Label labelReminder)
        {
            labelAnswer.Text = ReadNumber(new Point(0, AnswerRowIndex));
            labelAnswer.ForeColor = Color.Black;
        }

        public override int GetBlockedRowsCount()
        {
            return 2;
        }

        public override int GetPressOffsetX()
        {
            return 1;
        }

        public override bool CheckAnswer(Exercise currentExercise, bool andIntermediate = true)
        {
            var answer = currentExercise.GetAnswer();
            var result = base.CheckAnswer(currentExercise, andIntermediate);
            if (!andIntermediate)
            {
                return result;
            }

            var intermediates = answer.CalcIntermediate(currentExercise);
            for (var i = 0; i < intermediates.Count; i++)
            {
                var rowIndex = AnswerRowIndex - intermediates.Count + i;
                var interUserNumber = ReadNumber(new Point(i, rowIndex));
                var interAnswerNumber = intermediates[i];
                var res = FindAndSaveMistakes(interUserNumber, interAnswerNumber, i, rowIndex, true);
                if (!res)
                {
                    return false;
                }
            }

            return base.CheckAnswer(currentExercise, andIntermediate);
        }
    }
}
