using Columns.Exercises;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Columns.ExcerciseView
{
    public class DividePrinter : PrinterInterface
    {
        const int ANSWER_OFFSET = 2;
        private int AnswerRowIndex = 0;
        private List<BoldLine> BoldLines;
        private List<SpecialText> MinusSymbols;

        public DataGridView DataGrid { get; }

        public DividePrinter(DataGridView dataGrid)
        {
            BoldLines = new List<BoldLine>();
            MinusSymbols = new List<SpecialText>();
            DataGrid = dataGrid;
        }

        public IEnumerable<SpecialText> GetSpecialTexts(Exercise exercise)
        {
            return MinusSymbols;
        }

        public int GetBlockedRowsCount()
        {
            return 1;
        }

        public IEnumerable<BoldLine> GetBoldLines()
        {
            return BoldLines;
        }

        public void Print(Exercise exercise)
        {
            BoldLines.Clear();
            MinusSymbols.Clear();
            var len = Math.Max(exercise.Number2.ToString().Length, exercise.GetAnswer().Answer.ToString().Length);
            var diff = len - exercise.Number2.ToString().Length;
            PrintNumber(0, len + ANSWER_OFFSET, exercise.Number1);
            PrintNumber(0, diff + ANSWER_OFFSET, exercise.Number2);
            BoldLines.Add(new BoldLine(new Point(0, 0), new Point(len + ANSWER_OFFSET, 0), BoldLineType.Bottom));
            BoldLines.Add(new BoldLine(new Point(len + ANSWER_OFFSET, 0), new Point(len + ANSWER_OFFSET, 1), BoldLineType.Right));
            AnswerRowIndex = 1;
            DataGrid.CurrentCell = DataGrid.Rows[1].Cells[len + ANSWER_OFFSET - 1];
        }

        private void PrintNumber(int y, int xFrom, int number1)
        {
            var x = xFrom;
            do
            {
                var chislo = number1 % 10;
                number1 = number1 / 10;
                DataGrid[x++, y].Value = chislo;
            } while (number1 > 0 && x < DataGrid.ColumnCount);
        }

        public void UpdateAnswer(Exercise exercise, Label labelAnswer, Label labelReminder)
        {
            var len = Math.Max(exercise.Number2.ToString().Length, exercise.GetAnswer().Answer.ToString().Length);
            var answer = "";
            var reminder = "";
            for (int i = 0; i < len + ANSWER_OFFSET; i++)
            {
                if (DataGrid[i, AnswerRowIndex].Value == null) {
                    continue;
                }
                answer = DataGrid[i, AnswerRowIndex].Value.ToString() + answer;
            }
            var isSpace = true;
            var lastRow = 0;
            var x = len + ANSWER_OFFSET;
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
                if (!isSpace && DataGrid[x, j].Value == null) {
                    lastRow = j - 1;
                    break;
                }
            }
            if (lastRow > 0)
            {
                for (int i = x ; i < DataGrid.ColumnCount && DataGrid[i, lastRow].Value != null; i++)
                {
                    reminder = DataGrid[i, lastRow].Value.ToString() + reminder;
                }
            }
            if (answer == "")
            {
                answer = "0";
            }
            if (reminder == "")
            {
                reminder = "0";
            }

            labelAnswer.Text = answer.TrimStart(new[] { '0' });
            labelAnswer.ForeColor = Color.Black;

            labelReminder.Text = reminder.TrimStart(new[] { '0' });
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

            var len = Math.Max(exercise.Number2.ToString().Length, exercise.GetAnswer().Answer.ToString().Length);
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
                        var ki = i + numberLen + ANSWER_OFFSET;
                        while (ki + 1 < DataGrid.ColumnCount && DataGrid[ki + 1 - ANSWER_OFFSET, j - 1].Value != null)
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

        public int GetPressOffsetX()
        {
            return -1;
        }
    }
}
