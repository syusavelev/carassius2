using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace PNEditorEditView.Model
{
    public class ShadowCopyList<T, Q> : IList<T>
        where T : class
        where Q : class
    {
        private List<T> viewList;
        private List<Q> modelList;

        public ShadowCopyList(List<Q> modelList)
        {
            this.modelList = modelList;
            viewList = new List<T>();
        }

        //public ShadowCopyList()
        //{
        //    viewList = new List<T>();
        //    modelList = new List<Q>();
        //}
        public void Add(T item)
        {
            viewList.Add(item);
            var modelItem = MakeModelItemAndBind(item);
            if (modelItem == null)
                return;//do not need to add something
            modelList.Add(modelItem);
            //TODO: add another item
            //TODO: bind them
        }

        private Q MakeModelItemAndBind(T item)
        {
            var t = typeof(T);
            if (t == typeof(VPlace))
            {
                var p = item as VPlace;
                if (p.IsBound)
                    return null;
                var synced = new Place();
                p.BindSyncPlace(synced);
                return synced as Q;

            }

            if (t == typeof(VTransition))
            {
                var p = item as VTransition;
                if (p.IsBound)
                    return null;
                var synced = new Transition();
                p.BindSyncTransition(synced);
                return synced as Q;
            }

            if (t == typeof(VArc))
            {
                var p = item as VArc;
                if (p.IsBound)
                    return null;
                var synced = new Arc();
                synced.NodeFrom = p.From.SyncedNode;
                synced.NodeTo = p.To.SyncedNode;
                p.BindSyncArc(synced);
                return synced as Q;
            }
            throw new Exception("Illegal sync state");
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public int IndexOf(T item)
        {
            return viewList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            //not used in app
            throw new NotImplementedException();
        }
        public void Remove(T item)
        {
            viewList.Remove(item);
            //find and remove item
            var t = typeof(T);
            if (t == typeof(VPlace))
            {
                var p = item as VPlace;
                modelList.Remove(p.SyncPlace as Q);
            }

            if (t == typeof(VTransition))
            {
                var p = item as VTransition;
                modelList.Remove(p.SyncTransition as Q);
            }

            if (t == typeof(VArc))
            {
                var p = item as VArc;
                modelList.Remove(p.SyncArc as Q);
            }
            //TODO: SYNC remove another item
        }
        public void Clear()
        {
            viewList.Clear();
            modelList.Clear();
        }

        public bool Contains(T item)
        {
            return viewList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            viewList.CopyTo(array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotImplementedException();
        }



        public ShadowCopyList<T, Q> Fill(List<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }

            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return viewList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)viewList).GetEnumerator();
        }

        public int Count
        {
            get { return viewList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }




        public void RemoveAt(int index)
        {
            Remove(viewList[index]);
        }

        public T this[int index]
        {
            get { return viewList[index]; }
            set
            {
                Insert(index, value);
            }
        }
    }
}
