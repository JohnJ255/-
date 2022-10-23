using Columns.Exercises;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Columns.ExcerciseGrid
{
    public abstract class GridderBase : GridderInterface
    {
        protected int AnswerRowIndex = 0;
        protected List<BoldLine> BoldLines;
        protected List<Point> Mistakes;

        public DataGridView DataGrid { get; }

        public GridderBase(DataGridView dataGrid)
        {
            DataGrid = dataGrid;
            BoldLines = new List<BoldLine>();
            Mistakes = new List<Point>();
        }

        protected string ReadNumber(Point posOfLastDigit)
        {
            var answer = "";
            for (int i = posOfLastDigit.X; i < DataGrid.Columns.Count && (DataGrid[i, posOfLastDigit.Y].Value != null); i++)
            {
                answer = DataGrid[i, posOfLastDigit.Y].Value.ToString() + answer;
            }

            answer = answer.TrimStart(new[] { '0' });
            if (answer.Length == 0)
            {
                answer = "0";
            }

            return answer;
        }

        protected string ReadNumberWithSpaces(int xFrom, int xTo, int y)
        {
            var answer = "";
            for (int i = xFrom; i < xTo; i++)
            {
                if (DataGrid[i, y].Value == null)
                {
                    continue;
                }
                answer = DataGrid[i, y].Value.ToString() + answer;
            }
            answer = answer.TrimStart(new[] { '0' });

            if (answer == "")
            {
                answer = "0";
            }

            return answer;
        }

        public void ShowMistakes(Color color)
        {
            foreach (var m in Mistakes)
            {
                if (DataGrid[m.X, m.Y].Value != null) {
                    DataGrid[m.X, m.Y].Style.ForeColor = color;
                }
            }
        }

        public void ForgetMistakes()
        {
            foreach (var m in Mistakes)
            {
                DataGrid[m.X, m.Y].Style.ForeColor = Color.Black;
            }
            Mistakes.Clear();
        }

        public abstract void Print(Exercise currentExercise);

        public abstract void UpdateAnswer(Exercise currentExercise, Label labelAnswer, Label labelReminder);

        public abstract int GetBlockedRowsCount();

        public abstract int GetPressOffsetX();

        public abstract IEnumerable<SpecialText> GetSpecialTexts(Exercise currentExercise);

        public virtual bool CheckAnswer(Exercise currentExercise, bool andIntermediate = true)
        {
            var answer = currentExercise.GetAnswer();
            var userAnswer = ReadNumber(new Point(0, AnswerRowIndex));
            var result = answer.Answer.ToString() == userAnswer;

            if (!result)
            {
                FindAndSaveMistakes(userAnswer, answer.Answer, 0, AnswerRowIndex, true);
            }

            return result;
        }

        public virtual bool CheckRemainder(Exercise currentExercise, bool andIntermediate = true)
        {
            return true;
        }

        protected void PrintNumber(int xFrom, int y, int number1)
        {
            var x = xFrom;
            do
            {
                var chislo = number1 % 10;
                number1 = number1 / 10;
                DataGrid[x++, y].Value = chislo;
            } while (number1 > 0 && x < DataGrid.ColumnCount);
        }

        public IEnumerable<BoldLine> GetBoldLines()
        {
            return BoldLines;
        }

        protected bool FindAndSaveMistakes(string interUserNumber, int interAnswerNumberInt, int colIndexFrom, int rowIndex, bool useReverse)
        {
            var interAnswerNumber = interAnswerNumberInt.ToString();
            if (interAnswerNumber != interUserNumber)
            {
                for (var j = 0; j < interUserNumber.Length; j++)
                {
                    var k1 = j;
                    var k2 = j;
                    if (useReverse)
                    {
                        k1 = interUserNumber.Length - j - 1;
                        k2 = interAnswerNumber.Length - j - 1;
                    }
                    if (j >= interAnswerNumber.Length || interUserNumber[k1] != interAnswerNumber[k2])
                    {
                        Mistakes.Add(new Point(colIndexFrom + (useReverse ? j : interUserNumber.Length - j - 1), rowIndex));
                    }
                }

                return false;
            }

            return true;
        }

    }
}
