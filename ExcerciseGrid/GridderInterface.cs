using Columns.Exercises;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Columns.ExcerciseGrid
{
    public interface GridderInterface
    {
        void Print(Exercise currentExercise);
        void UpdateAnswer(Exercise currentExercise, Label labelAnswer, Label labelReminder);
        IEnumerable<BoldLine> GetBoldLines();
        int GetBlockedRowsCount();
        int GetPressOffsetX();
        IEnumerable<SpecialText> GetSpecialTexts(Exercise currentExercise);
        bool CheckAnswer(Exercise currentExercise, bool andIntermediate = true);
        bool CheckRemainder(Exercise currentExercise, bool andIntermediate = true);
        void ShowMistakes(Color red);
        void ForgetMistakes();
    }
}
