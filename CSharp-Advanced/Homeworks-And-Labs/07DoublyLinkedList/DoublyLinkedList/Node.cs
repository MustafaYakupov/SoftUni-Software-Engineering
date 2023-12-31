﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoublyLinkedList
{
    public class Node
    {
        public Node(int value)
        { 
            this.Value = value;
        }
        public int Value { get; set; }
        public Node Next { get; set; }
        public Node Previous { get; set; }

        public override string ToString()
        {
            return $"{Previous?.Value} <- {Value} -> {Next?.Value}";
        }
    }
}
