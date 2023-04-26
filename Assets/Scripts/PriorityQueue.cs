using System.Collections.Generic;

public class PriorityQueue<T> {

	List<Queue<T>> list = new List<Queue<T>>();


	int count = 0;
	int minimum = int.MaxValue;

	public int Count {
		get {
			return count;
		}
	}

	public void Enqueue (T element, int priority) {
		count += 1;
		if (priority < minimum) {
			minimum = priority;
		}
		while (priority >= list.Count) {
			list.Add(null);
		}
		if (list[priority] == null) {
			list[priority] = new Queue<T>();
		}
		list[priority].Enqueue(element);
	}

	public T Dequeue () {
		count -= 1;
		for (; minimum < list.Count; minimum++) {
			Queue<T> cell = list[minimum];
			if (cell != null && cell.Count > 0) {
				return cell.Dequeue();
			}
		}
		return default(T);
	}

	public void Clear () {
		list.Clear();
		count = 0;
		minimum = int.MaxValue;
	}
}