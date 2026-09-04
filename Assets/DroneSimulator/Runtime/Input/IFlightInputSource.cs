using DroneSimulator.Core;

namespace DroneSimulator.Input
{
    public interface IFlightInputSource
    {
        DroneInputState ReadInput();
    }
}

