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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DrawerMainView));
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
            resources.ApplyResources(this._tbInputFunction, "_tbInputFunction");
            this._tbInputFunction.Name = "_tbInputFunction";
            // 
            // _bsViewModel
            // 
            this._bsViewModel.DataSource = typeof(FunctionDrawer.DrawerViewModel);
            // 
            // _lbX
            // 
            resources.ApplyResources(this._lbX, "_lbX");
            this._lbX.Name = "_lbX";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // _lbY
            // 
            resources.ApplyResources(this._lbY, "_lbY");
            this._lbY.Name = "_lbY";
            // 
            // _lbError
            // 
            resources.ApplyResources(this._lbError, "_lbError");
            this._lbError.DataBindings.Add(new System.Windows.Forms.Binding("Text", this._bsViewModel, "ErrorMessage", true));
            this._lbError.Name = "_lbError";
            // 
            // _lbS
            // 
            resources.ApplyResources(this._lbS, "_lbS");
            this._lbS.Name = "_lbS";
            // 
            // DrawerMainView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._lbS);
            this.Controls.Add(this._lbError);
            this.Controls.Add(this._lbY);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._lbX);
            this.Controls.Add(this._tbInputFunction);
            this.DoubleBuffered = true;
            this.Name = "DrawerMainView";
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

