using Bot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotCore.BotForms
{
    public partial class BotForm : Form
    {
        public BotForm(BotCore.Client Client)
        {
            InitializeComponent();
            //WizardBotThread = new WizardBotThread(Client);
        }

        private WizardBotThread WizardBotThread;

        private void pauseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            WizardBotThread.Paused = pauseCheckBox.Checked;
        }
    }
}
