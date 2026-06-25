namespace task04;

public interface ISpaceship
{
    int Speed { get; }       // Скорость корабля
    int FirePower { get; }   // Мощность выстрела
    void MoveForward() => Console.WriteLine(this.ToString() + $" moved forward to {this.Speed}");      // Движение вперед
    void Rotate(int angle) => Console.WriteLine(this.ToString() + $" rotated by {angle} degrees");  // Поворот на угол (градусы)
    void Fire() => Console.WriteLine(this.ToString() + $" shoot with {this.FirePower} power");             // Выстрел ракетой
}

public class Fighter : ISpaceship
{
    public int Speed { get; } = 100;
    public int FirePower { get; } = 50;
}

public class Cruiser : ISpaceship
{
    public int Speed { get; } = 50;
    public int FirePower { get; } = 100;
}
