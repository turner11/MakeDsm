using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm.LinearDependencies
{
    public abstract class LinearDependencyLocator<T>
    {

        public ReadOnlyCollection<T> Items { get; }
        public ReadOnlyDictionary<T, ReadOnlyCollection<T>> LinearDependedRows;
        readonly Dictionary<T, bool[]> ItemLogicalCache;



        public LinearDependencyLocator(IList<T> ts)
        {
            this.Items = ts.ToList().AsReadOnly();
            this.ItemLogicalCache = this.Items.ToDictionary(it => it, it => null as bool[]);

        }

        internal void Init()
        {
            //var powerset = this.Rows.GetOrederedPowerSet().Select(s => s.ToList()).Where(s=> s.Count > 0).Reverse().ToList();
            var powerset = this.Items.GetOrederedPowerSet().Select(s => s.ToList()).Where(s => s.Count > 0).ToList();
            var depDic = new ConcurrentDictionary<T, ReadOnlyCollection<T>>();


            Action<T> AddDependenciesForRow = (item) =>
            {
                var setsWithOutRow = powerset.Where(s => !s.Contains(item)).ToList();
                var lRow = this.ToLogicalArray(item);
                if (lRow == null)
                {
                    Debug.Fail("Got a null item");
                    return;
                }
                foreach (var currSet in setsWithOutRow)
                {
                    
                    var lSet = currSet.Select(r => this.ToLogicalArray(r)).ToList();
                    var unionResult = lSet.Aggregate((l1, l2) => l1.Zip(l2, (b, l) => b || l).ToArray());
                    bool isLinearDependent = unionResult.SequenceEqual(lRow);
                    if (isLinearDependent)
                    {
                        if (!depDic.ContainsKey(item) || depDic[item].Count < currSet.Count) //add the longer one
                            depDic[item] = currSet.AsReadOnly();
                    }
                }
            };

            bool goParrallel = true;
            if (goParrallel)
            {
                Parallel.ForEach(this.Items, row => AddDependenciesForRow(row));

            }
            else
            {
                foreach (var row in this.Items)
                    AddDependenciesForRow(row);
            }


            this.LinearDependedRows = new ReadOnlyDictionary<T, ReadOnlyCollection<T>>(depDic);
        }

       
        public static Tuple<LinearRowDependencyLocator, LinearColumnDependencyLocator> Factory(ModularityMatrixVM vm)
        {

            var colLocator = new LinearColumnDependencyLocator(vm.ModularityMatrix);
            var rowLocator = new LinearRowDependencyLocator(vm.ModularityMatrix.Rows.OfType<DataRow>().ToList());
            colLocator.Init();
            rowLocator.Init();
            return new Tuple<LinearRowDependencyLocator, LinearColumnDependencyLocator>(rowLocator,colLocator);
        }

        private bool[] ToLogicalArray(T item)
        {
            var logical = this.ItemLogicalCache[item];
            if (logical == null)
            {
                logical = this.ToLogicalArrayInternal(item);
                this.ItemLogicalCache[item] = logical;
            }
            return logical;
        }


        protected abstract bool[] ToLogicalArrayInternal(T ts);
    }
}
