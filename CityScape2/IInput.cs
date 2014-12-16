using SharpDX;
using SharpDX.DirectInput;

namespace CityScape2
{
    internal interface IInput
    {
        bool IsKeyDown(Key key);
        bool IsKeyUp(Key key);
        Vector2 MousePosition();
    }
}