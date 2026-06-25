using Xunit;
using task04;

namespace task04tests;

public class SpaceshipTests
{
    [Fact]
    public void Cruiser_ShouldHaveCorrectStats()
    {
        ISpaceship cruiser = new Cruiser();
        Assert.Equal(50, cruiser.Speed);
        Assert.Equal(100, cruiser.FirePower);
    }

    [Fact]
    public void Fighter_ShouldBeFasterThanCruiser()
    {
        var fighter = new Fighter();
        var cruiser = new Cruiser();
        Assert.True(fighter.Speed > cruiser.Speed);
    }

    [Fact]
    public void Fighter_MoveForward_PrintsCorrectMessageToConsole()
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);

        ISpaceship fighter = new Fighter();

        fighter.MoveForward();
        var expectedMessage = fighter.ToString() + " moved forward to 100" + Environment.NewLine;
        Assert.Equal(expectedMessage, sw.ToString());
    }

    [Fact]
    public void Cruiser_Fire_PrintsCorrectMessageWithHighPower()
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);

        ISpaceship cruiser = new Cruiser();

        cruiser.Fire();
        var expectedMessage = cruiser.ToString() + " shoot with 100 power" + Environment.NewLine;
        Assert.Equal(expectedMessage, sw.ToString());
    }

    [Theory]
    [InlineData(45)]
    [InlineData(-90)]
    [InlineData(180)]
    public void Spaceships_Rotate_HandlesDifferentAngles(int angle)
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);

        ISpaceship fighter = new Fighter();

        fighter.Rotate(angle);

        var expectedMessage = fighter.ToString() + $" rotated by {angle} degrees" + Environment.NewLine;
        Assert.Equal(expectedMessage, sw.ToString());
    }
}
