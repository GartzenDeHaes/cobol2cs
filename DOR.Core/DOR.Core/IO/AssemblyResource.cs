using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace DOR.Core.IO
{
	public static class AssemblyResource
	{
		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string.
		/// </summary>
		/// <param name="rname">Resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		private static Stream GetEmbeddedStream(string rname)
		{
			// we'll get the executing assembly, and derive
			// the namespace using the first type in the assembly
			Assembly a = Assembly.GetExecutingAssembly();
			string nameSpace = a.GetTypes()[0].Namespace;

			// with the assembly and namespace, we'll get the
			// embedded resource as a stream
			Stream s = a.GetManifestResourceStream(string.Format("{0}.{1}", nameSpace, rname));
			return s;
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string
		/// </summary>
		/// <param name="a">The assembly containing the resource.</param>
		/// <param name="nameSpace">The namespace, usually the path in the project.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		private static Stream GetEmbeddedStream(Assembly a, string nameSpace, string rname)
		{
			// with the assembly and namespace, we'll get the
			// embedded resource as a stream
			Stream s = a.GetManifestResourceStream(string.Format("{0}.{1}", nameSpace, rname));
			return s;
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a binary
		/// </summary>
		/// <param name="a">The assembly containing the resource.</param>
		/// <param name="nameSpace">The namespace, usually the path in the project.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		private static MemoryStream GetEmbeddedBin(Assembly a, string nameSpace, string rname)
		{
			Stream s = GetEmbeddedStream(a, nameSpace, rname);
			if (s == null)
			{
				throw new Exception(a.FullName + nameSpace + rname + " not found");
			}
			BinaryReader br = new BinaryReader(s);
			MemoryStream ms = new MemoryStream(br.ReadBytes((int)br.BaseStream.Length));
			br.Close();
			s.Close();
			return ms;
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a binary
		/// </summary>
		/// <param name="assemblyName">The assembly containing the resource.</param>
		/// <param name="nameSpace">The namespace, usually the path in the project.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		public static MemoryStream GetEmbeddedBin(string assemblyName, string nameSpace, string rname)
		{
			Assembly a = Assembly.Load(assemblyName);
			return GetEmbeddedBin(a, nameSpace, rname);
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a binary
		/// </summary>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		public static MemoryStream GetEmbeddedBin(string rname)
		{
			// we'll get the executing assembly, and derive
			// the namespace using the first type in the assembly
			Assembly a = Assembly.GetExecutingAssembly();
			string sNamespace = a.GetTypes()[0].Namespace;
			return GetEmbeddedBin(a, sNamespace, rname);
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string
		/// </summary>
		/// <param name="a">The assembly containing the resource.</param>
		/// <param name="nameSpace">The namespace, usually the path in the project.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		private static string GetEmbeddedTextFile(Assembly a, string nameSpace, string rname)
		{
			Stream s = GetEmbeddedStream(a, nameSpace, rname);
			if (s == null)
			{
				throw new Exception(a.FullName + nameSpace + rname + " not found");
			}
			// read the contents of the stream into a string
			StreamReader sr = new StreamReader(s);
			String sContents = sr.ReadToEnd();

			sr.Close();
			s.Close();

			return sContents;
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string
		/// </summary>
		/// <param name="assemblyName">The assembly containing the resource.</param>
		/// <param name="nameSpace">The namespace, usually the path in the project.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		public static string GetEmbeddedTextFile(string assemblyName, string nameSpace, string rname)
		{
			Assembly a = Assembly.Load(assemblyName);
			return GetEmbeddedTextFile(a, nameSpace, rname);
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string
		/// </summary>
		/// <param name="assemblyName">The assembly containing the resource.</param>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		public static string GetEmbeddedTextFile(string assemblyName, string rname)
		{
			Assembly a = Assembly.Load(assemblyName);
			return GetEmbeddedTextFile(a, a.GetTypes()[0].Namespace, rname);
		}

		/// <summary>
		/// Generic function for retrieving the contents of an embedded text file resource as a string
		/// </summary>
		/// <param name="rname">The resource name.</param>
		/// <returns>A Stream of the resoruce, or NULL.</returns>
		public static string GetEmbeddedTextFile(string rname)
		{
			// we'll get the executing assembly, and derive
			// the namespace using the first type in the assembly
			Assembly a = Assembly.GetExecutingAssembly();
			string nameSpace = a.GetTypes()[0].Namespace;
			return GetEmbeddedTextFile(a, nameSpace, rname);
		}
	}
}
