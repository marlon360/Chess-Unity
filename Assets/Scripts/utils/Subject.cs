using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Subject<T> {
    //A list with observers that are waiting for something to happen
    List<Action<T>> observers = new List<Action<T>> ();

    //Send notifications if something has happened
    public void Notify (T para) {
        for (int i = 0; i < observers.Count; i++) {
            //Notify all observers even though some may not be interested in what has happened
            //Each observer should check if it is interested in this event
            observers[i].Invoke(para);
        }
    }

    //Add observer to the list
    public void AddObserver (Action<T> observer) {
        observers.Add (observer);
    }

    //Remove observer from the list
    public void RemoveObserver (Action<T> observer) { }

    public void ClearObservers() {
        observers.Clear();
    }

}