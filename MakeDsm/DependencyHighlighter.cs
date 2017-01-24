using MakeDsm.LinearDependencies;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.ObjectModel;

namespace MakeDsm
{
    abstract class DependencyHighlighter<TModel,TView>
    {

        static readonly Color DefaultColor = Color.White;
       protected abstract Color HighlightColor { get; } 
        public LinearDependencyLocator<TModel> DependencyLocator { get; }
        public DataGridView GridView { get; }
        private int _idx;
        private readonly List<KeyValuePair<TModel, ReadOnlyCollection<TModel>>> _allItems;

        protected event EventHandler PrePaintingItems;

        public int Idx
        {
            get { return _idx; }
            private set
            {
                this.Reset();
                
                if (_allItems.Count > 0)
                {

                    _idx = (value + _allItems.Count) % _allItems.Count;
                    var pair = _allItems[this._idx];
                   
                    this.PaintItems(pair.Key, pair.Value.ToList(), HighlightColor);
                }
                else
                {
                    _idx = -1;
                }
            }
        }     

        public void Next() => this.Idx++;

        public void Previous() => this.Idx--;

        public DependencyHighlighter(DataGridView gv, LinearDependencyLocator<TModel> dependencyLocator)
        {
            this.GridView = gv;
            this.DependencyLocator = dependencyLocator;
            this._allItems = this.DependencyLocator?.LinearDependedRows?.OrderBy(p=> p.Value.Count)?.ToList() ?? new List<KeyValuePair<TModel, ReadOnlyCollection<TModel>>>();
            this.Idx = 0;
        }

        private void PaintItems(TModel mainItem, IList<TModel> items ,Color color)
        {
            try
            {
                this.GridView.SuspendParentDrawing();
                this.GridView.SuspendLayout();//in order to avoid cell formatting when itterating rows

                this.PrePaintingItems?.Invoke(this, EventArgs.Empty);

                if (mainItem != null)
                {
                    var c = color;
                    var shiftedColor = Color.FromArgb(c.A, (int)(c.R * 0.8), (int)(c.G * 0.8), (int)(c.B * 0.8));//make darker
                    var mainItemView = this.GetView(mainItem);
                    this.PaintItem(mainItemView, shiftedColor);
                }
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    var itemView = this.GetView(item);
                    this.PaintItem(itemView, color);
                }
            }
            finally
            {
                this.GridView.ResumeLayout(false);
                this.GridView.ResumeParentDrawing(true);
            }
        }

        protected abstract void PaintItem(TView item, Color color);

        protected abstract TView GetView(TModel model);

        public void Reset()
        {
            var allItems = _allItems.SelectMany(p => p.Value).ToList();
            this.PaintItems(default(TModel), allItems, DefaultColor);
        }
    }
}
