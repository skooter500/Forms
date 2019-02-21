using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//addd instances of the items to the tunnell.
public abstract class ItemGenerator : MonoBehaviour
{

	public abstract void GenerateItems (Wormhole pipe);
}