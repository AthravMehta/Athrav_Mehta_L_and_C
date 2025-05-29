using VehicleServiceCenterManagementSystem.Enums;
using VehicleServiceCenterManagementSystem.VehicleService;
using VehicleServiceCenterManagementSystem.VehicleService.Contracts;

namespace VehicleServiceCenterManagementSystem
{
    public class VehicleServiceFactory
    {
        public IVehicleService PerformVehicleServiceOn(VehicleTypeEnum serviceType)
        {
            switch (serviceType) {
                case VehicleTypeEnum.Bike:
                    return new BikeService();
                case VehicleTypeEnum.Car:
                    return new CarService();
                case VehicleTypeEnum.Truck:
                    return new TruckService();
            }
            throw new Exception("Invalid Service Vehicle Type");
        }
    }
}
