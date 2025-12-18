namespace TestLauncher
{
    partial class KeyEntry
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
            this.txtKeyString = new System.Windows.Forms.TextBox();
            this.lblKeyString = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtKeyString
            // 
            this.txtKeyString.Location = new System.Drawing.Point(50, 13);
            this.txtKeyString.Margin = new System.Windows.Forms.Padding(4);
            this.txtKeyString.Name = "txtKeyString";
            this.txtKeyString.Size = new System.Drawing.Size(229, 22);
            this.txtKeyString.TabIndex = 2;
            // 
            // lblKeyString
            // 
            this.lblKeyString.AutoSize = true;
            this.lblKeyString.Location = new System.Drawing.Point(12, 16);
            this.lblKeyString.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblKeyString.Name = "lblKeyString";
            this.lblKeyString.Size = new System.Drawing.Size(30, 16);
            this.lblKeyString.TabIndex = 5;
            this.lblKeyString.Text = "Key";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(85, 43);
            this.btnSubmit.Margin = new System.Windows.Forms.Padding(4);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(123, 25);
            this.btnSubmit.TabIndex = 6;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // KeyEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(296, 77);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.lblKeyString);
            this.Controls.Add(this.txtKeyString);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "KeyEntry";
            this.Text = "Key Entry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtKeyString;
        private System.Windows.Forms.Label lblKeyString;
        private System.Windows.Forms.Button btnSubmit;
    }
}

