namespace Dreamer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabMapMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddWayPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveWayPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddWarpPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveWarpPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.TabMapMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(322, 213);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 50);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadMapToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadMapToolStripMenuItem
            // 
            this.loadMapToolStripMenuItem.Name = "loadMapToolStripMenuItem";
            this.loadMapToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
            this.loadMapToolStripMenuItem.Text = "Load Map";
            this.loadMapToolStripMenuItem.Click += new System.EventHandler(this.LoadMapToolStripMenuItem_Click);
            // 
            // TabMapMenu
            // 
            this.TabMapMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddWayPointToolStripMenuItem,
            this.RemoveWayPointToolStripMenuItem,
            this.AddWarpPointToolStripMenuItem,
            this.RemoveWarpPointToolStripMenuItem});
            this.TabMapMenu.Name = "TabMapMenu";
            this.TabMapMenu.Size = new System.Drawing.Size(180, 92);
            this.TabMapMenu.Text = "Tab Map";
            // 
            // AddWayPointToolStripMenuItem
            // 
            this.AddWayPointToolStripMenuItem.Name = "AddWayPointToolStripMenuItem";
            this.AddWayPointToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.AddWayPointToolStripMenuItem.Text = "Add Way Point";
            this.AddWayPointToolStripMenuItem.Click += new System.EventHandler(this.AddWayPointToolStripMenuItem_Click);
            // 
            // RemoveWayPointToolStripMenuItem
            // 
            this.RemoveWayPointToolStripMenuItem.Name = "RemoveWayPointToolStripMenuItem";
            this.RemoveWayPointToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.RemoveWayPointToolStripMenuItem.Text = "Remove Way Point";
            this.RemoveWayPointToolStripMenuItem.Click += new System.EventHandler(this.RemoveWayPointToolStripMenuItem_Click);
            // 
            // AddWarpPointToolStripMenuItem
            // 
            this.AddWarpPointToolStripMenuItem.Name = "AddWarpPointToolStripMenuItem";
            this.AddWarpPointToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.AddWarpPointToolStripMenuItem.Text = "Add Warp Point";
            // 
            // RemoveWarpPointToolStripMenuItem
            // 
            this.RemoveWarpPointToolStripMenuItem.Name = "RemoveWarpPointToolStripMenuItem";
            this.RemoveWarpPointToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.RemoveWarpPointToolStripMenuItem.Text = "Remove Warp Point";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Dreamer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.TabMapMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMapToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip TabMapMenu;
        private System.Windows.Forms.ToolStripMenuItem AddWayPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AddWarpPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveWayPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RemoveWarpPointToolStripMenuItem;
    }
}

