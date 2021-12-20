using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public class HMSAchievementsAdderDrawer : VerticalSequenceDrawer
    {
        private readonly IAchievementsManipulator _achievementsManipulator;

        private TextField.TextFieldBase _achievementNameTextField;
        private TextField.TextFieldBase _achievementIdTextField;

        public HMSAchievementsAdderDrawer(IAchievementsManipulator achievementsManipulator)
        {
            _achievementsManipulator = achievementsManipulator;
            _achievementNameTextField = new TextField.TextField("Name: ", "").SetLabelWidth(50);
            _achievementIdTextField = new TextField.TextField("ID: ", "").SetLabelWidth(30);

            SetupSequence();
        }

        private void SetupSequence()
        {
            AddDrawer(new HorizontalSequenceDrawer(_achievementNameTextField, new Space(10), _achievementIdTextField, new Space(15), new Button.Button("Add", OnAddAchievement).SetWidth(100)));
        }

        private void OnAddAchievement()
        {
            var name = _achievementNameTextField.GetCurrentText();
            var id = _achievementIdTextField.GetCurrentText();

            _achievementsManipulator.AddAchievement(name, id);
            _achievementNameTextField.ClearInput();
            _achievementIdTextField.ClearInput();
        }

    }
}
