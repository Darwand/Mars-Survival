using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater
{
    int x;
    int y;

    float width;

    public Crater(int x, int y, float width)
    {
        this.x = x;
        this.y = y;

        this.width = width / 2;
    }

    public float GetModifierForPosition(int x, int y)
    {
        float posX = this.x - x;
        float posY = this.y - y;

        if ((posX * posX) + (posY * posY) <= width * width)
        {
            float dist = (posX * posX) + (posY * posY);
            
            if(dist <= ((width - 10) * (width - 10)))
            {
                return .3f;
            }

            return 1;
        }

        return 0f;
    }
}
