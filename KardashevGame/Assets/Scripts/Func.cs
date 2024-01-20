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

    public static int[] ThePowersThatB2 ={
        1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048
    };
}
