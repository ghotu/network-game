namespace ChatClient
{
    partial class frmLogin
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
            if ( disposing && ( components != null ) )
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
            this.lblUserName = new Proshot.UtilityLib.Label();
            this.txtUsetName = new Proshot.UtilityLib.TextBox();
            this.btnEnter = new Proshot.UtilityLib.Button();
            this.btnExit = new Proshot.UtilityLib.Button();
            this.txtPassword = new Proshot.UtilityLib.TextBox();
            this.label1 = new Proshot.UtilityLib.Label();
            this.SuspendLayout();
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.BorderWidth = 1F;
            this.lblUserName.Location = new System.Drawing.Point(4, 9);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(74, 14);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User Name :";
            // 
            // txtUsetName
            // 
            this.txtUsetName.BorderWidth = 1F;
            this.txtUsetName.FloatValue = 0D;
            this.txtUsetName.Location = new System.Drawing.Point(78, 6);
            this.txtUsetName.MaxLength = 0;
            this.txtUsetName.Name = "txtUsetName";
            this.txtUsetName.Size = new System.Drawing.Size(126, 22);
            this.txtUsetName.TabIndex = 1;
            // 
            // btnEnter
            // 
            this.btnEnter.Location = new System.Drawing.Point(156, 64);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(48, 23);
            this.btnEnter.TabIndex = 2;
            this.btnEnter.Text = "Enter";
            this.btnEnter.UseVisualStyleBackColor = true;
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(78, 64);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(48, 23);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtPassword
            // 
            this.txtPassword.BorderWidth = 1F;
            this.txtPassword.FloatValue = 0D;
            this.txtPassword.Location = new System.Drawing.Point(78, 34);
            this.txtPassword.MaxLength = 0;
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(126, 22);
            this.txtPassword.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BorderWidth = 1F;
            this.label1.Location = new System.Drawing.Point(4, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Password :";
            // 
            // frmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(225, 90);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnEnter);
            this.Controls.Add(this.txtUsetName);
            this.Controls.Add(this.lblUserName);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmLogin";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Proshot.UtilityLib.Label lblUserName;
        private Proshot.UtilityLib.TextBox txtUsetName;
        private Proshot.UtilityLib.Button btnEnter;
        private Proshot.UtilityLib.Button btnExit;
        private Proshot.UtilityLib.TextBox txtPassword;
        private Proshot.UtilityLib.Label label1;
    }
}