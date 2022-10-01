using Columns.ExcerciseView;
using Columns.Exercises;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Columns
{
    public partial class Form1 : Form
    {
        private Exercise CurrentExercise;
        private PrinterInterface ExerciseDrawer;
        private ExerciseManager EManager;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.CellPainting += DrawBoldLineHandler;
            dataGridView1.CellPainting += DrawOperationTypeHandler;
            var f = dataGridView1.Font;
            dataGridView1.DefaultCellStyle.Font = new Font(f.Name, f.Size + 5);
            var sp = new StandartPrinter(dataGridView1);
            EManager = new ExerciseManager();

            DrawNewExcercise();
        }

        private void DrawNewExcercise()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.RowCount = 9;
            CurrentExercise = EManager.NextExercise();
            Console.WriteLine("D:" + CurrentExercise.EType.ToString());
            switch (CurrentExercise.EType) {
                case ExerciseType.Divide:
                    ExerciseDrawer = new DividePrinter(dataGridView1);
                    break;
                default:
                    ExerciseDrawer = new StandartPrinter(dataGridView1);
                    break;
            }
            ExerciseDrawer.Print(CurrentExercise);
            dataGridView1.Focus();
            ExerciseDrawer.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);

            labelReminder.Visible = CurrentExercise.EType == ExerciseType.Divide;
            label4.Visible = CurrentExercise.EType == ExerciseType.Divide;
        }

        private void DrawOperationTypeHandler(object sender, DataGridViewCellPaintingEventArgs ev)
        {
            foreach (var op in ExerciseDrawer.GetSpecialTexts(CurrentExercise)) {
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
            foreach (var bl in ExerciseDrawer.GetBoldLines())
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
            if (sc.RowIndex < ExerciseDrawer.GetBlockedRowsCount() || !char.IsDigit(e.KeyChar))
            {
                return;
            }

            sc.Value = e.KeyChar;
            ExerciseDrawer.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);
            var offsetX = ExerciseDrawer.GetPressOffsetX();
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
                    ExerciseDrawer.UpdateAnswer(CurrentExercise, labelAnswer, labelReminder);
                }
                return;
            }
            var sc = dataGridView1.SelectedCells[0];
            dataGridView1.CurrentCell = dataGridView1.Rows[sc.RowIndex].Cells[0];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var answer = CurrentExercise.GetAnswer();
            if (labelReminder.Visible)
            {
                if (answer.Remainder.ToString() == labelReminder.Text)
                {
                    labelReminder.ForeColor = Color.Green;
                }
                else
                {
                    labelReminder.ForeColor = Color.Red;
                }
            }
            if (answer.Answer.ToString() == labelAnswer.Text)
            {
                if (labelAnswer.ForeColor == Color.Green)
                {
                    button1.Text = "Проверить";
                    if (answer.Answer.ToString() == labelAnswer.Text && answer.Remainder.ToString() == labelReminder.Text)
                    {
                        //DrawNewExcercise();
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
            DrawNewExcercise();
        }
    }
}
