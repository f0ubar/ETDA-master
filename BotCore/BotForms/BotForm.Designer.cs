namespace BotCore.BotForms
{
    partial class BotForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.wizardTabPage = new System.Windows.Forms.TabPage();
            this.targetingGroupBox = new System.Windows.Forms.GroupBox();
            this.spriteListGroupBox = new System.Windows.Forms.GroupBox();
            this.hostileSpriteListGroupBox = new System.Windows.Forms.GroupBox();
            this.hostileSpriteListRemoveButton = new System.Windows.Forms.Button();
            this.hostileSpriteListAddTextBox = new System.Windows.Forms.TextBox();
            this.hostileSpriteListAddButton = new System.Windows.Forms.Button();
            this.hostileSpriteListBox = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.targetExplicitSpriteList = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.targetOnlyHostileCheckbox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.targetHostileFirstCheckbox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.targetWithoutPathCheckbox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.maximumDistance = new System.Windows.Forms.NumericUpDown();
            this.pauseCheckBox = new System.Windows.Forms.CheckBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.nonHostileSpriteListGroupBox = new System.Windows.Forms.GroupBox();
            this.nonHostileSpriteListRemoveButton = new System.Windows.Forms.Button();
            this.nonHostileSpriteListAddTextBox = new System.Windows.Forms.TextBox();
            this.nonHostileSpriteListAddButton = new System.Windows.Forms.Button();
            this.nonHostileSpriteListBox = new System.Windows.Forms.ListBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tabControl1.SuspendLayout();
            this.wizardTabPage.SuspendLayout();
            this.targetingGroupBox.SuspendLayout();
            this.spriteListGroupBox.SuspendLayout();
            this.hostileSpriteListGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maximumDistance)).BeginInit();
            this.nonHostileSpriteListGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.wizardTabPage);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 426);
            this.tabControl1.TabIndex = 0;
            // 
            // wizardTabPage
            // 
            this.wizardTabPage.Controls.Add(this.targetingGroupBox);
            this.wizardTabPage.Controls.Add(this.pauseCheckBox);
            this.wizardTabPage.Location = new System.Drawing.Point(4, 22);
            this.wizardTabPage.Name = "wizardTabPage";
            this.wizardTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.wizardTabPage.Size = new System.Drawing.Size(768, 400);
            this.wizardTabPage.TabIndex = 0;
            this.wizardTabPage.Text = "Wizard";
            this.wizardTabPage.UseVisualStyleBackColor = true;
            // 
            // targetingGroupBox
            // 
            this.targetingGroupBox.Controls.Add(this.label6);
            this.targetingGroupBox.Controls.Add(this.checkBox1);
            this.targetingGroupBox.Controls.Add(this.spriteListGroupBox);
            this.targetingGroupBox.Controls.Add(this.label5);
            this.targetingGroupBox.Controls.Add(this.targetExplicitSpriteList);
            this.targetingGroupBox.Controls.Add(this.label4);
            this.targetingGroupBox.Controls.Add(this.targetOnlyHostileCheckbox);
            this.targetingGroupBox.Controls.Add(this.label3);
            this.targetingGroupBox.Controls.Add(this.targetHostileFirstCheckbox);
            this.targetingGroupBox.Controls.Add(this.label2);
            this.targetingGroupBox.Controls.Add(this.targetWithoutPathCheckbox);
            this.targetingGroupBox.Controls.Add(this.label1);
            this.targetingGroupBox.Controls.Add(this.maximumDistance);
            this.targetingGroupBox.Location = new System.Drawing.Point(6, 29);
            this.targetingGroupBox.Name = "targetingGroupBox";
            this.targetingGroupBox.Size = new System.Drawing.Size(557, 207);
            this.targetingGroupBox.TabIndex = 1;
            this.targetingGroupBox.TabStop = false;
            this.targetingGroupBox.Text = "Targeting";
            // 
            // spriteListGroupBox
            // 
            this.spriteListGroupBox.Controls.Add(this.nonHostileSpriteListGroupBox);
            this.spriteListGroupBox.Controls.Add(this.hostileSpriteListGroupBox);
            this.spriteListGroupBox.Location = new System.Drawing.Point(238, 13);
            this.spriteListGroupBox.Name = "spriteListGroupBox";
            this.spriteListGroupBox.Size = new System.Drawing.Size(310, 186);
            this.spriteListGroupBox.TabIndex = 11;
            this.spriteListGroupBox.TabStop = false;
            this.spriteListGroupBox.Text = "Sprite List";
            // 
            // hostileSpriteListGroupBox
            // 
            this.hostileSpriteListGroupBox.Controls.Add(this.hostileSpriteListRemoveButton);
            this.hostileSpriteListGroupBox.Controls.Add(this.hostileSpriteListAddTextBox);
            this.hostileSpriteListGroupBox.Controls.Add(this.hostileSpriteListAddButton);
            this.hostileSpriteListGroupBox.Controls.Add(this.hostileSpriteListBox);
            this.hostileSpriteListGroupBox.Location = new System.Drawing.Point(7, 19);
            this.hostileSpriteListGroupBox.Name = "hostileSpriteListGroupBox";
            this.hostileSpriteListGroupBox.Size = new System.Drawing.Size(145, 160);
            this.hostileSpriteListGroupBox.TabIndex = 0;
            this.hostileSpriteListGroupBox.TabStop = false;
            this.hostileSpriteListGroupBox.Text = "Hostile";
            // 
            // hostileSpriteListRemoveButton
            // 
            this.hostileSpriteListRemoveButton.Location = new System.Drawing.Point(6, 19);
            this.hostileSpriteListRemoveButton.Name = "hostileSpriteListRemoveButton";
            this.hostileSpriteListRemoveButton.Size = new System.Drawing.Size(132, 23);
            this.hostileSpriteListRemoveButton.TabIndex = 2;
            this.hostileSpriteListRemoveButton.Text = "Remove";
            this.hostileSpriteListRemoveButton.UseVisualStyleBackColor = true;
            // 
            // hostileSpriteListAddTextBox
            // 
            this.hostileSpriteListAddTextBox.Location = new System.Drawing.Point(6, 133);
            this.hostileSpriteListAddTextBox.Name = "hostileSpriteListAddTextBox";
            this.hostileSpriteListAddTextBox.Size = new System.Drawing.Size(51, 20);
            this.hostileSpriteListAddTextBox.TabIndex = 1;
            // 
            // hostileSpriteListAddButton
            // 
            this.hostileSpriteListAddButton.Location = new System.Drawing.Point(63, 131);
            this.hostileSpriteListAddButton.Name = "hostileSpriteListAddButton";
            this.hostileSpriteListAddButton.Size = new System.Drawing.Size(75, 23);
            this.hostileSpriteListAddButton.TabIndex = 1;
            this.hostileSpriteListAddButton.Text = "button1";
            this.hostileSpriteListAddButton.UseVisualStyleBackColor = true;
            // 
            // hostileSpriteListBox
            // 
            this.hostileSpriteListBox.FormattingEnabled = true;
            this.hostileSpriteListBox.Location = new System.Drawing.Point(6, 47);
            this.hostileSpriteListBox.Name = "hostileSpriteListBox";
            this.hostileSpriteListBox.Size = new System.Drawing.Size(132, 82);
            this.hostileSpriteListBox.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Target Explicit Sprite List";
            // 
            // targetExplicitSpriteList
            // 
            this.targetExplicitSpriteList.AutoSize = true;
            this.targetExplicitSpriteList.Location = new System.Drawing.Point(139, 105);
            this.targetExplicitSpriteList.Name = "targetExplicitSpriteList";
            this.targetExplicitSpriteList.Size = new System.Drawing.Size(15, 14);
            this.targetExplicitSpriteList.TabIndex = 9;
            this.targetExplicitSpriteList.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(35, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Target Only Hostile";
            // 
            // targetOnlyHostileCheckbox
            // 
            this.targetOnlyHostileCheckbox.AutoSize = true;
            this.targetOnlyHostileCheckbox.Location = new System.Drawing.Point(139, 85);
            this.targetOnlyHostileCheckbox.Name = "targetOnlyHostileCheckbox";
            this.targetOnlyHostileCheckbox.Size = new System.Drawing.Size(15, 14);
            this.targetOnlyHostileCheckbox.TabIndex = 7;
            this.targetOnlyHostileCheckbox.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(37, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Target Hostile First";
            // 
            // targetHostileFirstCheckbox
            // 
            this.targetHostileFirstCheckbox.AutoSize = true;
            this.targetHostileFirstCheckbox.Location = new System.Drawing.Point(139, 65);
            this.targetHostileFirstCheckbox.Name = "targetHostileFirstCheckbox";
            this.targetHostileFirstCheckbox.Size = new System.Drawing.Size(15, 14);
            this.targetHostileFirstCheckbox.TabIndex = 4;
            this.targetHostileFirstCheckbox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Target Without Path";
            // 
            // targetWithoutPathCheckbox
            // 
            this.targetWithoutPathCheckbox.AutoSize = true;
            this.targetWithoutPathCheckbox.Location = new System.Drawing.Point(139, 45);
            this.targetWithoutPathCheckbox.Name = "targetWithoutPathCheckbox";
            this.targetWithoutPathCheckbox.Size = new System.Drawing.Size(15, 14);
            this.targetWithoutPathCheckbox.TabIndex = 2;
            this.targetWithoutPathCheckbox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Maximum Distance";
            // 
            // maximumDistance
            // 
            this.maximumDistance.Location = new System.Drawing.Point(139, 19);
            this.maximumDistance.Name = "maximumDistance";
            this.maximumDistance.Size = new System.Drawing.Size(94, 20);
            this.maximumDistance.TabIndex = 0;
            // 
            // pauseCheckBox
            // 
            this.pauseCheckBox.AutoSize = true;
            this.pauseCheckBox.Location = new System.Drawing.Point(6, 6);
            this.pauseCheckBox.Name = "pauseCheckBox";
            this.pauseCheckBox.Size = new System.Drawing.Size(56, 17);
            this.pauseCheckBox.TabIndex = 0;
            this.pauseCheckBox.Text = "Pause";
            this.pauseCheckBox.UseVisualStyleBackColor = true;
            this.pauseCheckBox.CheckedChanged += new System.EventHandler(this.pauseCheckBox_CheckedChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1068, 400);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // nonHostileSpriteListGroupBox
            // 
            this.nonHostileSpriteListGroupBox.Controls.Add(this.nonHostileSpriteListRemoveButton);
            this.nonHostileSpriteListGroupBox.Controls.Add(this.nonHostileSpriteListAddTextBox);
            this.nonHostileSpriteListGroupBox.Controls.Add(this.nonHostileSpriteListAddButton);
            this.nonHostileSpriteListGroupBox.Controls.Add(this.nonHostileSpriteListBox);
            this.nonHostileSpriteListGroupBox.Location = new System.Drawing.Point(158, 19);
            this.nonHostileSpriteListGroupBox.Name = "nonHostileSpriteListGroupBox";
            this.nonHostileSpriteListGroupBox.Size = new System.Drawing.Size(145, 160);
            this.nonHostileSpriteListGroupBox.TabIndex = 3;
            this.nonHostileSpriteListGroupBox.TabStop = false;
            this.nonHostileSpriteListGroupBox.Text = "Non-Hostile";
            // 
            // nonHostileSpriteListRemoveButton
            // 
            this.nonHostileSpriteListRemoveButton.Location = new System.Drawing.Point(6, 19);
            this.nonHostileSpriteListRemoveButton.Name = "nonHostileSpriteListRemoveButton";
            this.nonHostileSpriteListRemoveButton.Size = new System.Drawing.Size(132, 23);
            this.nonHostileSpriteListRemoveButton.TabIndex = 2;
            this.nonHostileSpriteListRemoveButton.Text = "Remove";
            this.nonHostileSpriteListRemoveButton.UseVisualStyleBackColor = true;
            // 
            // nonHostileSpriteListAddTextBox
            // 
            this.nonHostileSpriteListAddTextBox.Location = new System.Drawing.Point(6, 133);
            this.nonHostileSpriteListAddTextBox.Name = "nonHostileSpriteListAddTextBox";
            this.nonHostileSpriteListAddTextBox.Size = new System.Drawing.Size(51, 20);
            this.nonHostileSpriteListAddTextBox.TabIndex = 1;
            // 
            // nonHostileSpriteListAddButton
            // 
            this.nonHostileSpriteListAddButton.Location = new System.Drawing.Point(63, 131);
            this.nonHostileSpriteListAddButton.Name = "nonHostileSpriteListAddButton";
            this.nonHostileSpriteListAddButton.Size = new System.Drawing.Size(75, 23);
            this.nonHostileSpriteListAddButton.TabIndex = 1;
            this.nonHostileSpriteListAddButton.Text = "button1";
            this.nonHostileSpriteListAddButton.UseVisualStyleBackColor = true;
            // 
            // nonHostileSpriteListBox
            // 
            this.nonHostileSpriteListBox.FormattingEnabled = true;
            this.nonHostileSpriteListBox.Location = new System.Drawing.Point(6, 47);
            this.nonHostileSpriteListBox.Name = "nonHostileSpriteListBox";
            this.nonHostileSpriteListBox.Size = new System.Drawing.Size(132, 82);
            this.nonHostileSpriteListBox.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(139, 125);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 12;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(126, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Enable Switch To Hostile";
            // 
            // BotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 450);
            this.Controls.Add(this.tabControl1);
            this.Name = "BotForm";
            this.Text = "BotForm";
            this.tabControl1.ResumeLayout(false);
            this.wizardTabPage.ResumeLayout(false);
            this.wizardTabPage.PerformLayout();
            this.targetingGroupBox.ResumeLayout(false);
            this.targetingGroupBox.PerformLayout();
            this.spriteListGroupBox.ResumeLayout(false);
            this.hostileSpriteListGroupBox.ResumeLayout(false);
            this.hostileSpriteListGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maximumDistance)).EndInit();
            this.nonHostileSpriteListGroupBox.ResumeLayout(false);
            this.nonHostileSpriteListGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage wizardTabPage;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox pauseCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox targetingGroupBox;
        private System.Windows.Forms.NumericUpDown maximumDistance;
        private System.Windows.Forms.CheckBox targetWithoutPathCheckbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox targetHostileFirstCheckbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox targetOnlyHostileCheckbox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox targetExplicitSpriteList;
        private System.Windows.Forms.GroupBox spriteListGroupBox;
        private System.Windows.Forms.GroupBox hostileSpriteListGroupBox;
        private System.Windows.Forms.Button hostileSpriteListRemoveButton;
        private System.Windows.Forms.TextBox hostileSpriteListAddTextBox;
        private System.Windows.Forms.Button hostileSpriteListAddButton;
        private System.Windows.Forms.ListBox hostileSpriteListBox;
        private System.Windows.Forms.GroupBox nonHostileSpriteListGroupBox;
        private System.Windows.Forms.Button nonHostileSpriteListRemoveButton;
        private System.Windows.Forms.TextBox nonHostileSpriteListAddTextBox;
        private System.Windows.Forms.Button nonHostileSpriteListAddButton;
        private System.Windows.Forms.ListBox nonHostileSpriteListBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}