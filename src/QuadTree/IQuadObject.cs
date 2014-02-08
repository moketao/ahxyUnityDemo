using System;
using UnityEngine;

namespace CSharpQuadTree
{
    public interface IQuadObject
    {
        Rect theBounds { get; }
        event EventHandler Changed;
		bool inTree { get; set;}
    }
}