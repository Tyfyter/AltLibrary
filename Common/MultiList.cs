using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltLibrary.Common {
	public class MultiList<T> : ICollection<T>, ICollection<List<T>> {
		readonly List<List<T>> backingList = [];
		public T this[int index] {
			get {
				int listIndex = 0;
				List<T> list;
				while ((list = backingList[listIndex]).Count <= index) {
					index -= list.Count;
					listIndex++;
				}
				return list[index];
			}
		}
		public int Count {
			get {
				int count = 0;
				for (int i = 0; i < backingList.Count; i++) {
					count += backingList[i].Count;
				}
				return count;
			}
		}
		public bool IsReadOnly => true;

		public void Add(List<T> item) {
			backingList.Add(item);
		}

		public void Clear() {
			backingList.Clear();
		}

		public bool Contains(List<T> item) {
			return backingList.Contains(item);
		}

		public void CopyTo(List<T>[] array, int arrayIndex) {
			backingList.CopyTo(array, arrayIndex);
		}

		IEnumerator<List<T>> IEnumerable<List<T>>.GetEnumerator() => backingList.GetEnumerator();

		public bool Remove(List<T> item) {
			return backingList.Remove(item);
		}

		void ICollection<T>.Add(T item) {
			throw new NotImplementedException();
		}
		void ICollection<T>.Clear() {
			throw new NotImplementedException();
		}
		bool ICollection<T>.Contains(T item) {
			throw new NotImplementedException();
		}
		void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
			throw new NotImplementedException();
		}
		public IEnumerator<T> GetEnumerator() {
			for (int i = 0; i < backingList.Count; i++) {
				List<T> list = backingList[i];
				for (int j = 0; j < list.Count; j++) {
					yield return list[j];
				}
			}
		}
		IEnumerator IEnumerable.GetEnumerator() {
			for (int i = 0; i < backingList.Count; i++) {
				List<T> list = backingList[i];
				for (int j = 0; j < list.Count; j++) {
					yield return list[j];
				}
			}
		}
		bool ICollection<T>.Remove(T item) {
			throw new NotImplementedException();
		}
	}
}
