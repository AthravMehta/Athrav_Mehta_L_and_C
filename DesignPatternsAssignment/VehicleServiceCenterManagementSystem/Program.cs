using VehicleServiceCenterManagementSystem;
using VehicleServiceCenterManagementSystem.Enums;
using VehicleServiceCenterManagementSystem.VehicleService.Contracts;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Factory Pattern for Vehicle Service Center Management System");
        
        Console.WriteLine("Enter Vehicle Type (Car, Bike, Truck): ");
        string vehicleTypeString = Console.ReadLine();

        VehicleTypeEnum vehicleType;
        if (Enum.TryParse(vehicleTypeString, true, out vehicleType))
        {
            if (vehicleType == VehicleTypeEnum.Invalid)
            {
                Console.WriteLine("Invalid Vehicle Type");
                return;
            }

            VehicleServiceFactory vehicleServiceFactory = new VehicleServiceFactory();
            IVehicleService service = vehicleServiceFactory.PerformVehicleServiceOn(vehicleType);
            service.PerformVehicleService();
        }
        else
        {
            Console.WriteLine("Invalid Vehicle Type");
        }


    }
}