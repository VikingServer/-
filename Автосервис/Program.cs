class Program
{
    public static void Main(string[] args)
    {
        Warehouse warehouse = new Warehouse();

        int minCountCar = 5;
        int maxCountCar = 20;

        int countCar = Utils.GetRandomNumber(minCountCar, maxCountCar);

        CarService carService = new CarService(warehouse, countCar);

        carService.Work();
    }
}

static class Utils
{
    private static Random s_random = new Random();

    public static int GetRandomNumber(int minValue, int maxValue)
    {
        return s_random.Next(minValue, maxValue);
    }
}

enum NameDetails
{
    Alternator,
    BackSeat,
    Battery,
    BrakePad,
    Bumper,
    Camshaft,
    Coolant,
    DoorHandle,
    Fender,
    Filter,
    Lamp,
    PassengerSeat,
    PetrolCap
}

class Detail
{
    public Detail(NameDetails nameDetails)
    {
        Name = nameDetails;

        int minPrice = 30;
        int maxPrice = 3000;

        Price = Utils.GetRandomNumber(minPrice, maxPrice);

        IsBroken = false;
    }

    public NameDetails Name { get; private set; }
    public int Price { get; private set; }

    public bool IsBroken { get; private set; }

    public void ShowInformation(int number)
    {
        Console.WriteLine($"{number}) {Name}, цена : {Price}");
    }

    public void Break()
    {
        IsBroken = true;
    }
}

class DetailsCreater
{
    public List<Detail> Create()
    {
        List<Detail> details = new List<Detail>();

        int countDetails = Enum.GetValues(typeof(NameDetails)).Length;

        for (int i = 0; i < countDetails; i++)
        {
            details.Add(new Detail((NameDetails)i));
        }

        return details;
    }
}

class Warehouse
{
    private List<Detail> _details = new List<Detail>();

    public Warehouse()
    {
        int minCountDetails = 1;
        int maxCountDetails = 7;

        int countDetails = Utils.GetRandomNumber(minCountDetails, maxCountDetails);

        for (int i = 0; i < countDetails; i++)
        {
            DetailsCreater detailsCreater = new DetailsCreater();

            _details.AddRange(detailsCreater.Create());
        }
    }

    public void RemoveDetail(Detail detail)
    {
        _details.Remove(detail);
    }

    public void ShowInformation()
    {
        Dictionary<NameDetails, int> details = new Dictionary<NameDetails, int>();

        FillDetailDictionary(details);

        foreach (var detail in details)
        {
            Console.WriteLine($"{detail.Key}  -   {detail.Value}");
        }
    }

    public bool TryGetDetail(Detail detailCar, out Detail detailWarehouse)
    {
        detailWarehouse = _details.Find(detail => detail.Name == detailCar.Name);

        return detailWarehouse != null;
    }

    public void Add(Detail detail)
    {
        _details.Add(detail);
    }

    public int GetCountBrokenDetailsInStock(IReadOnlyList<Detail> detailsCar)
    {
        int countSuitableDetails = 0;

        for (int i = 0; i < detailsCar.Count; i++)
        {
            if (detailsCar[i].IsBroken)
            {
                if (TryGetDetail(detailsCar[i], out Detail detailWarehouse))
                    countSuitableDetails++;
                else
                    Console.WriteLine($"{detailsCar[i].Name} нет в наличии");
            }
        }

        return countSuitableDetails;
    }

    private void FillDetailDictionary(Dictionary<NameDetails, int> details)
    {
        DetailsCreater detailsCreater = new DetailsCreater();

        List<Detail> baseDetail = detailsCreater.Create();

        int count = 0;

        for (int i = 0; i < baseDetail.Count; i++)
        {
            details[baseDetail[i].Name] = count;
        }

        for (int i = 0; i < _details.Count; i++)
        {
            if (details.ContainsKey(_details[i].Name))
                details[_details[i].Name]++;
        }
    }
}

class Car
{
    private List<Detail> _details = new List<Detail>();

    public Car()
    {
        DetailsCreater detailsCreater = new DetailsCreater();

        _details = detailsCreater.Create();

        Break();

        DetailsForCarService = _details;
    }

    public IReadOnlyList<Detail> DetailsForCarService { get; private set; }
    public int CountBrokenDetails { get; private set; }

    public bool TryGetBrokenDetail(out Detail detail)
    {
        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].IsBroken)
            {
                detail = _details[i];

                return true;
            }
        }

        detail = null;

        return false;
    }

    public void RemoveBrokenDetail(Detail detailBroken)
    {
        Console.WriteLine($"Снята деталь: {detailBroken.Name}");

        _details.Remove(detailBroken);
    }

    public void AddNewDetail(Detail detail)
    {
        Console.WriteLine($"Поставлена новая деталь {detail.Name}\n");

        _details.Add(detail);
    }

    public void ShowInformation()
    {
        Console.WriteLine("Поломки: ");

        for (int i = 0; i < _details.Count; i++)
        {
            if (_details[i].IsBroken == true)
                _details[i].ShowInformation(i + 1);
        }
    }

    private void Break()
    {
        int minCountBrokenDetails = 1;
        int maxCountBrokenDetails = 7;

        int countBrokenDetails = Utils.GetRandomNumber(minCountBrokenDetails, maxCountBrokenDetails);

        for (int i = 0; i < countBrokenDetails; i++)
        {
            int index = GetIndexDetail(_details.Count);

            if (_details[index].IsBroken == false)
            {
                _details[index].Break();
                CountBrokenDetails++;
            }
        }
    }

    private int GetIndexDetail(int countDetails)
    {
        int minIndex = 0;
        int maxIndex = countDetails;

        return Utils.GetRandomNumber(minIndex, maxIndex);
    }
}

