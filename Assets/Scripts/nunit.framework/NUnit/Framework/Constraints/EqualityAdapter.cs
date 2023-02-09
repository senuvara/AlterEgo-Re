using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
	public abstract class EqualityAdapter
	{
		private class ComparerAdapter : EqualityAdapter
		{
			private IComparer comparer;

			public ComparerAdapter(IComparer comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				return comparer.Compare(x, y) == 0;
			}
		}

		private class EqualityComparerAdapter : EqualityAdapter
		{
			private IEqualityComparer comparer;

			public EqualityComparerAdapter(IEqualityComparer comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				return comparer.Equals(x, y);
			}
		}

		internal class PredicateEqualityAdapter<TActual, TExpected> : EqualityAdapter
		{
			private readonly Func<TActual, TExpected, bool> _comparison;

			public override bool CanCompare(object x, object y)
			{
				return true;
			}

			public override bool AreEqual(object x, object y)
			{
				return _comparison((TActual)y, (TExpected)x);
			}

			public PredicateEqualityAdapter(Func<TActual, TExpected, bool> comparison)
			{
				_comparison = comparison;
			}
		}

		private abstract class GenericEqualityAdapter<T> : EqualityAdapter
		{
			public override bool CanCompare(object x, object y)
			{
				return typeof(T).GetTypeInfo().IsAssignableFrom(x.GetType().GetTypeInfo()) && typeof(T).GetTypeInfo().IsAssignableFrom(y.GetType().GetTypeInfo());
			}

			protected void ThrowIfNotCompatible(object x, object y)
			{
				if (!typeof(T).GetTypeInfo().IsAssignableFrom(x.GetType().GetTypeInfo()))
				{
					throw new ArgumentException("Cannot compare " + x.ToString());
				}
				if (!typeof(T).GetTypeInfo().IsAssignableFrom(y.GetType().GetTypeInfo()))
				{
					throw new ArgumentException("Cannot compare " + y.ToString());
				}
			}
		}

		private class EqualityComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			private IEqualityComparer<T> comparer;

			public EqualityComparerAdapter(IEqualityComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer.Equals((T)x, (T)y);
			}
		}

		private class ComparerAdapter<T> : GenericEqualityAdapter<T>
		{
			private IComparer<T> comparer;

			public ComparerAdapter(IComparer<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer.Compare((T)x, (T)y) == 0;
			}
		}

		private class ComparisonAdapter<T> : GenericEqualityAdapter<T>
		{
			private Comparison<T> comparer;

			public ComparisonAdapter(Comparison<T> comparer)
			{
				this.comparer = comparer;
			}

			public override bool AreEqual(object x, object y)
			{
				ThrowIfNotCompatible(x, y);
				return comparer((T)x, (T)y) == 0;
			}
		}

		public abstract bool AreEqual(object x, object y);

		public virtual bool CanCompare(object x, object y)
		{
			if (x is string && y is string)
			{
				return true;
			}
			if (x is IEnumerable || y is IEnumerable)
			{
				return false;
			}
			return true;
		}

		public static EqualityAdapter For(IComparer comparer)
		{
			return new ComparerAdapter(comparer);
		}

		public static EqualityAdapter For(IEqualityComparer comparer)
		{
			return new EqualityComparerAdapter(comparer);
		}

		public static EqualityAdapter For<TExpected, TActual>(Func<TExpected, TActual, bool> comparison)
		{
			return new PredicateEqualityAdapter<TExpected, TActual>(comparison);
		}

		public static EqualityAdapter For<T>(IEqualityComparer<T> comparer)
		{
			return new EqualityComparerAdapter<T>(comparer);
		}

		public static EqualityAdapter For<T>(IComparer<T> comparer)
		{
			return new ComparerAdapter<T>(comparer);
		}

		public static EqualityAdapter For<T>(Comparison<T> comparer)
		{
			return new ComparisonAdapter<T>(comparer);
		}
	}
}
