using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DeepCopy
{
    public static Node DeepCopyNode(Node n)
    {
        Node temp = new Node();
        temp.pid = n.pid;
        temp.text = n.text;
        temp.links = n.links;
        temp.name = n.name;

        return temp;
    }
}