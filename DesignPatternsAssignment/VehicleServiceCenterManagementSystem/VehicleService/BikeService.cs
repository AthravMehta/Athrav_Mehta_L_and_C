using VehicleServiceCenterManagementSystem.VehicleService.Contracts;

namespace VehicleServiceCenterManagementSystem.VehicleService
{
    public class BikeService : IVehicleService
    {
        public void PerformVehicleService()
        {
            Console.WriteLine("Bike Servicing initiated");
            ChainLubrication();
            BrakeTightening();
            Console.WriteLine("Bike Servicing Done");
        }

        public void ChainLubrication() { Console.WriteLine("Chain Lubrication Performed"); }
        public void BrakeTightening() { Console.WriteLine("Brake Tightening Performed"); }
    }
}
