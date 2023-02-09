using System.Collections;
using System.Collections.Generic;

namespace NUnit.Framework.Constraints
{
	public class CollectionTally
	{
		private readonly List<object> list = new List<object>();

		private readonly NUnitEqualityComparer comparer;

		public int Count
		{
			get
			{
				return list.Count;
			}
		}

		public CollectionTally(NUnitEqualityComparer comparer, IEnumerable c)
		{
			this.comparer = comparer;
			foreach (object item in c)
			{
				list.Add(item);
			}
		}

		private bool ItemsEqual(object expected, object actual)
		{
			Tolerance tolerance = Tolerance.Default;
			return comparer.AreEqual(expected, actual, ref tolerance);
		}

		public bool TryRemove(object o)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (ItemsEqual(list[i], o))
				{
					list.RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public bool TryRemove(IEnumerable c)
		{
			foreach (object item in c)
			{
				if (!TryRemove(item))
				{
					return false;
				}
			}
			return true;
		}
	}
}
