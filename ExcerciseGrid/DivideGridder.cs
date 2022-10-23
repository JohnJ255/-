using Columns.Exercises;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Columns.ExcerciseGrid
{
    public class DivideGridder : GridderBase
    {
        const int ANSWER_OFFSET = 1;
        const int MINUS_OFFSET = 2;
        private List<SpecialText> MinusSymbols;

        public DivideGridder(DataGridView dataGrid) : base(dataGrid)
        {
            MinusSymbols = new List<SpecialText>();
        }

        public override IEnumerable<SpecialText> GetSpecialTexts(Exercise exercise)
        {
            return MinusSymbols;
        }

        public override int GetBlockedRowsCount()
        {
            return 1;
        }

        public override void Print(Exercise exercise)
        {
            BoldLines.Clear();
            MinusSymbols.Clear();
            var len = GetRightPartLength(exercise);
            var diff = len - exercise.Number2.ToString().Length;
            PrintNumber(len + ANSWER_OFFSET, 0, exercise.Number1);
            PrintNumber(diff + ANSWER_OFFSET, 0, exercise.Number2);
            BoldLines.Add(new BoldLine(new Point(0, 0), new Point(len + ANSWER_OFFSET, 0), BoldLineType.Bottom));
            BoldLines.Add(new BoldLine(new Point(len + ANSWER_OFFSET, 0), new Point(len + ANSWER_OFFSET, 1), BoldLineType.Right));
            AnswerRowIndex = 1;
            DataGrid.CurrentCell = DataGrid.Rows[1].Cells[len + ANSWER_OFFSET - 1];
        }

        private int GetRightPartLength(Exercise exercise)
        {
            return Math.Max(exercise.Number2.ToString().Length, exercise.GetAnswer().Answer.ToString().Length);
        }

        private string ReadRemainder(int x)
        {
            var remainder = "";
            var isSpace = true;
            var lastRow = 0;
            for (int j = 1; j < DataGrid.RowCount; j++)
            {
                if (isSpace && DataGrid[x, j].Value != null)
                {
                    lastRow = j;
                    isSpace = false;
                    continue;
                }
                if (isSpace && DataGrid[x, j].Value == null)
                {
                    continue;
                }
                if (!isSpace && DataGrid[x, j].Value == null)
                {
                    lastRow = j - 1;
                    break;
                }
            }
            if (lastRow > 0)
            {
                for (int i = x; i < DataGrid.ColumnCount && DataGrid[i, lastRow].Value != null; i++)
                {
                    remainder = DataGrid[i, lastRow].Value.ToString() + remainder;
                }
            }
            remainder = remainder.TrimStart(new[] { '0' });
            if (remainder == "")
            {
                remainder = "0";
            }
            return remainder;
        }

        public override void UpdateAnswer(Exercise exercise, Label labelAnswer, Label labelReminder)
        {
            var len = GetRightPartLength(exercise);
            var answer = ReadNumberWithSpaces(0, len + ANSWER_OFFSET, AnswerRowIndex);
            var remainder = ReadRemainder(len + ANSWER_OFFSET);

            labelAnswer.Text = answer;
            labelAnswer.ForeColor = Color.Black;

            labelReminder.Text = remainder;
            labelReminder.ForeColor = Color.Black;

            UpdateTemporaryBoldLines(exercise);
        }

        private void UpdateTemporaryBoldLines(Exercise exercise)
        {
            while (BoldLines.Count > 2)
            {
                BoldLines.RemoveAt(2);
            }
            MinusSymbols.Clear();

            var len = GetRightPartLength(exercise);
            for (int j = 1; j < DataGrid.RowCount; j += 2)
            {
                for (int i = 0; i < DataGrid.ColumnCount; i++)
                {
                    if (j == 1 && i < len + ANSWER_OFFSET)
                    {
                        continue;
                    }

                    if (DataGrid[i, j].Value != null)
                    {
                        var numberLen = 0;
                        for (int k = i + 1; k < DataGrid.ColumnCount && DataGrid[k, j].Value != null; k++)
                        {
                            numberLen++;
                        }
                        BoldLines.Add(new BoldLine(new Point(i, j), new Point(i + numberLen, j), BoldLineType.Bottom));

                        var offset = new Point(27, -19);
                        var ki = i + numberLen + MINUS_OFFSET;
                        while (ki + 1 < DataGrid.ColumnCount && DataGrid[ki + 1 - MINUS_OFFSET, j - 1].Value != null)
                        {
                            ki++;
                        }

                        MinusSymbols.Add(new SpecialText(ki, j, offset, "_"));
                        DataGrid.Rows[0].Visible = false;
                        DataGrid.Rows[0].Visible = true;

                        break;
                    }
                }
            }
        }

        public override int GetPressOffsetX()
        {
            return -1;
        }

        public override bool CheckAnswer(Exercise exercise, bool andIntermediate = true)
        {
            var answer = exercise.GetAnswer();
            var len = GetRightPartLength(exercise) + ANSWER_OFFSET;
            var userAnswer = ReadNumberWithSpaces(0, len, AnswerRowIndex);
            var result = answer.Answer.ToString() == userAnswer;

            if (!result)
            {
                var x = 0;
                while (x < len && DataGrid[x, AnswerRowIndex].Value == null)
                {
                    x++;
                }
                FindAndSaveMistakes(userAnswer, answer.Answer, x, AnswerRowIndex, false);
            }

            return result;
        }

        public override bool CheckRemainder(Exercise exercise, bool andIntermediate = true)
        {
            var answer = exercise.GetAnswer();
            var xFrom = GetRightPartLength(exercise) + ANSWER_OFFSET;

            var remainderCheckResult = ReadRemainder(xFrom) == answer.Remainder.ToString();
            if (!andIntermediate)
            {
                return remainderCheckResult;
            }

            var xTo = xFrom + exercise.Number1.ToString().Length;


            var intermediates = answer.CalcIntermediate(exercise);
            for (var i = 0; i < intermediates.Count; i++)
            {
                var rowIndex = AnswerRowIndex + i;
                
                var x = xFrom;
                while (x < xTo && DataGrid[x, rowIndex].Value == null)
                {
                    x += 1;
                }
                
                var interUserNumber = ReadNumberWithSpaces(x, xTo, rowIndex);
                var interAnswerNumber = intermediates[i];
                var res = FindAndSaveMistakes(interUserNumber, interAnswerNumber, x, rowIndex, true);
                if (!res)
                {
                    return false;
                }
            }

            return remainderCheckResult;
        }
    }
}
