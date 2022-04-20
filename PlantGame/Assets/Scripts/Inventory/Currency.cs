using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    private int total;
    Currency()
    {
        total = 50;
    }
    //public int get() { return total; }
    //increment
    public void gain(int inc)
    {
        total += inc;
    }
    //decrement
    public void lose(int dec)
    {
        if((total - dec) >= 0)
        {
            total -= dec;
        }
        else
        {
            total = 0;
        }
    }
}