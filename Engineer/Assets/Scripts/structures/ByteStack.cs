using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteStack{
    private byte[] stack;
    private int top = -1;

    public ByteStack(int size) { stack = new byte[size]; }

    public byte pop() {return stack[top--]; }
    public void push(byte x) { stack[++top] = x; }
    public byte peek() { return stack[top]; }
    public bool isEmpty() { return top < 0; }
    public int size() { return top + 1; }
}
