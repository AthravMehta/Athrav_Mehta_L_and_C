using VehicleServiceCenterManagementSystem.VehicleService.Contracts;

namespace VehicleServiceCenterManagementSystem.VehicleService
{
    public class TruckService : IVehicleService
    {
        public void PerformVehicleService()
        {
            Console.WriteLine("Truck Servicing initiated");
            HeavyDutyEngineDiagnostics();
            CargoInspection();
            Console.WriteLine("Truck Servicing Done");
        }

        public void HeavyDutyEngineDiagnostics() { Console.WriteLine("Heavy Duty Engine Diagnostics Performed"); }
        public void CargoInspection() { Console.WriteLine("Cargo Inspection Done"); }
    }
}
