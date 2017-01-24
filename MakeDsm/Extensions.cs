using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public static class Extensions
{
    public static string AsString(this DataTable dataTable)
    {
        var output = new StringBuilder();

        var columnsWidths = new int[dataTable.Columns.Count];

        // Get column widths
        foreach (DataRow row in dataTable.Rows)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var length = row[i].ToString().Length;
                if (columnsWidths[i] < length)
                    columnsWidths[i] = length;
            }
        }

        // Get Column Titles
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            var length = dataTable.Columns[i].ColumnName.Length;
            if (columnsWidths[i] < length)
                columnsWidths[i] = length;
        }

        // Write Column titles
        for (int i = 0; i < dataTable.Columns.Count; i++)
        {
            var text = dataTable.Columns[i].ColumnName;
            output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
        }
        output.Append("|\n" + new string('=', output.Length) + "\n");

        // Write Rows
        foreach (DataRow row in dataTable.Rows)
        {
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                var text = row[i].ToString();
                output.Append("|" + PadCenter(text, columnsWidths[i] + 2));
            }
            output.Append("|\n");
        }
        return output.ToString();
    }

    private static string PadCenter(string text, int maxLength)
    {
        int diff = maxLength - text.Length;
        return new string(' ', diff / 2) + text + new string(' ', (int)(diff / 2.0 + 0.5));

    }

    public static IOrderedEnumerable<List<T>> GetOrederedPowerSet<T>(this IList<T> list)
    {
        return GetPowerSet(list).OrderBy(en => en.Count());
    }

    //public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(this IList<T> list, int limitGroupSize = int.MaxValue)
    public static IEnumerable<List<T>> GetPowerSet<T>(this IList<T> list, int limitGroupSize = int.MaxValue)
    {

        //return from m in Enumerable.Range(0, 1 << list.Count)
        //      select
        //          from i in Enumerable.Range(0, list.Count)
        //          where (m & (1 << i)) != 0
        //          select list[i];

        List<List<T>> subsetList = new List<List<T>>();

        //The set bits of each intermediate value represent unique 
        //combinations from the startingSet.
        //We can start checking for combinations at (1<<minSubsetSize)-1 since
        //values less than that will not yield large enough subsets.

        var affectiveMaxLength = Math.Min(list.Count, limitGroupSize);
        long iLimitTemp = Convert.ToInt64(Math.Pow(2, list.Count));
        int iLimit = iLimitTemp > int.MaxValue ? int.MaxValue : Convert.ToInt32(iLimitTemp);
        var numbers = Enumerable.Range(1, iLimit).Where(num => NumberOfSetBits(num) <= limitGroupSize).ToList();
        for (int i = 0; i < numbers.Count; i++)
        {
            int setBitCount = NumberOfSetBits(i);
            List<T> subset = new List<T>(setBitCount);

            for (int j = 0; j < list.Count; j++)
            {
                //If the j'th bit in i is set, 
                //then add the j'th element of the startingSet to this subset.
                if ((i & (1 << j)) != 0)
                {
                    subset.Add(list[j]);
                }
            }
            yield return subset;

        }


        //for (int i = 1; i < iLimit; i++)
        //{
        //    //Get the number of 1's in this 'i'
        //    int setBitCount = NumberOfSetBits(i);

        //    //Only include this subset if it will have at least minSubsetSize members.
        //    if (setBitCount <= limitGroupSize)
        //    {
        //        List<T> subset = new List<T>(setBitCount);

        //        for (int j = 0; j < list.Count; j++)
        //        {
        //            //If the j'th bit in i is set, 
        //            //then add the j'th element of the startingSet to this subset.
        //            if ((i & (1 << j)) != 0)
        //            {
        //                subset.Add(list[j]);
        //            }
        //        }
        //       yield return subset;
        //    }
        //}

    }

    static int NumberOfSetBits(int i)
    {
        i = i - ((i >> 1) & 0x55555555);
        i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
        return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
    }
}

public static class UIExtensions
{
    [DllImport("user32.dll")]
    public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

    private const int WM_SETREDRAW = 11;

    public static void SuspendParentDrawing(this Control contol)
    {
        contol?.Parent?.SuspendDrawing();
    }

    public static void ResumeParentDrawing(this Control contol, bool refresh = true)
    {
        contol?.Parent?.ResumeDrawing(refresh);
    }

    private static void SuspendDrawing(this Control parent)
    {
        SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
    }

    private static void ResumeDrawing(this Control parent, bool refresh = true)
    {
        SendMessage(parent.Handle, WM_SETREDRAW, true, 0);

        if (refresh)
        {
            parent.Refresh();

        }
    }

    public static void InvokeIfRequired(this Control obj, MethodInvoker action)
    {
        if (obj.InvokeRequired)
        {
            var args = new object[0];
            obj.Invoke(action, args);
        }
        else
        {
            action();
        }
    }

    public static void DoubleBuffered(this DataGridView dgv, bool setting)
    {
        //http://stackoverflow.com/questions/4255148/how-to-improve-painting-performance-of-datagridview
        Type dgvType = dgv.GetType();
        PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
              BindingFlags.Instance | BindingFlags.NonPublic);
        pi.SetValue(dgv, setting, null);
    }

}

