namespace DM.Forms
{
    partial class Librated
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
            this.SuspendLayout();
            // 
            // Librated
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(224, 255);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Librated";
            this.Opacity = 0.75;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "车辆击震力状态";
            this.Deactivate += new System.EventHandler(this.Librated_Deactivate);
            this.Load += new System.EventHandler(this.Librated_Load);
            this.Activated += new System.EventHandler(this.Librated_Activated);
            this.Resize += new System.EventHandler(this.Librated_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Librated_KeyDown);
            this.ResumeLayout(false);

        }

        #endregion
    }
}