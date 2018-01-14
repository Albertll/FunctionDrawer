namespace FunctionDrawer
{
    partial class DrawerMainView
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
            this._tbInputFunction = new System.Windows.Forms.TextBox();
            this._bsViewModel = new System.Windows.Forms.BindingSource(this.components);
            this._lbX = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._lbY = new System.Windows.Forms.Label();
            this._lbError = new System.Windows.Forms.Label();
            this._lbS = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._bsViewModel)).BeginInit();
            this.SuspendLayout();
            // 
            // _tbInputFunction
            // 
            this._tbInputFunction.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bsViewModel, "Function", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this._tbInputFunction.Location = new System.Drawing.Point(51, 6);
            this._tbInputFunction.Name = "_tbInputFunction";
            this._tbInputFunction.Size = new System.Drawing.Size(169, 20);
            this._tbInputFunction.TabIndex = 0;
            this._tbInputFunction.Text = "x";
            // 
            // _bsViewModel
            // 
            this._bsViewModel.DataSource = typeof(FunctionDrawer.DrawerViewModel);
            // 
            // _lbX
            // 
            this._lbX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lbX.AutoSize = true;
            this._lbX.Location = new System.Drawing.Point(716, 441);
            this._lbX.Name = "_lbX";
            this._lbX.Size = new System.Drawing.Size(38, 13);
            this._lbX.TabIndex = 2;
            this._lbX.Text = "xLabel";
            this._lbX.TextAlign = System.Drawing.ContentAlignment.BottomRight;
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
            // _lbY
            // 
            this._lbY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lbY.AutoSize = true;
            this._lbY.Location = new System.Drawing.Point(716, 468);
            this._lbY.Name = "_lbY";
            this._lbY.Size = new System.Drawing.Size(38, 13);
            this._lbY.TabIndex = 7;
            this._lbY.Text = "yLabel";
            this._lbY.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // _lbError
            // 
            this._lbError.AutoSize = true;
            this._lbError.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bsViewModel, "ErrorMessage", true));
            this._lbError.Location = new System.Drawing.Point(12, 41);
            this._lbError.Name = "_lbError";
            this._lbError.Size = new System.Drawing.Size(77, 13);
            this._lbError.TabIndex = 8;
            this._lbError.Text = "Label for errors";
            // 
            // _lbS
            // 
            this._lbS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._lbS.AutoSize = true;
            this._lbS.Location = new System.Drawing.Point(716, 496);
            this._lbS.Name = "_lbS";
            this._lbS.Size = new System.Drawing.Size(38, 13);
            this._lbS.TabIndex = 9;
            this._lbS.Text = "sLabel";
            this._lbS.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // DrawerMainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 529);
            this.Controls.Add(this._lbS);
            this.Controls.Add(this._lbError);
            this.Controls.Add(this._lbY);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._lbX);
            this.Controls.Add(this._tbInputFunction);
            this.DoubleBuffered = true;
            this.Name = "DrawerMainView";
            this.Text = "Function Drawer";
            ((System.ComponentModel.ISupportInitialize)(this._bsViewModel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _tbInputFunction;
        private System.Windows.Forms.Label _lbX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lbY;
        private System.Windows.Forms.BindingSource _bsViewModel;
        private System.Windows.Forms.Label _lbError;
        private System.Windows.Forms.Label _lbS;
    }
}

