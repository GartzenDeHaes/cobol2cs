using System;
using System.Collections.Generic;
using System.Diagnostics;
#if ! SILVERLIGHT
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
#endif
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using DOR.Core.Logging;

namespace DOR.Core
{
	public class PrinipleInfo
	{
		public string Name { get; set; }
		public string DisplayName { get; set; }
		public string UserPrincipalName { get; set; }
		public string Description { get; set; }
		public string DistinguishedName { get; set; }
	}

	/// <summary>
	/// Seperates logon from DorEmployee for impersonation of SA accounts.
	/// </summary>
	public class NtLogon
	{
		private string m_logonWithDomain;
		private string m_domain;
		private string m_userId;
		private string m_password;

#if ! SILVERLIGHT
		private WindowsImpersonationContext m_impersonation;
#endif

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="ntLogonIdWithDomain">dor\abcis140</param>
		/// <param name="password">Password for impersonation</param>
		public NtLogon(string ntLogonIdWithDomain, string password)
		{
			if (String.IsNullOrEmpty(ntLogonIdWithDomain))
			{
				throw new ArgumentException("Empty logon passed to NtLogon");
			}
			if (String.IsNullOrEmpty(password))
			{
				throw new ArgumentException("Empty password passed to NtLogon");
			}

			Debug.Assert(ntLogonIdWithDomain.IndexOf('\\') > -1);

			m_logonWithDomain = ntLogonIdWithDomain;
			int backSlashPos = ntLogonIdWithDomain.IndexOf('\\');
			m_domain = ntLogonIdWithDomain.Substring(0, backSlashPos);
			m_userId = ntLogonIdWithDomain.Substring(backSlashPos + 1);
			m_password = password;
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="domain">DOR</param>
		/// <param name="userId">abcis140</param>
		/// <param name="password">Password for impersonation</param>
		public NtLogon(string domain, string userId, string password)
		{
			m_domain = domain;
			m_userId = userId;
			m_password = password;
			m_logonWithDomain = m_domain + "\\" + m_userId;
		}

		/// <summary>NT domain</summary>
		public string Domain
		{
			get { return m_domain; }
		}

		/// <summary>NT user ID</summary>
		public string UserId
		{
			get { return m_userId; }
		}

		/// <summary>Password for impersonation</summary>
		public string Password
		{
			get { return m_password; }
		}

		/// <summary>dor\abcis140</summary>
		public string LogonWithDomain
		{
			get { return m_logonWithDomain; }
		}

#if ! SILVERLIGHT

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool LogonUser(String lpszUsername,
												String lpszDomain,
												String lpszPassword,
												int dwLogonType,
												int dwLogonProvider,
												ref IntPtr phToken);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private extern static bool CloseHandle(IntPtr handle);

		const int LOGON32_LOGON_INTERACTIVE = 2;
		const int LOGON32_PROVIDER_DEFAULT = 0;

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int LogonUserA(String lpxzUsername,
												String lpszDomain,
												String lpszPassword,
												int dwLogonType,
												int dwLogonProvider,
												ref IntPtr phToken);
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern int DuplicateToken(IntPtr existingTokenHandle,
												int impersonationLevel,
												ref IntPtr duplicateTokenHandle);
		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern long RevertToSelf();

		/// <summary>Begin impersonation</summary>
		/// TODO: move the impersonation code from DorEmployee to NtLogon.
		public virtual void Impersonate()
		{
			WindowsIdentity tempWindowsIdentity;
			IntPtr token = IntPtr.Zero;
			IntPtr tokenDuplicate = IntPtr.Zero;

			if (0 != RevertToSelf())
			{
				if (LogonUserA(m_userId, m_domain, m_password, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref token) != 0)
				{
					if (DuplicateToken(token, 2, ref tokenDuplicate) != 0)
					{
						tempWindowsIdentity = new WindowsIdentity(tokenDuplicate);
						m_impersonation = tempWindowsIdentity.Impersonate();
						if (null != m_impersonation)
						{
							return;
						}
					}
				}
			}

			if (!tokenDuplicate.Equals(IntPtr.Zero))
			{
				CloseHandle(tokenDuplicate);
			}
			if (!token.Equals(IntPtr.Zero))
			{
				CloseHandle(token);
			}
			throw new ImpersonationException();
		}

		/// <summary>Stop impersonation</summary>
		public virtual void EndImpersonate()
		{
			if (null != m_impersonation)
			{
				m_impersonation.Undo();
				m_impersonation = null;
			}
		}

		public IList<ISecurityAuthorization> ListSecurityGroups
		(
			string system
		)
		{
			List<ISecurityAuthorization> auths = new List<ISecurityAuthorization>();

			using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
			{
				using (var p = Principal.FindByIdentity(ctx, LogonWithDomain))
				{
					var groups = p.GetGroups();
					using (groups)
					{
						foreach (Principal group in groups)
						{
							auths.Add(new NtGroupAuthorization(group.SamAccountName));
						}
					}
				}
			}

			return auths;
		}

		public bool IsMemberOfGroup
		(
			string system,
			string groupName,
			string groupDomain
		)
		{
			if (String.IsNullOrEmpty(groupName) || groupName == "*")
			{
				return true;
			}

			bool tryHarder = false;

			using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain, groupDomain))
			{
				using (GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, groupName))
				{
					if (gp == null)
					{
						// group doesn't exist
						return false;
					}
					using (var p = Principal.FindByIdentity(ctx, LogonWithDomain))
					{
						if (p == null)
						{
							//SimpleFileLogger.WriteS(SystemPID.Unknown, "Principle null for " + LogonWithDomain);
							tryHarder = true;
						}
						else
						{
							return gp.Members.Contains(p);
						}
					}
				}
			}

			if (tryHarder)
			{
				List<PrinipleInfo> lst = ListGroupMembers(groupName);
				var res = from p in lst where p.UserPrincipalName.ToLower().StartsWith(UserId.ToLower() + "@") select p;
				if (res.Any())
				{
					return true;
				}
			}

			return false;
		}

		public static List<PrinipleInfo> ListGroupMembers
		(
			string groupName
		)
		{
			List<PrinipleInfo> lst = new List<PrinipleInfo>();

			using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
			{
				using (GroupPrincipal gp = GroupPrincipal.FindByIdentity(ctx, groupName))
				{
					using (PrincipalSearchResult<Principal> results = gp.GetMembers(true))
					{
						foreach (var p in results)
						{
							PrinipleInfo pi = new PrinipleInfo();
							pi.Description = "" + p.Description;
							pi.DisplayName = "" + p.DisplayName;
							pi.DistinguishedName = p.DistinguishedName;
							pi.Name = p.Name;
							pi.UserPrincipalName = "" + p.UserPrincipalName;
							lst.Add(pi);
						}
					}
				}
			}

			return lst;
		}
#endif
	}

#if ! SILVERLIGHT
	/// <summary>
	/// The purpose of this class is to bypass impersonation in a transparent way.
	/// </summary>
	public class NtLogonCurrentUser : NtLogon
	{
		public NtLogonCurrentUser()
		: base(Environment.UserDomainName, Environment.UserName, "")
		{
		}

		/// <summary>Do nothing</summary>
		public override void Impersonate()
		{
		}

		/// <summary>Do nothing</summary>
		public override void EndImpersonate()
		{
		}
	}
#endif
}
