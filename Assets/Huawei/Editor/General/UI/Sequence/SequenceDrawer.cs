using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace HmsPlugin
{
    public interface ISequenceDrawer
    {
        void AddDrawer(IDrawer drawer);
        void RemoveDrawer(IDrawer drawer);
        int Count();
    }

    public class SequenceDrawer : IDrawer, ISequenceDrawer
    {
        protected List<IDrawer> _drawers = new List<IDrawer>();

        public SequenceDrawer()
        {
        }

        public SequenceDrawer(params IDrawer[] drawers)
        {
            _drawers = drawers.ToList();
        }

        public virtual void AddDrawer(IDrawer drawer)
        {
            _drawers.Add(drawer);
        }

        public virtual void RemoveDrawer(IDrawer drawer)
        {
            _drawers.Remove(drawer);
        }

        public virtual void RemoveAllDrawers()
        {
            _drawers.Clear();
        }

        public virtual void Draw()
        {
            _drawers.ForEach(d => d.Draw());
        }

        public int Count()
        {
            return _drawers.Count;
        }

        public IList<T> FindByType<T>() where T : class, IDrawer
        {
            List<T> found = new List<T>();

            foreach (var drawer in _drawers)
            {
                if (drawer is T)
                {
                    found.Add(drawer as T);
                }
                else if (drawer is SequenceDrawer)
                {
                    found.AddRange((drawer as SequenceDrawer).FindByType<T>());
                }
            }
            return found;
        }
    }
}