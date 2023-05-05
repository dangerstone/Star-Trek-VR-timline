using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagComparer: IComparer
{

    // Calls CaseInsensitiveComparer.Compare on the name string.
    int IComparer.Compare(System.Object x, System.Object y)
    {
        return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
    }

}
