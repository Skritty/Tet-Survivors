//Made by: Bill Dang
using System;
using System.Collections.Generic;

/// <summary>
/// Given a list of WeightedChanceEntry<T>, returns a random entry value using each value's weight
/// </summary>
/// <typeparam name="T">Type of object to be returned</typeparam>
public class WeightedChance<T>
{

    private float totalWeight;
    private readonly List<WeightedChanceEntry<T>> entries;

    public WeightedChance() 
    {
        entries = new List<WeightedChanceEntry<T>>();
    }

    public WeightedChance(List<WeightedChanceEntry<T>> entryList)
    {
        if (entryList.Count == 0)
            throw new InvalidOperationException("Entry list must contain at least one value");

        entries = entryList;
        foreach (WeightedChanceEntry<T> entry in entries)
            totalWeight += entry.weight;
        foreach (WeightedChanceEntry<T> entry in entries)
            entry.Percent = entry.weight / totalWeight;
    }

    public void Add(WeightedChanceEntry<T> entry)
    {
        entries.Add(entry);
        totalWeight += entry.weight;
        foreach (WeightedChanceEntry<T> e in entries)
            e.Percent = e.weight / totalWeight;
    }

    public T GetRandomEntry()
    {
        Random random = new Random();
        return GetRandomEntry(random);
    }

    public T GetRandomEntry(Random random)
    {
        float chance = (float)random.NextDouble();
        foreach (WeightedChanceEntry<T> entry in entries)
        {
            if (chance < entry.Percent)
                return entry.value;
            else
                chance -= entry.Percent;
        }

        throw new IndexOutOfRangeException();
    }

}

public class WeightedChanceEntry<T>
{

    public float Percent { get; set; }
    public readonly float weight;
    public readonly T value;

    public WeightedChanceEntry(float w, T val)
    {
        if (w < 0)
            throw new InvalidOperationException($"Weight cannot be a negative number (weight given was {w})");

        weight = w;
        value = val;
    }

}

/* Example:
 * 
 * WeightedChance<Func<float>> weightedChance = new WeightedChance<Func<float>>(new List<WeightedChanceEntry<Func<float>>> {
 *     new WeightedChanceEntry<Func<float>>(24,    () => { return value; }),
 *     new WeightedChanceEntry<Func<float>>(14,    () => { return value + 2; }),
 *     new WeightedChanceEntry<Func<float>>(18,    () => { return value - 2; }),
 *     new WeightedChanceEntry<Func<float>>(0.8f,  () => { return value + 5; }),
 *     new WeightedChanceEntry<Func<float>>(1.2f,  () => { return value - 5; }),
 *     new WeightedChanceEntry<Func<float>>(18,    () => { return value * 1.02f; }),
 *     new WeightedChanceEntry<Func<float>>(22,    () => { return value * 0.985f; }),
 *     new WeightedChanceEntry<Func<float>>(0.8f,  () => { return value * 1.05f; }),
 *     new WeightedChanceEntry<Func<float>>(1.2f,  () => { return value * 0.95f; })
 * });
 * return weightedChance.GetRandomEntry(random).Invoke();
 *  
 */