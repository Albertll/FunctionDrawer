namespace FunctionDrawer
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
            this.functionInputTextBox = new System.Windows.Forms.TextBox();
            this.xLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.yLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // functionInputTextBox
            // 
            this.functionInputTextBox.Location = new System.Drawing.Point(51, 6);
            this.functionInputTextBox.Name = "functionInputTextBox";
            this.functionInputTextBox.Size = new System.Drawing.Size(169, 20);
            this.functionInputTextBox.TabIndex = 0;
            this.functionInputTextBox.Text = "x";
            this.functionInputTextBox.TextChanged += new System.EventHandler(this.functionInputTextBox_TextChanged);
            // 
            // xLabel
            // 
            this.xLabel.AutoSize = true;
            this.xLabel.Location = new System.Drawing.Point(12, 39);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(38, 13);
            this.xLabel.TabIndex = 2;
            this.xLabel.Text = "xLabel";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "f(x) = ";
            // 
            // yLabel
            // 
            this.yLabel.AutoSize = true;
            this.yLabel.Location = new System.Drawing.Point(12, 62);
            this.yLabel.Name = "yLabel";
            this.yLabel.Size = new System.Drawing.Size(38, 13);
            this.yLabel.TabIndex = 7;
            this.yLabel.Text = "yLabel";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 529);
            this.Controls.Add(this.yLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.xLabel);
            this.Controls.Add(this.functionInputTextBox);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.OnMouseWheel);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox functionInputTextBox;
        private System.Windows.Forms.Label xLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label yLabel;
    }
}

