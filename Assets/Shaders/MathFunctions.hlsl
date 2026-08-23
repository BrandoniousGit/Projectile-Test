#ifndef MY_SHAPES_INCLUDED
#define MY_SHAPES_INCLUDED

void Circle_float(float2 UV, float Input, out float Output)
{
    float2 cell = floor(UV * Input);
    
    Output = fmod(cell.x + cell.y, 2);
}

void ElectricArc_float(float2 UV, float2 StartPos, float2 EndPos, float Amplitude, out float Output)
{
    float arc = sin(UV.y * 3.141592) * Amplitude;
    Output = arc;
}

#endif