using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSLeaderboardAdderDrawer : VerticalSequenceDrawer
    {
        private readonly ILeaderboardManipulator _leaderboardManipulator;

        private TextField.TextFieldBase _leaderboardNameTextField;
        private TextField.TextFieldBase _leaderboardIdTextField;

        public HMSLeaderboardAdderDrawer(ILeaderboardManipulator leaderboardManipulator)
        {
            _leaderboardManipulator = leaderboardManipulator;
            _leaderboardNameTextField = new TextField.TextField("Name: ", "").SetLabelWidth(50);
            _leaderboardIdTextField = new TextField.TextField("ID: ", "").SetLabelWidth(30);

            SetupSequence();
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(_leaderboardNameTextField, new Space(10), _leaderboardIdTextField, new Space(15), new Button.Button("Add", OnAddLeaderboard).SetWidth(100)));
        }

        private void OnAddLeaderboard()
        {
            var name = _leaderboardNameTextField.GetCurrentText();
            var id = _leaderboardIdTextField.GetCurrentText();

            _leaderboardManipulator.AddLeaderboard(name, id);
            _leaderboardIdTextField.ClearInput();
            _leaderboardNameTextField.ClearInput();
        }
    }
}
