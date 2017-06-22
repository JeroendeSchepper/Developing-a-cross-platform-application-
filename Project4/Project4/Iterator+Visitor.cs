﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Project4
{
    public interface IIterator<T>
    {
        IOption<T> GetNext();
        void Reset();
    }

    public interface IOption<T>
    {
        void Visit(Action onNone, Action<T> onSome);
        U Visit<U>(Func<U> onNone, Func<T, U> onSome);
    }

    public class None<T> : IOption<T>
    {
        public void Visit(Action onNone, Action<T> onSome)
        {
            onNone();
        }

        public U Visit<U>(Func<U> onNone, Func<T, U> onSome)
        {
            return onNone();
        }
    }

    public class Some<T> : IOption<T>
    {
        T value;
        public Some(T value) { this.value = value; }

        public void Visit(Action onNone, Action<T> onSome)
        {
            onSome(value);
        }

        public U Visit<U>(Func<U> onNone, Func<T, U> onSome)
        {
            return onSome(value);
        }
    }
}