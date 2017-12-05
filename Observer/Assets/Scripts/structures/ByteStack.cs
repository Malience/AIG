using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByteStack{
    private byte[] stack;
    private int top = 0;

    public ByteStack(int size) { stack = new byte[size]; }

    public int pop() {return stack[top--]; }
    public void push(byte x) { stack[++top] = x; }
    public int peek() { return stack[top]; }
    public bool isEmpty() { return top < 0; }
}
