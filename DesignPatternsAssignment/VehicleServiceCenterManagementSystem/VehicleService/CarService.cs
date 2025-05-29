using VehicleServiceCenterManagementSystem.VehicleService.Contracts;

namespace VehicleServiceCenterManagementSystem.VehicleService
{
    public class CarService : IVehicleService
    {
        public void PerformVehicleService()
        {
            Console.WriteLine("Car Servicing initiated");
            OilChange();
            BrakeInspection();
            TireRotation();
            Console.WriteLine("Car Servicing Done");
        }

        public void OilChange() { Console.WriteLine("Oil Service Performed"); }
        public void BrakeInspection() { Console.WriteLine("Brake Inspection Performed"); }
        public void TireRotation() { Console.WriteLine("Tire Rotation Checked"); }
    }
}
