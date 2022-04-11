namespace CobView
{
	partial class Configuration
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnFileRoot = new System.Windows.Forms.Button();
			this.txtSrcPath = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.txtFtpPassword = new System.Windows.Forms.TextBox();
			this.txtFtpUser = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.txtOssGuardianPath = new System.Windows.Forms.TextBox();
			this.txtFtpCmd = new System.Windows.Forms.TextBox();
			this.txtPort = new System.Windows.Forms.TextBox();
			this.txtTandemName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.lblVolError = new System.Windows.Forms.Label();
			this.btnAddVolume = new System.Windows.Forms.Button();
			this.txtNewVolume = new System.Windows.Forms.TextBox();
			this.lstVolumes = new System.Windows.Forms.ListBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnFileRoot);
			this.groupBox1.Controls.Add(this.txtSrcPath);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(13, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(349, 66);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Local Files";
			// 
			// btnFileRoot
			// 
			this.btnFileRoot.Location = new System.Drawing.Point(305, 21);
			this.btnFileRoot.Name = "btnFileRoot";
			this.btnFileRoot.Size = new System.Drawing.Size(37, 23);
			this.btnFileRoot.TabIndex = 2;
			this.btnFileRoot.Text = "...";
			this.btnFileRoot.UseVisualStyleBackColor = true;
			this.btnFileRoot.Click += new System.EventHandler(this.btnFileRoot_Click);
			// 
			// txtSrcPath
			// 
			this.txtSrcPath.Location = new System.Drawing.Point(78, 22);
			this.txtSrcPath.Name = "txtSrcPath";
			this.txtSrcPath.Size = new System.Drawing.Size(221, 22);
			this.txtSrcPath.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "File Root";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.txtFtpPassword);
			this.groupBox2.Controls.Add(this.txtFtpUser);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.txtOssGuardianPath);
			this.groupBox2.Controls.Add(this.txtFtpCmd);
			this.groupBox2.Controls.Add(this.txtPort);
			this.groupBox2.Controls.Add(this.txtTandemName);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(13, 85);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(349, 214);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Tandem";
			// 
			// txtFtpPassword
			// 
			this.txtFtpPassword.Location = new System.Drawing.Point(142, 83);
			this.txtFtpPassword.Name = "txtFtpPassword";
			this.txtFtpPassword.Size = new System.Drawing.Size(198, 22);
			this.txtFtpPassword.TabIndex = 12;
			// 
			// txtFtpUser
			// 
			this.txtFtpUser.Location = new System.Drawing.Point(142, 54);
			this.txtFtpUser.Name = "txtFtpUser";
			this.txtFtpUser.Size = new System.Drawing.Size(198, 22);
			this.txtFtpUser.TabIndex = 11;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(37, 86);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(99, 17);
			this.label7.TabIndex = 10;
			this.label7.Text = "FTP Password";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(68, 57);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(68, 17);
			this.label6.TabIndex = 9;
			this.label6.Text = "FTP User";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtOssGuardianPath
			// 
			this.txtOssGuardianPath.Location = new System.Drawing.Point(142, 170);
			this.txtOssGuardianPath.Name = "txtOssGuardianPath";
			this.txtOssGuardianPath.Size = new System.Drawing.Size(65, 22);
			this.txtOssGuardianPath.TabIndex = 8;
			// 
			// txtFtpCmd
			// 
			this.txtFtpCmd.Location = new System.Drawing.Point(142, 141);
			this.txtFtpCmd.Name = "txtFtpCmd";
			this.txtFtpCmd.Size = new System.Drawing.Size(199, 22);
			this.txtFtpCmd.TabIndex = 7;
			// 
			// txtPort
			// 
			this.txtPort.Location = new System.Drawing.Point(142, 112);
			this.txtPort.Name = "txtPort";
			this.txtPort.Size = new System.Drawing.Size(44, 22);
			this.txtPort.TabIndex = 6;
			// 
			// txtTandemName
			// 
			this.txtTandemName.Location = new System.Drawing.Point(142, 25);
			this.txtTandemName.Name = "txtTandemName";
			this.txtTandemName.Size = new System.Drawing.Size(199, 22);
			this.txtTandemName.TabIndex = 5;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(3, 173);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(133, 17);
			this.label5.TabIndex = 4;
			this.label5.Text = "OSS Guardian Path";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(35, 144);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 17);
			this.label4.TabIndex = 3;
			this.label4.Text = "FTP Command";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(72, 115);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 17);
			this.label3.TabIndex = 2;
			this.label3.Text = "FTP Port";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(58, 28);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78, 17);
			this.label2.TabIndex = 1;
			this.label2.Text = "DNS Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(74, 443);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(220, 443);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.lblVolError);
			this.groupBox3.Controls.Add(this.btnAddVolume);
			this.groupBox3.Controls.Add(this.txtNewVolume);
			this.groupBox3.Controls.Add(this.lstVolumes);
			this.groupBox3.Location = new System.Drawing.Point(13, 306);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(349, 117);
			this.groupBox3.TabIndex = 4;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Source Volume/Sub Vols";
			// 
			// lblVolError
			// 
			this.lblVolError.AutoSize = true;
			this.lblVolError.ForeColor = System.Drawing.Color.Red;
			this.lblVolError.Location = new System.Drawing.Point(154, 50);
			this.lblVolError.Name = "lblVolError";
			this.lblVolError.Size = new System.Drawing.Size(46, 17);
			this.lblVolError.TabIndex = 3;
			this.lblVolError.Text = "label8";
			this.lblVolError.Visible = false;
			// 
			// btnAddVolume
			// 
			this.btnAddVolume.Location = new System.Drawing.Point(294, 20);
			this.btnAddVolume.Name = "btnAddVolume";
			this.btnAddVolume.Size = new System.Drawing.Size(46, 23);
			this.btnAddVolume.TabIndex = 2;
			this.btnAddVolume.Text = "Add";
			this.btnAddVolume.UseVisualStyleBackColor = true;
			this.btnAddVolume.Click += new System.EventHandler(this.btnAddVolume_Click);
			// 
			// txtNewVolume
			// 
			this.txtNewVolume.Location = new System.Drawing.Point(153, 21);
			this.txtNewVolume.Name = "txtNewVolume";
			this.txtNewVolume.Size = new System.Drawing.Size(135, 22);
			this.txtNewVolume.TabIndex = 1;
			this.txtNewVolume.Text = "$D10.SOURCE";
			// 
			// lstVolumes
			// 
			this.lstVolumes.FormattingEnabled = true;
			this.lstVolumes.ItemHeight = 16;
			this.lstVolumes.Location = new System.Drawing.Point(10, 21);
			this.lstVolumes.Name = "lstVolumes";
			this.lstVolumes.Size = new System.Drawing.Size(137, 84);
			this.lstVolumes.TabIndex = 0;
			this.lstVolumes.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstVolumes_KeyDown);
			// 
			// Configuration
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(386, 478);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Configuration";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Configuration";
			this.Load += new System.EventHandler(this.Configuration_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnFileRoot;
		private System.Windows.Forms.TextBox txtSrcPath;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtOssGuardianPath;
		private System.Windows.Forms.TextBox txtFtpCmd;
		private System.Windows.Forms.TextBox txtPort;
		private System.Windows.Forms.TextBox txtTandemName;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.TextBox txtFtpPassword;
		private System.Windows.Forms.TextBox txtFtpUser;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button btnAddVolume;
		private System.Windows.Forms.TextBox txtNewVolume;
		private System.Windows.Forms.ListBox lstVolumes;
		private System.Windows.Forms.Label lblVolError;

	}
}
