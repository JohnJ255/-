using Columns.Exercises;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Columns.ExcerciseView
{
    public class StandartPrinter: PrinterInterface
    {
        private int AnswerRowIndex = 0;
        private List<BoldLine> BoldLines;

        public DataGridView DataGrid { get; }

        public StandartPrinter(DataGridView dataGrid)
        {
            BoldLines = new List<BoldLine>();
            DataGrid = dataGrid;
        }

        public void Print(Exercise exercise)
        {
            BoldLines.Clear();
            PrintNumber(0, exercise.Number1);
            PrintNumber(1, exercise.Number2);
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

        public IEnumerable<SpecialText> GetSpecialTexts(Exercise exercise)
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

        private void PrintNumber(int y, int number1)
        {
            var x = 0;
            do
            {
                var chislo = number1 % 10;
                number1 = number1 / 10;
                DataGrid[x++, y].Value = chislo;
            } while (number1 > 0 && x < DataGrid.ColumnCount);
        }

        public void UpdateAnswer(Exercise currentExercise, Label labelAnswer, Label labelReminder)
        {
            var answer = "";
            for (int i = 0; i < DataGrid.Columns.Count && (DataGrid[i, AnswerRowIndex].Value != null); i++)
            {
                answer = DataGrid[i, AnswerRowIndex].Value.ToString() + answer;
            }
            labelAnswer.Text = answer.TrimStart(new[] { '0' });
            labelAnswer.ForeColor = Color.Black;
        }

        public IEnumerable<BoldLine> GetBoldLines()
        {
            return BoldLines;
        }

        public int GetBlockedRowsCount()
        {
            return 2;
        }

        public int GetPressOffsetX()
        {
            return 1;
        }
    }
}
