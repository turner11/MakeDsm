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
        Dictionary<T, bool[]> ItemLogicalCache;



        public LinearDependencyLocator(IList<T> ts)
        {
            this.Items = ts.ToList().AsReadOnly();


        }

        internal void Init()
        {
            this.ItemLogicalCache = this.Items.ToDictionary(itm => itm, itm => this.ToLogicalArrayInternal(itm));
          

            //var maxCount = maxCountItem.Count(v=> v);//for optimization...
            ////var powerset = this.Rows.GetOrederedPowerSet().Select(s => s.ToList()).Where(s=> s.Count > 0).Reverse().ToList();
            //var powerset = this.Items.GetPowerSet(maxCount).ToList();
           
            var depDic = new ConcurrentDictionary<T, ReadOnlyCollection<T>>();
            //var setByCount = powerset.GroupBy(s=> s.Count);
            

            Action<T> AddDependenciesForRow = (item) =>
            {
                var lRow = ToLogicalArray(item);
                if (lRow == null)
                {
                    Debug.Fail("Got a null item");
                    return;
                }
                var itemCount = lRow.Count(b=> b);

                var itemTruesIdxs = lRow.Select((b, i) => new { Value = b, Index = i }).Where(an => an.Value).Select(an => an.Index).ToList();
                var candidates = this.ItemLogicalCache.Where(p=> !p.Key.Equals(item))
                                .Where(p => itemTruesIdxs.Any(i => p.Value[i]))
                                .Select(p=> p.Key)
                                .ToList();// Its a candidate only if there is any intersction on "true"s

                var maxCountItem = lRow.Count(v => v);
                var powerset = candidates.GetPowerSet(maxCountItem).Where(s=> s.Count > 0).ToList();
                foreach (var currSet in powerset)
                {
                    
                    var lSet = currSet.Select(r => ToLogicalArray(r)).ToList();
                    var unionResult = lSet.Aggregate((l1, l2) => l1.Zip(l2, (b, l) => b || l).ToArray());
                    bool isLinearDependent = unionResult.SequenceEqual(lRow);
                    if (isLinearDependent)
                    {
                        if (!depDic.ContainsKey(item)) //add the longer one
                            depDic[item] = currSet.AsReadOnly();
                        else
                        {
                            depDic[item] = currSet.Union(depDic[item]).Distinct().ToList().AsReadOnly();
                        }
                    }
                }
            };

            bool goParrallel = true;
#if DEBUG
            goParrallel = false;
#endif
            if (goParrallel)
            {
                Parallel.ForEach(this.Items, row => AddDependenciesForRow(row));

            }
            else
            {
                foreach (var item in this.Items)
                    AddDependenciesForRow(item);
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
