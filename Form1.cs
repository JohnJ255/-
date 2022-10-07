using Columns.ExcerciseGrid;
using Columns.Exercises;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Columns
{
    public partial class Form1 : Form
    {
        private Exercise CurrentExercise;
        private GridderInterface ExerciseGridder;
        private ExerciseManager EManager;
        private int StopAfterCountExercises = 0;
        private string RunProcessAfterSuccessFinish = "";

        public Form1()
        {
            InitializeComponent();
            EManager = new ExerciseManager();
            var args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                var argParts = args[i].Split('=');
                if (argParts.Length < 2)
                {
                    continue;
                }
                var cmd = argParts[0].Trim().TrimStart(new [] {'-'});
                var value = argParts[1].TrimStart(new [] {' ', '"', '\''}).TrimEnd(new[] { ' ', '"', '\'' });
                if (cmd == "n")
                {
                    StopAfterCountExercises = Convert.ToInt32(value);
                }
                if (cmd == "run")
                {
                    RunProcessAfterSuccessFinish = value;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelCount.Visible = StopAfterCountExercises > 0;

            dataGridView1.CellPainting += DrawBoldLineHandler;
            dataGridView1.CellPainting += DrawOperationTypeHandler;
            var f = dataGridView1.Font;
            dataGridView1.DefaultCellStyle.Font = new Font(f.Name, f.Size + 5);

            DrawNewExcercise();
        }

        private void DrawNewExcercise()
        {
            if (StopAfterCountExercises > 0)
            {
                labelCount.Text = (EManager.ExercisesCount + 1) + "/" + StopAfterCountExercises;

                if (EManager.ExercisesCount == StopAfterCountExercises)
                {
                    Close();
                    if (!String.IsNullOrEmpty(RunProcessAfterSuccessFinish))
                    {
                        var procInfo = new ProcessStartInfo(RunProcessAfterSuccessFinish);
                        procInfo.UseShellExecute = true;
                        Process.Start(procInfo);
                    }
                    return;
                }
            }
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = 9;
            CurrentExercise = EManager.NextExercise();

            switch (CurrentExercise.EType) {
                case ExerciseType.Divide:
                    ExerciseGridder = new DivideGridder(dataGridView1);
                    break;
                default:
                    ExerciseGridder = new StandartGridder(dataGridView1);
                    break;
            }
            ExerciseGridder.Print(CurrentExercise);
            dataGridView1.Focus();
            ExerciseGridder.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);

            labelReminder.Visible = CurrentExercise.EType == ExerciseType.Divide;
            label4.Visible = CurrentExercise.EType == ExerciseType.Divide;
        }

        private void DrawOperationTypeHandler(object sender, DataGridViewCellPaintingEventArgs ev)
        {
            foreach (var op in ExerciseGridder.GetSpecialTexts(CurrentExercise)) {
                if (ev.ColumnIndex != op.X || ev.RowIndex != op.Y)
                {
                    continue;
                }
                var rx = ev.CellBounds;
                var p = new Point(rx.Left + op.Offset.X, rx.Top + op.Offset.Y);
                TextRenderer.DrawText(ev.Graphics, op.Text, ev.CellStyle.Font, p, ev.CellStyle.ForeColor);
            }
        }

        private void DrawBoldLineHandler(object sender, DataGridViewCellPaintingEventArgs ev)
        {
            foreach (var bl in ExerciseGridder.GetBoldLines())
            {
                if (ev.ColumnIndex <= bl.To.X && ev.RowIndex <= bl.To.Y && ev.ColumnIndex >= bl.From.X && ev.RowIndex >= bl.From.Y)
                {
                    ev.Handled = true;
                    ev.Paint(ev.CellBounds, DataGridViewPaintParts.All);
                    var r = ev.CellBounds;
                    using (var pen = new Pen(Color.Black))
                    {
                        pen.Width = 2;
                        pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;

                        switch (bl.BLType)
                        {
                            case BoldLineType.Bottom:
                                ev.Graphics.DrawLine(pen, r.Left, r.Bottom, r.Right, r.Bottom);
                                break;
                            case BoldLineType.Left:
                                ev.Graphics.DrawLine(pen, r.Left, r.Top, r.Left, r.Bottom);
                                break;
                            case BoldLineType.Top:
                                ev.Graphics.DrawLine(pen, r.Left, r.Top, r.Right, r.Top);
                                break;
                            case BoldLineType.Right:
                                ev.Graphics.DrawLine(pen, r.Right, r.Top, r.Right, r.Bottom);
                                break;
                        }
                    }
                }
            }
        }

        private void dataGridView1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }
            var sc = dataGridView1.SelectedCells[0];
            if (sc.RowIndex < ExerciseGridder.GetBlockedRowsCount() || !char.IsDigit(e.KeyChar))
            {
                return;
            }

            sc.Value = e.KeyChar;
            ExerciseGridder.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);
            var offsetX = ExerciseGridder.GetPressOffsetX();
            if (sc.ColumnIndex + offsetX < dataGridView1.ColumnCount && sc.ColumnIndex + offsetX >= 0)
            {
               dataGridView1.CurrentCell = dataGridView1.Rows[sc.RowIndex].Cells[sc.ColumnIndex + offsetX];
            }
            if (dataGridView1.CurrentCell.RowIndex >= dataGridView1.RowCount - 2)
            {
                dataGridView1.RowCount = dataGridView1.RowCount + 1;
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 0)
            {
                return;
            } 
            if (e.KeyCode != Keys.Enter)
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    dataGridView1.SelectedCells[0].Value = null;
                    ExerciseGridder.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);
                }
                return;
            }
            var sc = dataGridView1.SelectedCells[0];
            dataGridView1.CurrentCell = dataGridView1.Rows[sc.RowIndex].Cells[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExerciseGridder.ForgetMistakes();
            var answerCheckResult = ExerciseGridder.CheckAnswer(CurrentExercise);
            var remainderCheckResult = ExerciseGridder.CheckRemainder(CurrentExercise);
            if (!answerCheckResult || !remainderCheckResult)
            {
                ExerciseGridder.ShowMistakes(Color.Red);
            }

            if (labelReminder.Visible)
            {
                if (remainderCheckResult)
                {
                    labelReminder.ForeColor = Color.Green;
                }
                else
                {
                    labelReminder.ForeColor = Color.Red;
                }
            }
            if (answerCheckResult)
            {
                if (labelAnswer.ForeColor == Color.Green)
                {
                    button1.Text = "Проверить";
                    if (answerCheckResult && remainderCheckResult)
                    {
                        DrawNewExcercise();
                    }
                }
                else
                {
                    button1.Text = "Далее";
                    labelAnswer.ForeColor = Color.Green;
                }
            }
            else
            {
                labelAnswer.ForeColor = Color.Red;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        }
    }
}
