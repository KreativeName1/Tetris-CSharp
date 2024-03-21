using System.Diagnostics;

namespace Tetris
{
	public class DebugList<T> : List<T>
	{
		public DebugList(IEnumerable<T> collection) : base(collection)
		{
		}
		public DebugList() { }

		public new void Add(T item)
		{
			base.Add(item);
			LogChange($"Added: {item}");
		}
		public new void Clear()
		{
			base.Clear();
			LogChange("Cleared");
		}
		public new void Insert(int index, T item)
		{
			base.Insert(index, item);
			LogChange($"Inserted: {item}");
		}
		public new void InsertRange(int index, IEnumerable<T> collection)
		{
			base.InsertRange(index, collection);
			LogChange($"Inserted range: {collection}");
		}
		public new bool Remove(T item)
		{
			bool result = base.Remove(item);
			LogChange($"Removed: {item}");
			return result;
		}
		public new int RemoveAll(Predicate<T> match)
		{
			int result = base.RemoveAll(match);
			LogChange($"Removed all: {match}");
			return result;
		}
		public new void RemoveAt(int index)
		{
			base.RemoveAt(index);
			LogChange($"Removed at: {index}");
		}
		public new void RemoveRange(int index, int count)
		{
			base.RemoveRange(index, count);
			LogChange($"Removed range: {index} - {count}");
		}
		public new void Reverse()
		{
			base.Reverse();
			LogChange("Reversed");
		}
		public new void Reverse(int index, int count)
		{
			base.Reverse(index, count);
			LogChange($"Reversed: {index} - {count}");
		}
		public new void Sort()
		{
			base.Sort();
			LogChange("Sorted");
		}
		public new void Sort(Comparison<T> comparison)
		{
			base.Sort(comparison);
			LogChange($"Sorted: {comparison}");
		}
		public new void Sort(IComparer<T> comparer)
		{
			base.Sort(comparer);
			LogChange($"Sorted: {comparer}");
		}
		public new void Sort(int index, int count, IComparer<T> comparer)
		{
			base.Sort(index, count, comparer);
			LogChange($"Sorted: {index} - {count} - {comparer}");
		}
		public new T this[int index]
		{
			get
			{
				LogChange($"Get: {index}");
				//LogChange(this[index].ToString());
				return base[index];
			}
			set
			{
				LogChange($"Set: {index} - {value}");
				base[index] = value;
			}
		}
		public new void AddRange(IEnumerable<T> collection)
		{
			base.AddRange(collection);
			LogChange($"Added range: {collection}");
		}
		public new void ForEach(Action<T> action)
		{
			base.ForEach(action);
			LogChange($"For each: {action}");
		}
		public new List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
		{
			List<TOutput> result = base.ConvertAll(converter);
			LogChange($"Converted all: {converter}");
			return result;
		}
		public new bool Exists(Predicate<T> match)
		{
			bool result = base.Exists(match);
			LogChange($"Exists: {match}");
			return result;
		}
		public new T Find(Predicate<T> match)
		{
			T result = base.Find(match);
			LogChange($"Found: {match}");
			return result;
		}
		public new List<T> FindAll(Predicate<T> match)
		{
			List<T> result = base.FindAll(match);
			LogChange($"Found all: {match}");
			return result;
		}
		public new int FindIndex(Predicate<T> match)
		{
			int result = base.FindIndex(match);
			LogChange($"Found index: {match}");
			return result;
		}
		public new int FindIndex(int startIndex, Predicate<T> match)
		{
			int result = base.FindIndex(startIndex, match);
			LogChange($"Found index: {startIndex} - {match}");
			return result;
		}
		public new int FindIndex(int startIndex, int count, Predicate<T> match)
		{
			int result = base.FindIndex(startIndex, count, match);
			LogChange($"Found index: {startIndex} - {count} - {match}");
			return result;
		}

		public void LogChange(string message)
		{
			Debug.WriteLine($"List changed: {message}");
		}

		public void LogList()
		{
			Debug.WriteLine("List:");
			foreach (T item in this)
			{
				Debug.WriteLine(item);
			}
		}
	}
}
