/*
* Copyright (C) 2008-2012 Emulator Nexus <http://emulatornexus.com//>
*
* This program is free software; you can redistribute it and/or modify it
* under the terms of the GNU General Public License as published by the
* Free Software Foundation; either version 3 of the License, or (at your
* option) any later version.
*
* This program is distributed in the hope that it will be useful, but WITHOUT
* ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
* FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
* more details.
*
* You should have received a copy of the GNU General Public License along
* with this program. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ProtocolWalker.Capture
{
    public class CaptureSessionList : EventAwareList<CaptureSession>
    {
        static CaptureSessionList _instance;
        public static CaptureSessionList Instance
        {
            get
            {
                if (_instance == null) _instance = new CaptureSessionList();
                return _instance;
            }
        }
    }

    public class CapturedPacketsList : EventAwareList<TORCapturedPacket>
    {
        
    }

    public class PacketList : ListBox
    {
        public CaptureSession Session;
    }

    public delegate void EventAwareListAddEvent<T>(object sender, EventArgs<T> e);
    public delegate void EventAwareListRemoveEvent<T>(object sender, EventArgs<T> e);
    public delegate bool EventAwareListBeforeAddEvent<T>(object sender, EventArgs<T> e);

    public class EventAwareList<T> : IList<T>
    {
        public event EventAwareListAddEvent<T> OnAdd;
        public event EventAwareListRemoveEvent<T> OnRemove;
        public event EventAwareListBeforeAddEvent<T> BeforeAdd;

        List<T> list = new List<T>();

        int IList<T>.IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item)
        {
            lock (this)
            {
                list.Insert(index, item);
            }
        }

        void IList<T>.RemoveAt(int index)
        {
            lock (this)
            {
                list.RemoveAt(index);
            }
        }

        T IList<T>.this[int index]
        {
            get
            {
                lock (this)
                {
                    return list[index];
                }
            }
            set
            {
                lock (this)
                {
                    list[index] = value;
                }
            }
        }

        public void Add(T item)
        {
            ((ICollection<T>)this).Add(item);
        }

        void ICollection<T>.Add(T item)
        {
            lock (this)
            {
                if (BeforeAdd != null)
                    if (!BeforeAdd(this, new EventArgs<T>(item)))
                        return;
                list.Add(item);
                if (OnAdd != null)
                    OnAdd(this, new EventArgs<T>(item));
            }
        }

        public void AddRange(IEnumerable<T> item)
        {
            foreach (T i in item)
            {
                Add(i);
            }
        }

        public void Remove(T item)
        {
            ((ICollection<T>)this).Remove(item);
        }

        bool ICollection<T>.Remove(T item)
        {
            lock (this)
            {
                bool result = list.Remove(item);
                if (OnRemove != null)
                    OnRemove(this, new EventArgs<T>(item));
                return result;
            }
        }

        public void Clear()
        {
            ((ICollection<T>)this).Clear();
        }

        void ICollection<T>.Clear()
        {
            lock (this)
            {
                list.Clear();
            }
        }

        bool ICollection<T>.Contains(T item)
        {
            return list.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            lock (this)
            {
                list.CopyTo(array, arrayIndex);
            }
        }

        int ICollection<T>.Count
        {
            get { return list.Count; }
        }

        public int Count
        {
            get { return list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (this)
            {
                return list.GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (this)
            {
                return list.GetEnumerator();
            }
        }

        public T Find(Predicate<T> predicate)
        {
            return list.Find(predicate);
        }

        public List<T> FindAll(Predicate<T> predicate)
        {
            return list.FindAll(predicate);
        }

    }

    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value) { val = value; }
        T val;
        public T Value { get { return val; } }
    }

}
