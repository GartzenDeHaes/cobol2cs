using System;
using System.Collections.Generic;
using System.Text;

namespace DOR.Core.Collections
{
	/// <summary>
	/// Associate two instances.
	/// </summary>
	/// <typeparam name="A">Left item type.</typeparam>
	/// <typeparam name="B">Right item type.</typeparam>
	[Serializable]
	public class Association<A, B>
	{
		private A m_a;
		private B m_b;

		public Association(A a, B b)
		{
			m_a = a;
			m_b = b;
		}

		public A Left
		{
			get { return m_a; }
		}

		public B Right
		{
			get { return m_b; }
		}

		public A Key
		{
			get { return Left; }
			set { m_a = value; }
		}

		public B Value
		{
			get { return Right; }
			set { m_b = value; }
		}

		public static bool operator ==(Association<A, B> a1, Association<A, B> a2)
		{
			return a1.Equals(a2);
		}

		public static bool operator !=(Association<A, B> a1, Association<A, B> a2)
		{
			return !a1.Equals(a2);
		}

		public override bool Equals(object obj)
		{
			if (null == obj)
			{
				return false;
			}
			if (obj is Association<A, B>)
			{
				return m_a.Equals(((Association<A, B>)obj).m_a) &&
					m_b.Equals(((Association<A, B>)obj).m_b);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_a.GetHashCode() ^ m_b.GetHashCode();
		}

		public string ToXml()
		{
			return "<" + m_a + ">" + StringHelper.XmlEncode(m_b.ToString()) + "</" + m_a + ">";
		}

		public Association<A, B> Clone()
		{
			return new Association<A, B>(Left, Right);
		}
	}
}
