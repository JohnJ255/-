using Columns.Exercises;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Columns.ExcerciseView
{
    public interface PrinterInterface
    {
        void Print(Exercise currentExercise);
        void UpdateAnswer(Exercise currentExercise, Label labelAnswer, Label labelReminder);
        IEnumerable<BoldLine> GetBoldLines();
        int GetBlockedRowsCount();
        int GetPressOffsetX();
        IEnumerable<SpecialText> GetSpecialTexts(Exercise currentExercise);
    }
}
