using System;
using System.Collections.Generic;
using System.Text;

namespace BayesInferCore.Model
{
	public class FloatArray
	{
		/** Serialization runtime version number */
		private static long serialVersionUID = 0;

		public static int DEFAULT_SIZE = 0;

		public float[] data;
		private int size;


		public FloatArray(int initialCapacity)
		{
			if (initialCapacity < 0)
				throw new ArgumentException("CapacityException" + initialCapacity);
			this.data = new float[initialCapacity];
		}

		public FloatArray()
		{
			this.data = new float[DEFAULT_SIZE];
		}




		/// <summary>
		///  Increases the capacity of this FloatArray instance, if necessary, to ensure  that it can hold at least the number of elements specified by the minimum capacity argument.
		/// </summary>
		/// <param name="minCapacity">minCapacity   the desired minimum capacity.</param>
		public void EnsureCapacity(int minCapacity)
		{
			if (minCapacity > data.Length)
			{
				float[] oldData = data;
				int newCapacity = minCapacity;

				data = new float[newCapacity];
				Array.Copy(oldData, 0, data, 0, size);
			}
		}


		/// <summary>
		/// Returns the number of elements in this list
		/// </summary>
		/// <returns>the number of elements in this list</returns>
		public int Size()
		{
			return size;
		}

		/// </summary>
		/// <param name="index">index index of element to return.</param>
		/// <returns>the element at the specified position in this list.</returns>

		public float Get(int index)
		{
			return data[index];
		}

		/// <summary>
		/// Replaces the element at the specified position in this list with the specified element.
		/// </summary>
		/// <param name="index">index index of element to replace.</param>
		/// <param name="element">element element to be stored at the specified position.</param>
		/// <returns>the element previously at the specified position.</returns>
		public float Set(int index, float element)
		{
			float oldValue = data[index];
			data[index] = element;
			return oldValue;
		}

		/// <summary>
		/// Appends the specified element to the end of this list.
		/// </summary>
		/// <param name="newElement">o element to be appended to this list</param>
		/// <returns>true</returns>
		public Boolean Add(float newElement)
		{
			EnsureCapacity(size + 1);
			data[size++] = newElement;
			return true;
		}


		/// <summary>
		/// Inserts the specified element at the specified position in this list. Shifts the element currently at that position (if any) and any subsequent elements to the right (adds one to their indices).
		/// </summary>
		/// <param name="index">index index at which the specified element is to be inserted.</param>
		/// <param name="element">element element to be inserted.</param>
		public void Add(int index, float element)
		{
			EnsureCapacity(size + 1);
			Array.Copy(data, index, data, index + 1,
					 size - index);
			data[index] = element;
			size++;
		}


		/// <summary>
		/// Removes the element at the specified position in this list. Shifts any subsequent elements to the left (subtracts one from their * indices).
		/// </summary>
		/// <param name="index">index the index of the element to removed.</param>
		public void Remove(int index)
		{
			int numMoved = size - index - 1;
			if (numMoved > 0)
				Array.Copy(data, index + 1, data, index, numMoved);
			if (size > 0)
			{
				data[--size] = 0;
			}
		}
	}
}
