using System.Collections;
using UnityEngine;

public abstract class Observer<T> {
    public abstract void OnNotify (T para);
}