using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Func
{
    public static int CountUp(int n) {
        int result = 0;
        for (int i = 1; i <= n; i++) result += i;
        
        return result;
    }
}
