namespace DAAD_Scraper
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            BtnStart = new Button();
            TxtSource = new TextBox();
            BtnNext = new Button();
            LblCount = new Label();
            TxtIndex = new TextBox();
            SuspendLayout();
            // 
            // BtnStart
            // 
            BtnStart.Location = new Point(12, 415);
            BtnStart.Name = "BtnStart";
            BtnStart.Size = new Size(75, 23);
            BtnStart.TabIndex = 0;
            BtnStart.Text = "Start";
            BtnStart.UseVisualStyleBackColor = true;
            BtnStart.Click += BtnStart_Click;
            // 
            // TxtSource
            // 
            TxtSource.AccessibleRole = AccessibleRole.OutlineButton;
            TxtSource.Location = new Point(12, 73);
            TxtSource.Multiline = true;
            TxtSource.Name = "TxtSource";
            TxtSource.Size = new Size(575, 336);
            TxtSource.TabIndex = 1;
            // 
            // BtnNext
            // 
            BtnNext.Location = new Point(512, 415);
            BtnNext.Name = "BtnNext";
            BtnNext.Size = new Size(75, 23);
            BtnNext.TabIndex = 3;
            BtnNext.Text = "Next";
            BtnNext.UseVisualStyleBackColor = true;
            BtnNext.Click += BtnNext_Click;
            // 
            // LblCount
            // 
            LblCount.AutoSize = true;
            LblCount.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            LblCount.Location = new Point(65, 38);
            LblCount.Name = "LblCount";
            LblCount.Size = new Size(63, 25);
            LblCount.TabIndex = 4;
            LblCount.Text = "label1";
            // 
            // TxtIndex
            // 
            TxtIndex.AccessibleRole = AccessibleRole.OutlineButton;
            TxtIndex.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            TxtIndex.Location = new Point(12, 34);
            TxtIndex.Name = "TxtIndex";
            TxtIndex.Size = new Size(47, 33);
            TxtIndex.TabIndex = 5;
            TxtIndex.KeyDown += TxtIndex_KeyDown;
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(599, 450);
            Controls.Add(TxtIndex);
            Controls.Add(LblCount);
            Controls.Add(BtnNext);
            Controls.Add(TxtSource);
            Controls.Add(BtnStart);
            Name = "FormMain";
            Text = "Form1";
            Load += FormMain_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnStart;
        private TextBox TxtSource;
        private Button BtnNext;
        private Label LblCount;
        private TextBox TxtIndex;
    }
}
