using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CobolParser.Generator
{
	public class CsProjPropertyGroupConfiguration
	{
		public string Condition
		{
			get { return " '$(Configuration)' == '' "; }
		}

		public string Configuration
		{
			get { return "Debug"; }
		}

		public string PlatformCondition
		{
			get { return " '$(Platform)' == '' "; }
		}

		public string Platform
		{
			get { return "x86"; }
		}

		public string ProductVersion
		{
			get { return "8.0.30703"; }
		}

		public string SchemaVersion
		{
			get { return "2.0"; }
		}

		private Guid _guid = Guid.NewGuid();
		public Guid ProjectGuid
		{
			get { return _guid; }
		}

		public string OutputType
		{
			get;
			set;
		}

		public string AppDesignerFolder
		{
			get { return "Properties"; }
		}

		public string RootNamespace
		{
			get;
			set;
		}

		public string AssemblyName
		{
			get { return RootNamespace; }
		}

		public string TargetFrameworkVersion
		{
			get { return "v4.0"; }
		}

		private string _targetFrameworkProfile = "Client";
		public string TargetFrameworkProfile
		{
			get { return _targetFrameworkProfile; }
			set { _targetFrameworkProfile = value; }
		}

		public string FileAlignment
		{
			get { return "512"; }
		}

		public void ToXml(StringBuilder xml)
		{
			xml.Append("  <PropertyGroup>\r\n");
			xml.Append("    <Configuration Condition=\"" + Condition + "\">" + Configuration + "</Configuration>\r\n");
			xml.Append("    <Platform Condition=\"" + PlatformCondition + "\">" + Platform + "</Platform>\r\n");
			xml.Append("    <ProductVersion>" + ProductVersion + "</ProductVersion>\r\n");
			xml.Append("    <SchemaVersion>" + SchemaVersion + "</SchemaVersion>\r\n");
			xml.Append("    <ProjectGuid>{" + ProjectGuid.ToString() + "}</ProjectGuid>\r\n");
			xml.Append("    <OutputType>" + OutputType + "</OutputType>\r\n");
			xml.Append("    <AppDesignerFolder>" + AppDesignerFolder + "</AppDesignerFolder>\r\n");
			xml.Append("    <RootNamespace>" + RootNamespace + "</RootNamespace>\r\n");
			xml.Append("    <AssemblyName>" + RootNamespace + "</AssemblyName>\r\n");
			xml.Append("    <TargetFrameworkVersion>" + TargetFrameworkVersion + "</TargetFrameworkVersion>\r\n");
			if (! String.IsNullOrEmpty(TargetFrameworkProfile))
			{
				xml.Append("    <TargetFrameworkProfile>" + TargetFrameworkProfile + "</TargetFrameworkProfile>\r\n");
			}
			xml.Append("    <FileAlignment>" + FileAlignment + "</FileAlignment>\r\n");
			xml.Append("  </PropertyGroup>\r\n");
		}
	}

	public class CsProjPropertyGroupCompileTarget
	{
		public string Condition
		{
			get;
			set;
		}

		public string PlatformTarget
		{
			get { return "x86"; }
		}

		public string DebugSymbols
		{
			get;
			set;
		}

		public string DebugType
		{
			get;
			set;
		}

		public string Optimize
		{
			get;
			set;
		}

		public string OutputPath
		{
			get;
			set;
		}

		public string DefineConstants
		{
			get;
			set;
		}

		public string ErrorReport
		{
			get;
			set;
		}

		public string WarningLevel
		{
			get;
			set;
		}

		public void ToXml(StringBuilder xml)
		{
			xml.Append("  <PropertyGroup Condition=\"" + Condition + "\">\r\n");
			xml.Append("    <PlatformTarget>" + PlatformTarget + "</PlatformTarget>\r\n");
			xml.Append("    <DebugSymbols>" + DebugSymbols + "</DebugSymbols>\r\n");
			xml.Append("    <DebugType>" + DebugType + "</DebugType>\r\n");
			xml.Append("    <Optimize>" + Optimize + "</Optimize>\r\n");
			xml.Append("    <OutputPath>" + OutputPath + "</OutputPath>\r\n");
			xml.Append("    <DefineConstants>" + DefineConstants + "</DefineConstants>\r\n");
			xml.Append("    <ErrorReport>" + ErrorReport + "</ErrorReport>\r\n");
			xml.Append("    <WarningLevel>" + WarningLevel + "</WarningLevel>\r\n");
			xml.Append("  </PropertyGroup>\r\n");
		}
	}

	public class ItemGroup
	{
		public List<string> Items = new List<string>();

		public string ItemName
		{
			get;
			set;
		}

		public void Add(string s)
		{
			Items.Add(s);
		}

		public void ToXml(StringBuilder xml)
		{
			xml.Append("  <ItemGroup>\r\n");
			foreach (var i in Items)
			{
				xml.Append("    <" + ItemName + " Include=\"" + i + "\" />\r\n");
			}
			xml.Append("  </ItemGroup>\r\n");
		}
	}

	public class ProjectReference
	{
		public string Include
		{
			get;
			set;
		}

		// Shouldn't need this
		//public string Project
		//{
		//    get;
		//    set;
		//}

		public string Name
		{
			get;
			set;
		}

		public void ToXml(StringBuilder xml)
		{
			xml.Append("    <ProjectReference Include=\"" + Include + "\">\r\n");
			if (!String.IsNullOrWhiteSpace(Name))
			{
				xml.Append("      <Name>" + Name + "</Name>\r\n");
			}
			xml.Append("    </ProjectReference>\r\n");
		}
	}

	public class ItemGroupProjectReference
	{
		public List<ProjectReference> Refs = new List<ProjectReference>();

		public void Add(string fileNpath, string name)
		{
			Refs.Add(new ProjectReference() { Include = fileNpath, Name = name });
		}

		public void Add(string fileNpath)
		{
			Refs.Add(new ProjectReference() { Include = fileNpath });
		}

		public void ToXml(StringBuilder xml)
		{
			xml.Append("  <ItemGroup>\r\n");

			foreach (var r in Refs)
			{
				r.ToXml(xml);
			}

			xml.Append("  </ItemGroup>\r\n");
		}
	}

	public class CodeAnalysisDependentAssemblyPaths
	{
		public string Condition
		{
			get { return " '$(VS100COMNTOOLS)' != '' "; }
		}

		public string Include
		{
			get { return "$(VS100COMNTOOLS)..\\IDE\\PrivateAssemblies"; }
		}

		public void ToXml(StringBuilder buf)
		{
			buf.Append("  <ItemGroup>\r\n");
			buf.Append("    <CodeAnalysisDependentAssemblyPaths Condition=\"" + Condition + "\" Include=\"" + Include + "\">\r\n");
			buf.Append("      <Visible>False</Visible>\r\n");
			buf.Append("    </CodeAnalysisDependentAssemblyPaths>\r\n");
			buf.Append("  </ItemGroup>\r\n");
		}
	}

	public class CsProjFile
	{
		public string ToolVersion
		{
			get { return "4.0"; }
		}

		public string DefaultTargets
		{
			get { return "Build"; }
		}

		public string XmlNs
		{
			get { return "http://schemas.microsoft.com/developer/msbuild/2003"; }
		}

		public CsProjPropertyGroupConfiguration ProjectConfig
		{
			get;
			private set;
		}

		public string TargetFrameworkProfile
		{
			get { return ProjectConfig.TargetFrameworkProfile; }
			set { ProjectConfig.TargetFrameworkProfile = value; }
		}

		public CsProjPropertyGroupCompileTarget DebugX86
		{
			get;
			private set;
		}

		public CsProjPropertyGroupCompileTarget ReleaseX86
		{
			get;
			private set;
		}

		public ItemGroup SystemReferences
		{
			get;
			private set;
		}

		public ItemGroup CompileFiles
		{
			get;
			private set;
		}

		public ItemGroupProjectReference ProjectReferences
		{
			get;
			private set;
		}

		public CodeAnalysisDependentAssemblyPaths UnitTestItemGroup
		{
			get;
			private set;
		}

		public CsProjFile(string nameSpace, bool forUnitTest)
		{
			ProjectConfig = new CsProjPropertyGroupConfiguration()
			{
				OutputType = "Library",
				RootNamespace = nameSpace
			};

			DebugX86 = new CsProjPropertyGroupCompileTarget()
			{
				Condition = " '$(Configuration)|$(Platform)' == 'Debug|x86' ",
				DebugSymbols = "true",
				DebugType = "full",
				Optimize = "false",
				OutputPath = "bin\\Debug\\",
				DefineConstants = "DEBUG;TRACE",
				ErrorReport = "prompt",
				WarningLevel = "4"
			};

			ReleaseX86 = new CsProjPropertyGroupCompileTarget()
			{
				Condition = " '$(Configuration)|$(Platform)' == 'Release|x86' ",
				DebugType = "pdbonly",
				Optimize = "true",
				OutputPath = "bin\\Release\\",
				DefineConstants = "TRACE",
				ErrorReport = "prompt",
				WarningLevel = "4"
			};

			SystemReferences = new ItemGroup()
			{
				ItemName = "Reference"
			};
			SystemReferences.Items.Add("Microsoft.CSharp");
			if (forUnitTest)
			{
				SystemReferences.Items.Add("Microsoft.VisualStudio.QualityTools.UnitTestFramework, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a, processorArchitecture=MSIL");
			}
			SystemReferences.Items.Add("System");
			SystemReferences.Items.Add("System.Core");
			SystemReferences.Items.Add("System.Data");
			SystemReferences.Items.Add("System.Data.DataSetExtensions");
			SystemReferences.Items.Add("System.Xml");
			SystemReferences.Items.Add("System.Xml.Linq");

			CompileFiles = new ItemGroup()
			{
				ItemName = "Compile"
			};

			ProjectReferences = new ItemGroupProjectReference();

			if (forUnitTest)
			{
				UnitTestItemGroup = new CodeAnalysisDependentAssemblyPaths();
			}
		}

		public string ToXml()
		{
			StringBuilder xml = new StringBuilder();

			xml.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
			xml.Append("<Project ToolsVersion=\"" + ToolVersion + "\" DefaultTargets=\"" + DefaultTargets + "\" xmlns=\"" + XmlNs + "\">\r\n");

			ProjectConfig.ToXml(xml);
			DebugX86.ToXml(xml);
			ReleaseX86.ToXml(xml);
			SystemReferences.ToXml(xml);

			if (UnitTestItemGroup != null)
			{
				UnitTestItemGroup.ToXml(xml);
			}

			CompileFiles.ToXml(xml);
			ProjectReferences.ToXml(xml);

			xml.Append("  <Import Project=\"$(MSBuildToolsPath)\\Microsoft.CSharp.targets\" />\r\n");
			xml.Append("</Project>");

			return xml.ToString();
		}
	}
}