class CarService
{
    private int _money = 0;
    private Warehouse _warehouse;
    private Queue<Car> _cars = new Queue<Car>();

    public CarService(Warehouse warehouse, int countCars)
    {
        _warehouse = warehouse;

        for (int i = 0; i < countCars; i++)
        {
            _cars.Enqueue(new Car());
        }
    }

    public void Work()
    {
        const string CommandFixCar = "1";
        const string CommandBuyDetail = "2";

        Store store = new Store();

        while (_money >= 0 && _cars.Count > 0)
        {
            Console.Clear();
            ShowMoney();

            Console.WriteLine("\nНаличие на складе: ");
            _warehouse.ShowInformation();
            Console.WriteLine();

            Console.Write($"\n{CommandFixCar} - чинить авто\n{CommandBuyDetail} - купить детали\n");

            Console.Write("\nВыберите команду: ");
            string userInput = Console.ReadLine();

            switch (userInput)
            {
                case CommandFixCar:
                    TakeCar();
                    break;

                case CommandBuyDetail:
                    BuyDetail(store);
                    break;

                default:
                    Console.WriteLine("\nТакой команды нет");
                    break;
            }

            Console.ReadKey();
        }

        if (_money < 0)
            Console.WriteLine("Вы банкрот");
        else if (_cars.Count <= 0)
            Console.WriteLine("Смена окончена");
    }

    public bool TryBuyDetail(Detail detail)
    {
        if (_money >= detail.Price)
        {
            _money -= detail.Price;
            _warehouse.Add(detail);

            return true;
        }
        else
        {
            Console.WriteLine("\nУ Вас недостаточно средств");
            Console.WriteLine();

            return false;
        }
    }

    private void ShowMoney()
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine($"Баланс мастерской: {_money}");
    }

    private void DiagnoseCar(Car car)
    {
        if (IsDetailsEnough(car))
            RepairCar(car);
        else
            Penalize();
    }

    private bool IsDetailsEnough(Car car)
    {
        int countSuitableDetails = _warehouse.GetCountBrokenDetailsInStock(car.DetailsForCarService);

        return countSuitableDetails >= car.CountBrokenDetails;
    }

    private void Penalize()
    {
        int penalty = 5000;

        Console.WriteLine("\nКлиент ушел, штраф ");

        _money -= penalty;
    }

    private void RepairCar(Car car)
    {
        int sumPriceRepair = 0;

        for (int i = 0; i < car.CountBrokenDetails; i++)
        {
            if (car.TryGetBrokenDetail(out Detail datailCar))
            {
                if (_warehouse.TryGetDetail(datailCar, out Detail newDetailCar))
                {
                    _warehouse.RemoveDetail(newDetailCar);

                    car.RemoveBrokenDetail(datailCar);
                    car.AddNewDetail(newDetailCar);

                    sumPriceRepair += CalculateRepairs(newDetailCar);
                }
            }
        }

        Console.WriteLine($"Итог: {sumPriceRepair}");
        Console.WriteLine("\nРемонт окончен успешно");
    }

    private void TakeCar()
    {
        Car car = _cars.Dequeue();

        Console.WriteLine();
        car.ShowInformation();
        Console.WriteLine();

        DiagnoseCar(car);
    }

    private int CalculateRepairs(Detail newCarDetail)
    {
        int sumRepair = 0;
        int priceWork = 1000;

        sumRepair += newCarDetail.Price + priceWork;
        _money += sumRepair;

        return sumRepair;
    }

    private void BuyDetail(Store store)
    {
        int detailNumber = store.GetDetailNumber();

        if (store.TryGetDetail(detailNumber, out Detail detail))
        {
            if (TryBuyDetail(detail))
                Console.WriteLine("\nПокупка успешна!");
        }
    }
}

class Store
{
    private List<Detail> _details = new List<Detail>();

    public Store()
    {
        DetailsCreater detailsCreater = new DetailsCreater();

        _details = detailsCreater.Create();
    }

    public bool TryGetDetail(int indexDetail, out Detail detail)
    {
        if (indexDetail >= 0 && indexDetail < _details.Count)
        {
            detail = _details[indexDetail];
            return true;
        }
        else
        {
            Console.WriteLine("\nТакого товара нет");

            detail = null;

            return false;
        }
    }

    public int GetDetailNumber()
    {
        ShowInformation();

        Console.Write("\nВыберите товар: ");
        string userInput = Console.ReadLine();

        int detailNumber = ChooseDetail(userInput);

        return detailNumber;
    }

    private int ChooseDetail(string userInput)
    {
        int productNumber = GetInt(userInput) - 1;

        return productNumber;
    }

    private int GetInt(string userInput)
    {
        int number;

        while (int.TryParse(userInput, out number) == false)
        {
            Console.Write("Введите число: ");
            userInput = Console.ReadLine();
        }

        return number;
    }

    private void ShowInformation()
    {
        Console.WriteLine("\nТовар: ");

        for (int i = 0; i < _details.Count; i++)
        {
            _details[i].ShowInformation(i + 1);
        }
    }
}