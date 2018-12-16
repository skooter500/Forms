using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class CreatureController: MonoBehaviour
{
    public float minHeight = 500;
    public float maxHeight = 2000;

    public BGE.Forms.Mother mother;

    public abstract void Restart();        
}
