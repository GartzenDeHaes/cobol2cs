using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;
using CobolParser;

namespace CobView
{
	public partial class Configuration : DockContent
	{
		public Configuration()
		{
			InitializeComponent();
		}

		private void btnFileRoot_Click(object sender, EventArgs e)
		{
			var fdia = new FolderBrowserDialog();
			fdia.Description = "Select the root directory for the COBOL source tree";
			var res = fdia.ShowDialog(this);

			if (res == System.Windows.Forms.DialogResult.OK)
			{
				txtSrcPath.Text = fdia.SelectedPath;
			}
		}

		private void Configuration_Load(object sender, EventArgs e)
		{
			txtSrcPath.Text = CobDb.Default.SourceRoot;
			txtTandemName.Text = CobDb.Default.TandemDnsName;
			txtPort.Text = CobDb.Default.FtpPort.ToString();
			txtFtpCmd.Text = CobDb.Default.FtpCommand;
			txtOssGuardianPath.Text = CobDb.Default.OssGuardianVolumePath;
			txtFtpUser.Text = CobDb.Default.FtpUser;
			txtFtpPassword.Text = CobDb.Default.FtpPassword;

			string volDelim = CobDb.Default.SrcVolumes;

			if (string.IsNullOrWhiteSpace(volDelim))
			{
				return;
			}

			string[] vols = volDelim.Split(new char[] { ',' });
			for (int x = 0; x < vols.Length; x++)
			{
				if (String.IsNullOrWhiteSpace(vols[x]))
				{
					continue;
				}
				lstVolumes.Items.Add(vols[x]);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			CobDb.Default.SourceRoot = txtSrcPath.Text;
			CobDb.Default.TandemDnsName = txtTandemName.Text;
			CobDb.Default.FtpPort = Int32.Parse(txtPort.Text);
			CobDb.Default.FtpCommand = txtFtpCmd.Text;
			CobDb.Default.OssGuardianVolumePath = txtOssGuardianPath.Text;
			CobDb.Default.FtpUser = txtFtpUser.Text;
			CobDb.Default.FtpPassword = txtFtpPassword.Text;

			StringBuilder buf = new StringBuilder();

			foreach (string vol in lstVolumes.Items)
			{
				buf.Append(vol);
				buf.Append(',');
			}

			CobDb.Default.SrcVolumes = buf.ToString();

			CobDb.Default.Save();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnAddVolume_Click(object sender, EventArgs e)
		{
			if (! GuardianPath.IsGuardianPath(txtNewVolume.Text + ".ZZZ"))
			{
				lblVolError.Text = "Invalid volume/sub vol";
				lblVolError.Visible = true;
				return;
			}

			var gpath = new GuardianPath(txtNewVolume.Text + ".ZZZ");
			lstVolumes.Items.Add(gpath.Drive + "." + gpath.Volume);
		}

		private void lstVolumes_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete)
			{
				if (lstVolumes.SelectedItem != null)
				{
					lstVolumes.Items.Remove(lstVolumes.SelectedItem);
				}
			}
		}
	}
}
