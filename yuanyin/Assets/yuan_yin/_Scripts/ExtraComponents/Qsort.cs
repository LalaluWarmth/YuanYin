using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Qsort
{
    public static void QSort(int[] data, int low, int high) //递归实现
    {
        if (low >= high) return;
        int i, j, pivot;
        i = low;
        j = high;
        pivot = data[low];
        while (i < j)
        {
            while (data[j] > pivot) j--;
            data[i] = data[j];
            while (i < j && data[i] <= pivot) i++;
            data[j] = data[i];
        }

        data[i] = pivot;
        QSort(data, low, i - 1);
        QSort(data, i + 1, high);
    }
}