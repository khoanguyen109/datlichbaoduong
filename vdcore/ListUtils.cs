using System;
using System.Collections.Generic;
using System.Text;

namespace Vendare
{
    public class ListUtils
    {
        
        public static void ShuffleList<T>(IList<T> list)
        {
            Random random = new Random();
            if (list.Count > 1)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    T tmp = list[i];
                    int randomIndex = random.Next(i + 1);

                    //Swap elements
                    list[i] = list[randomIndex];
                    list[randomIndex] = tmp;
                }
            }
        }

    }
}
