using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

    public static IOrderedEnumerable<IEnumerable<T>> GetOrederedPowerSet<T>(this IList<T> list)
    {
        return GetPowerSet(list).OrderBy(en => en.Count());
    }

    public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(this IList<T> list, int limitGroupSize = int.MaxValue)
    {
      
         return from m in Enumerable.Range(0, 1 << list.Count)
               select
                   from i in Enumerable.Range(0, list.Count)
                   where (m & (1 << i)) != 0
                   select list[i];
         
    }


}

public static class UIExtensions
{
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
}

