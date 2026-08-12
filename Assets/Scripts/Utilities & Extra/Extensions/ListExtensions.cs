using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

public static class ListExtensions
{
    static Random rng = new Random();

    /// <summary>
    /// Determines whether a collection is null or has no elements
    /// without having to enumerate the entire collection to get a count.
    ///
    /// Uses LINQ's Any() method to determine if the collection is empty,
    /// so there is some GC overhead.
    /// </summary>
    /// <param name="list">List to evaluate</param>
    public static bool IsNullOrEmpty<T>(this IList<T> list) {
        return list == null || !list.Any();
    }

    /// <summary>
    /// Creates a new list that is a copy of the original list.
    /// </summary>
    /// <param name="list">The original list to be copied.</param>
    /// <returns>A new list that is a copy of the original list.</returns>
    public static List<T> Clone<T>(this IList<T> list) 
    {
        List<T> newList = new List<T>();
        foreach (T item in list) 
        {
            newList.Add(item);
        }

        return newList;
    }

    /// <summary>
    /// Shuffles the elements in the list using the Durstenfeld implementation of the Fisher-Yates algorithm.
    /// This method modifies the input list in-place, ensuring each permutation is equally likely, and returns the list for method chaining.
    /// Reference: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
    /// </summary>
    /// <param name="list">The list to be shuffled.</param>
    /// <typeparam name="T">The type of the elements in the list.</typeparam>
    /// <returns>The shuffled list.</returns>
    public static IList<T> Shuffle<T>(this IList<T> list) 
    {
        if (rng == null) rng = new Random();
        int count = list.Count;
        while (count > 1) {
            --count;
            int index = rng.Next(count + 1);
            (list[index], list[count]) = (list[count], list[index]);
        }
        return list;
    }

    /// <summary>
    /// Filters a collection based on a predicate and returns a new list
    /// containing the elements that match the specified condition.
    /// </summary>
    /// <param name="source">The collection to filter.</param>
    /// <param name="predicate">The condition that each element is tested against.</param>
    /// <returns>A new list containing elements that satisfy the predicate.</returns>
    public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)
    {
        List<T> list = new List<T>();
        foreach (T item in source)
        {
            if (predicate(item))
            {
                list.Add(item);
            }
        }
        return list;
    }

    /// <summary>
    /// Swaps two elements in the list at the specified indices.
    /// </summary>
    /// <param name="list">The list.</param>
    /// <param name="indexA">The index of the first element.</param>
    /// <param name="indexB">The index of the second element.</param>
    public static void Swap<T>(this IList<T> list, int indexA, int indexB) {
        (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
    }

    /// <summary>
    /// Removes and returns the first element in the list.
    /// </summary>
    public static T PopFirst<T>(this IList<T> list)
    {
        if (list.IsNullOrEmpty())
            throw new ArgumentException("List is null or empty.", nameof(list));

        var value = list[0];
        list.RemoveAt(0);
        return value;
    }

    /// <summary>
    /// Removes and returns the last element in the list.
    /// </summary>
    public static T PopLast<T>(this IList<T> list)
    {
        if (list.IsNullOrEmpty())
            throw new ArgumentException("List is null or empty.", nameof(list));

        var lastIndex = list.Count - 1;
        var value = list[lastIndex];
        list.RemoveAt(lastIndex);
        return value;
    }

    #region Random Item From List
    /// <summary>
	/// Choose one item from the list, leaving the original list unaltered.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	public static T ChooseRandom<T>(this IReadOnlyList<T> list)
	{
        if (rng == null) rng = new Random();

		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));

		return list[rng.Next(0, list.Count)];
	}

    /// <summary>
	/// Choose count items from the list, leaving the original list unaltered.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
	/// <param name="count">The count.</param>
	/// <param name="allowDuplicates">if set to <c>true</c> is the same item can be picked multiple times.</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	/// <exception cref="ArgumentOutOfRangeException">
	/// count;count must be greater than or equal to zero
	/// or
	/// count;count must be less than or equal to list.Count if allowDuplicates is false
	/// </exception>
	public static List<T> ChooseRandom<T>(this IReadOnlyList<T> list, int count, bool allowDuplicates = true)
	{
		if (list == null || list.Count == 0)
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));

		if (count < 0)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be greater than or equal to zero");
		if (list.Count < count && !allowDuplicates)
			throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be less than or equal to {nameof(list)}.Count if {nameof(allowDuplicates)} is false");

		var result = new List<T>();

		if (count == 0)
			return result;

		if (allowDuplicates)
		{
			for (var x = 0; x < count; x++)
				result.Add(ChooseRandom(list));
		}
		else
		{
			var temp = list.ToList();
			for (var x = 0; x < count; x++)
				result.Add(DrawRandom(temp));
		}

		return result;
	}

    /// <summary>
	/// Picks a random element from the list. Optionally removes the element from the list.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="list">The list.</param>
    /// <param name="removeFromList">
    /// If <c>true</c>, the picked element is removed from the list; 
    /// if <c>false</c>, the list remains unchanged.
    /// </param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">list;list is null.</exception>
	/// <exception cref="ArgumentException">List cannot be read-only;list</exception>
	public static T DrawRandom<T>(this IList<T> list)
	{
        if (rng == null) rng = new Random();

		if (list.IsNullOrEmpty())
			throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));
		if (list.IsReadOnly)
			throw new ArgumentException($"{nameof(list)} cannot be read-only", nameof(list));

		var index = rng.Next(0, list.Count);
		var result = list[index];

		list.RemoveAt(index);
        
		return result;
	}

    /// <summary>
    /// Picks a number of random elements from the list, optionally removing them.
    /// </summary>
    /// <typeparam name="T">Type of elements in the list.</typeparam>
    /// <param name="list">The list to pick from.</param>
    /// <param name="count">Number of elements to pick.</param>
    /// <param name="removeFromList">
    /// If true, picked elements are removed; otherwise, the list remains unchanged.
    /// </param>
    /// <returns>A list of randomly picked elements.</returns>
    /// <exception cref="ArgumentException">Thrown if the list is null, empty, or read-only when removing.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if count is invalid.</exception>
    public static List<T> DrawRandom<T>(this IList<T> list, int count, bool removeFromList = false)
    {
        if (list.IsNullOrEmpty())
            throw new ArgumentException($"{nameof(list)} is null or empty.", nameof(list));
        if (removeFromList && list.IsReadOnly)
            throw new ArgumentException($"{nameof(list)} cannot be read-only when removing elements.", nameof(list));
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be greater than or equal to zero.");
        if (count > list.Count)
            throw new ArgumentOutOfRangeException(nameof(count), count, $"{nameof(count)} must be less than or equal to {nameof(list)}.Count.");

        var result = new List<T>(count);

        for (int i = 0; i < count; i++)
        {
            result.Add(list.DrawRandom());
        }

        return result;
    }
    #endregion
}
