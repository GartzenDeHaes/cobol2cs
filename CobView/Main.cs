using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace CobView
{
	public partial class Main : Form
	{
		private DeserializeDockContent m_deserializeDockContent;

		public Main()
		{
			InitializeComponent();

			m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
		}

		private void MainForm_Load(object sender, System.EventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), "DockPanel.config");

			if (File.Exists(configFile))
			{
				dockPanel.LoadFromXml(configFile, m_deserializeDockContent);
			}
		}

		private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			string configFile = Path.Combine(Path.GetDirectoryName(Application.UserAppDataPath), "DockPanel.config");
			dockPanel.SaveAsXml(configFile);
		}
		
		private IDockContent GetContentFromPersistString(string persistString)
		{
			//if (persistString == typeof(DummySolutionExplorer).ToString())
			//   return m_solutionExplorer;
			//else if (persistString == typeof(DummyPropertyWindow).ToString())
			//   return m_propertyWindow;
			//else if (persistString == typeof(DummyToolbox).ToString())
			//   return m_toolbox;
			//else if (persistString == typeof(DummyOutputWindow).ToString())
			//   return m_outputWindow;
			//else if (persistString == typeof(DummyTaskList).ToString())
			//   return m_taskList;
			//else
			//{
			//   // DummyDoc overrides GetPersistString to add extra information into persistString.
			//   // Any DockContent may override this value to add any needed information for deserialization.

			//   string[] parsedStrings = persistString.Split(new char[] { ',' });
			//   if (parsedStrings.Length != 3)
			//      return null;

			//   if (parsedStrings[0] != typeof(DummyDoc).ToString())
			//      return null;

			//   DummyDoc dummyDoc = new DummyDoc();
			//   if (parsedStrings[1] != string.Empty)
			//      dummyDoc.FileName = parsedStrings[1];
			//   if (parsedStrings[2] != string.Empty)
			//      dummyDoc.Text = parsedStrings[2];

			//   return dummyDoc;
			//}

			return null;
		}

		private void Main_Shown(object sender, EventArgs e)
		{
			if (String.IsNullOrWhiteSpace(CobDb.Default.SourceRoot))
			{
				var confWin = new Configuration();
				confWin.ShowDialog(this);
			}
		}
	}
}
