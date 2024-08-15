using System.Windows.Forms;

namespace OrleanCAD
{
    partial class Map
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.DebugLabel = new System.Windows.Forms.Label();
            this.SearchPostalEnter = new System.Windows.Forms.TextBox();
            this.SearchPostalButton = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SearchPostalButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::OrleanCAD.Properties.Resources.Map;
            this.pictureBox.Location = new System.Drawing.Point(-1, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(666, 1024);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // DebugLabel
            // 
            this.DebugLabel.AutoSize = true;
            this.DebugLabel.Location = new System.Drawing.Point(12, 92);
            this.DebugLabel.Name = "DebugLabel";
            this.DebugLabel.Size = new System.Drawing.Size(48, 13);
            this.DebugLabel.TabIndex = 1;
            this.DebugLabel.Text = "DEBUG:";
            // 
            // SearchPostalEnter
            // 
            this.SearchPostalEnter.BackColor = System.Drawing.SystemColors.WindowFrame;
            this.SearchPostalEnter.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SearchPostalEnter.Font = new System.Drawing.Font("Gotham Pro Medium", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SearchPostalEnter.ForeColor = System.Drawing.SystemColors.Control;
            this.SearchPostalEnter.Location = new System.Drawing.Point(15, 37);
            this.SearchPostalEnter.Multiline = true;
            this.SearchPostalEnter.Name = "SearchPostalEnter";
            this.SearchPostalEnter.Size = new System.Drawing.Size(91, 12);
            this.SearchPostalEnter.TabIndex = 3;
            this.SearchPostalEnter.Text = "Введите блок...";
            this.SearchPostalEnter.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SearchPostalButton
            // 
            this.SearchPostalButton.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SearchPostalButton.Controls.Add(this.label2);
            this.SearchPostalButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SearchPostalButton.Location = new System.Drawing.Point(15, 53);
            this.SearchPostalButton.Name = "SearchPostalButton";
            this.SearchPostalButton.Size = new System.Drawing.Size(90, 26);
            this.SearchPostalButton.TabIndex = 4;
            this.SearchPostalButton.Click += new System.EventHandler(this.SearchPostalButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Gotham Pro Medium", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(22, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "ПОИСК";
            this.label2.UseWaitCursor = true;
            // 
            // Map
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(665, 1001);
            this.Controls.Add(this.SearchPostalButton);
            this.Controls.Add(this.SearchPostalEnter);
            this.Controls.Add(this.DebugLabel);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Map";
            this.Text = "Map";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.SearchPostalButton.ResumeLayout(false);
            this.SearchPostalButton.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private Label DebugLabel;
        private TextBox SearchPostalEnter;
        private Panel SearchPostalButton;
        private Label label2;
    }
}